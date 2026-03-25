namespace BankingAppTeamB.Models.DTOs
{
	public class BillPaymentDto
	{
		public int UserId { get; set; }
		public int SourceAccountId { get; set; }
		public int BillerId { get; set; }
		public string BillerReference { get; set; }
		public decimal Amount { get; set; }
		public bool IsPayInFull { get; set; }
		public string? TwoFAToken { get; set; }
	}
}