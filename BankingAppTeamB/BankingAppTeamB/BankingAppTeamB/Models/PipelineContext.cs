namespace BankingAppTeamB.Models
{
    public class PipelineContext
    {
        public int UserId { get; set; }
        public int SourceAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Fee { get; set; }
        public string CounterpartyName { get; set; } = string.Empty;
        public string RelatedEntityType { get; set; } = string.Empty;
        public int RelatedEntityId { get; set; }
    }
}
