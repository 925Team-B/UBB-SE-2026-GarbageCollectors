using System;

namespace BankingAppTeamB.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int? CardId { get; set; }
        public string TransactionRef { get; set; }
        public string Type { get; set; }
        public string Direction { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public decimal BalanceAfter { get; set; }
        public string CounterpartyName { get; set; }
        public string? CounterpartyIBAN { get; set; }
        public decimal Fee { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string Status { get; set; }
        public string? RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
