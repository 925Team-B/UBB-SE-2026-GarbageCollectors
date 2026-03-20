namespace BankingAppTeamB.Models.DTOs
{
    public class TransferDto
    {
        public int UserId { get; set; }
        public int SourceAccountId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string RecipientIBAN { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public string? TwoFAToken { get; set; }
    }
}
