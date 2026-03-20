using System;

namespace BankingAppTeamB.Models
{
    public class Transfer
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SourceAccountId { get; set; }
        public int? TransactionId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string RecipientIBAN { get; set; } = string.Empty;
        public string? RecipientBankName { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public decimal? ConvertedAmount { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal Fee { get; set; }
        public string? Reference { get; set; }
        public TransferStatus Status { get; set; }
        public DateTime? EstimatedArrival { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
