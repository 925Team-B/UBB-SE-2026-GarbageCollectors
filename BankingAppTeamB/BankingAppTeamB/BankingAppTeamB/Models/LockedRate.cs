using System;

namespace BankingAppTeamB.Models
{
    public class LockedRate
    {
        public int UserId { get; set; }
        public string CurrencyPair { get; set; }
        public decimal Rate { get; set; }
        public DateTime LockedAt { get; set; }

        public bool IsExpired()
        {
            return (DateTime.Now - LockedAt).TotalSeconds > 30;
        }

        public int SecondsRemaining()
        {
            var elapsed= (DateTime.Now - LockedAt).TotalSeconds;
            int remaining = 30 - (int)Math.Floor(elapsed);
            return Math.Max(remaining, 0);
        }
    }
}
