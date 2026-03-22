using System;


namespace BankingAppTeamB.Models
{
    public class RateAlert
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string BaseCurrency {  get; set; }
        public string TargetCurrency { get; set; }
        public decimal TargetRate { get; set; }
        public bool IsTriggered { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsBuyAlert { get; set; }

        public RateAlert(int userId, string baseCurrency, string targetCurrency, decimal rate, bool isBuyAlert)
        {
            Id = userId;
            BaseCurrency = baseCurrency;
            TargetCurrency = targetCurrency;
            TargetRate = rate;
            IsTriggered = false;
            CreatedAt = DateTime.Now;
            IsBuyAlert = isBuyAlert;
        }

        public RateAlert(){}
        
        public bool isBuyAlert(){ return IsBuyAlert; }
        public int getId() { return Id; }
        public bool isTriggered() { return IsTriggered; }
        public void setTriggered(bool v) { IsTriggered = v; }
        public decimal getTargetRate() { return TargetRate; }
        public string getBaseCurrency() { return BaseCurrency; }
        public string getTargetCurrency() { return TargetCurrency; }
    }
}
