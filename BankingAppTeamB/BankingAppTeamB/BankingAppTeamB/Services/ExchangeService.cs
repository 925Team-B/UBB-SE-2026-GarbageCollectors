
using BankingAppTeamB.Models;
using System;
using System.Collections.Generic;
using BankingAppTeamB.Repositories;

using BankingAppTeamB.Models.DTOs;
using BankingAppTeamB.Mocks;
using Microsoft.IdentityModel.Tokens;


namespace BankingAppTeamB.Services
{
    public class ExchangeService
    {
        private readonly IExchangeRepository _exchangeRepository;
        private readonly TransactionPipelineService _transactionPipelineService;

        private Dictionary<string, decimal> _cachedRates;
        private DateTime _ratesLastFetched;

        private readonly Dictionary<int, LockedRate> _lockedRates = new();
        private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);




        public ExchangeService(IExchangeRepository exchangeRepository, 
            TransactionPipelineService transactionPipelineService)
        {
            _exchangeRepository = exchangeRepository;
            _transactionPipelineService = transactionPipelineService;
        }



        public Dictionary<string, decimal> GetLiveRates()
        {
            if (_cachedRates != null && DateTime.Now - _ratesLastFetched < CacheDuration)
                return _cachedRates;

            Dictionary<string, decimal> rates = new Dictionary<string, decimal>
            {
                { "EUR/USD", 1.15m },
                { "EUR/GBP", 0.86m },
                { "EUR/RON", 5.09m },
                { "USD/RON", 4.41m },
                { "GBP/RON", 5.90m }
            };

            //compute the inverses to be easier if changes needed
            
            List<string> keys = new List<string>(rates.Keys);
            foreach (string pair in keys)
            {
                string[] parts = pair.Split('/');
                string inverseKey = $"{parts[1]}/{parts[0]}";
                rates[inverseKey] = 1 / rates[pair];
            }
            

            _cachedRates = rates;
            _ratesLastFetched = DateTime.Now;
            return _cachedRates;
        }

        public decimal GetRate(string from, String to)
        {
            Dictionary<string, decimal> rates = GetLiveRates();
            string key = $"{from}/{to}";

            if (rates.ContainsKey(key))
                return rates[key];

            string inverseKey = $"{to}/{from}";
            if (rates.ContainsKey(inverseKey))
                return 1 / rates[inverseKey];

            throw new Exception($"rate not found for pair {from}/{to}");
        }

        public LockedRate LockRate(int userId, string from, String to)
        {
            decimal rate = GetRate(from, to);

            LockedRate lockedRate = new LockedRate
            {
                UserId = userId,
                CurrencyPair = $"{from}/{to}",
                Rate = rate,
                LockedAt = DateTime.Now
            };
            _lockedRates[userId] = lockedRate;
            return lockedRate;
        }

        public bool IsRateLockValid(int userId)
        {
            if (!_lockedRates.ContainsKey(userId))
                return false;
            return !_lockedRates[userId].IsExpired();
        }

        public decimal CalculateCommission(decimal amount)
        {
            decimal percentage = amount * 0.005m;
            return Math.Max(0.50m, percentage);
        }

        public decimal CalculateTargetAmount(decimal sourceAmount, decimal rate)
        {
            decimal commission = CalculateCommission(sourceAmount);
            return sourceAmount * rate - commission;
        }

        public ExchangeTransaction ExecuteExchange(ExchangeDto dto)
        {
            if (!IsRateLockValid(dto.UserId))
                throw new Exception("No valid rate lock found or the 3-second window has expired.");

            LockedRate lockedRateEntry = _lockedRates[dto.UserId];

            decimal commission = CalculateCommission(dto.SourceAmount);
            decimal targetAmount=CalculateTargetAmount(dto.SourceAmount, lockedRateEntry.Rate);

            PipelineContext context = new PipelineContext { 
                    UserId=dto.UserId,
                    SourceAccountId=dto.SourceAccountId,
                    Amount=dto.SourceAmount,
                    Currency=dto.SourceCurrency,
                    Type="Exchange",
                    Fee=0,
                    CounterpartyName=$"Exchange to {dto.TargetCurrency}",
                    RelatedEntityType="Exchange",
                    RelatedEntityId=0

                };

            Transaction transactionLog =_transactionPipelineService.RunPipeline(context);
            AccountService.CreditAccount(dto.TargetAccountId, targetAmount);


            ExchangeTransaction exchangeTransaction = new ExchangeTransaction
            {
                UserId = dto.UserId,
                SourceAccountId = dto.SourceAccountId,
                TargetAccountId = dto.TargetAccountId,
                TransactionId = transactionLog.Id, 
                SourceCurrency = dto.SourceCurrency,
                TargetCurrency = dto.TargetCurrency,
                SourceAmount = dto.SourceAmount,
                TargetAmount = targetAmount,
                ExchangeRate = lockedRateEntry.Rate,
                Commission = commission,
                RateLockedAt = lockedRateEntry.LockedAt,
                Status = TransferStatus.Completed,
                CreatedAt = DateTime.Now
            };

            _exchangeRepository.Add(exchangeTransaction);
            _lockedRates.Remove(dto.UserId);

            return exchangeTransaction;



        }
        
        public void ClearLocks(int userId)
        {
            _lockedRates.Remove(userId);
        }


        public List<RateAlert> GetUserAlerts(int userId)
        {
            return _exchangeRepository.GetUserActiveAlerts(userId);
        }

        public RateAlert CreateAlert(int userId, string source, string target, decimal rate, bool isBuyAlert)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException("source currency cannot be null or empty");
            }

            if (string.IsNullOrEmpty(target))
            {
                throw new ArgumentException("target currency cannot be null or empty");
            }

            if (source.Equals(target))
            {
                throw new  ArgumentException("source currency cannot be the same as target currency");
            }

            if (rate <= 0)
            {
                throw new ArgumentException("rate cannot be zero or negative");
            }

            RateAlert rateAlert = new RateAlert(userId, source, target, rate, isBuyAlert);

            return _exchangeRepository.AddAlert(rateAlert);
        }

        public void DeleteAlert(int id)
        {
            _exchangeRepository.DeleteAlert(id);
        }

        public void CheckRateAlerts()
        {
            List<RateAlert> activeAlerts = _exchangeRepository.GetAllActiveAlerts();

            foreach (RateAlert alert in activeAlerts)
            {
                var targetRate = alert.getTargetRate();
                var currentRate = GetRate(alert.getBaseCurrency(), alert.getTargetCurrency());

                if (alert.isBuyAlert())
                {
                    if (currentRate <= targetRate)
                    {
                        Console.WriteLine("The current rate is below the target rate for alert with id " + alert.Id);
                    }
                }
                else
                {
                    if (currentRate > targetRate)
                    {
                        Console.WriteLine("The current rate is above the  target rate for alert with id: " +  alert.Id);
                    }
                }
            }
            
            
        }
        
    }
}

















