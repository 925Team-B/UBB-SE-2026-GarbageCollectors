using System;

namespace BankingAppTeamB.Models
{
    public class Beneficiary
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string IBAN { get; set; }
        public string? BankName { get; set; }
        public DateTime? LastTransferDate { get; set; }
        public decimal TotalAmountSent { get; set; }
        public int TransferCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
