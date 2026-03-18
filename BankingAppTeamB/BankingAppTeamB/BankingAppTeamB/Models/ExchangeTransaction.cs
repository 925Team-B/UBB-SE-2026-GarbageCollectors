using System;

namespace BankingAppTeamB.Models
{
    public class ExchangeTransaction
    {   
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SourceAccountId {  get; set; }
        public int TargetAccountId {  get; set; }
        public int? TransactionId { get; set; }
        public string SourceCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public decimal SourceAmount { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal Commission {  get; set; }
        public DateTime RateLockedAt { get; set; }
        public TransferStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }


        public int getId() { return Id; }
        public TransferStatus getStatus() { return Status; }
        public void setStatus(TransferStatus s) { Status = s; }
        public decimal getExchangeRate() { return  ExchangeRate; }
        public decimal getTargetAmount() { return TargetAmount;  }


    }
}
