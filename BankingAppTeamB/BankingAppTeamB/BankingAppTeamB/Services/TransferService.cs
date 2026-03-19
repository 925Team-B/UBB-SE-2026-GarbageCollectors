using BankingAppTeamB.Models;
using BankingAppTeamB.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Text;

namespace BankingAppTeamB.Services
{
    public class TransferService
    {
        private readonly ITransferRepository transferRepo;
        private readonly IBeneficiaryRepository beneficiaryRepo;
        private readonly TransactionPipelineService pipeline;
        private readonly ExchangeService? exchangeService;

        public TransferService(
            ITransferRepository transferRepo,
            IBeneficiaryRepository beneficiaryRepo,
            TransactionPipelineService pipeline,
            ExchangeService? exchangeService = null)
        {
            this.transferRepo    = transferRepo;
            this.beneficiaryRepo = beneficiaryRepo;
            this.pipeline        = pipeline;
            this.exchangeService = exchangeService;
        }

        public Transfer ExecuteTransfer(TransferDto dto)
        {
            if (!ValidateIBAN(dto.RecipientIBAN))
                throw new InvalidOperationException("Recipient IBAN is invalid.");

            var context = new PipelineContext
            {
                UserId             = dto.UserId,
                SourceAccountId    = dto.SourceAccountId,
                Amount             = dto.Amount,
                Currency           = dto.Currency,
                Type               = "Transfer",
                Fee                = 0,
                CounterpartyName   = dto.RecipientName,
                RelatedEntityType  = "Transfer",
                RelatedEntityId    = 0
            };

            var transaction = pipeline.RunPipeline(context, dto.TwoFAToken);

            var transfer = new Transfer
            {
                UserId            = dto.UserId,
                SourceAccountId   = dto.SourceAccountId,
                TransactionId     = transaction.Id,
                RecipientName     = dto.RecipientName,
                RecipientIBAN     = dto.RecipientIBAN,
                RecipientBankName = GetBankNameFromIBAN(dto.RecipientIBAN),
                Amount            = dto.Amount,
                Currency          = dto.Currency,
                Fee               = 0,
                Reference         = dto.Reference,
                Status            = "Completed",
                CreatedAt         = DateTime.UtcNow
            };

            transferRepo.Add(transfer);
            return transfer;
        }

        public bool ValidateIBAN(string iban)
        {
            if (string.IsNullOrWhiteSpace(iban)) return false;
            if (iban.Length < 15 || iban.Length > 34) return false;
            if (!char.IsLetter(iban[0]) || !char.IsLetter(iban[1])) return false;
            if (!char.IsDigit(iban[2]) || !char.IsDigit(iban[3])) return false;
            /*
              checksum modulo 97 explained: (Wikipedia)
              Reorder: Move the first four characters(country code + check digits) to the end of the string.
              Convert: Replace letters with digits(A= 10, B= 11, ..., Z = 35).
              Calculate: Interpret the resulting string as a large integer and compute its remainder when divided by 97(using Big Integer math).
              Validate: If the remainder is
,             the IBAN is valid.
            */
            string rearrangedIban = iban.Substring(4) + iban.Substring(0, 4);
            StringBuilder numericIban = new System.Text.StringBuilder();
            foreach (char c in rearrangedIban)
            {
                if (char.IsLetter(c)){
                    numericIban.Append(c - 'A' + 10);
                }
                else if (char.IsDigit(c)){
                    numericIban.Append(c);
                }
                else{
                    // If there's a special character somehow, it's invalid
                    return false;
                }
            }
            if (System.Numerics.BigInteger.TryParse(numericIban.ToString(), out System.Numerics.BigInteger giantNumber)){
                return giantNumber % 97 == 1;
            }

            return false;
        }

        public string GetBankNameFromIBAN(string iban)
        {
            if (string.IsNullOrWhiteSpace(iban) || iban.Length < 2)
                return "Unknown Bank";

            string countryCode = iban.Substring(0, 2).ToUpper();
            return countryCode switch
            {
                "RO" => "Romanian Bank",
                "DE" => "German Bank",
                "GB" => "UK Bank",
                "FR" => "French Bank",
                "US" => "US Bank",
                _    => "International Bank"
            };
        }

        public FxPreview GetFxPreview(string src, string tgt, decimal amt)
        {
            if (src.Equals(tgt, StringComparison.OrdinalIgnoreCase))
                return new FxPreview { Rate = 1, ConvertedAmount = amt };

            if (exchangeService == null)
                return new FxPreview { Rate = 1, ConvertedAmount = amt };

            var rates = exchangeService.GetLiveRates();
            string pair = $"{src.ToUpper()}/{tgt.ToUpper()}";

            if (!rates.ContainsKey(pair))
                return new FxPreview { Rate = 1, ConvertedAmount = amt };

            decimal rate = rates[pair];
            return new FxPreview
            {
                Rate            = rate,
                ConvertedAmount = Math.Round(amt * rate, 2)
            };
        }

        public List<Transfer> GetHistory(int userId)
        {
            return transferRepo.GetByUserId(userId);
        }

        public bool Requires2FA(decimal amount)
        {
            return amount >= 1000;
        }
    }
}
