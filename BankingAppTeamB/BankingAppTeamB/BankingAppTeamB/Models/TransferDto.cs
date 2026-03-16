namespace BankingAppTeamB.Models
{
    public class TransferDto
    {
        public int UserId { get; set; }
        public int SourceAccountId { get; set; }
        public string RecipientName { get; set; }
        public string RecipientIBAN { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string? Reference { get; set; }
        public string? TwoFAToken { get; set; }
    }
}
