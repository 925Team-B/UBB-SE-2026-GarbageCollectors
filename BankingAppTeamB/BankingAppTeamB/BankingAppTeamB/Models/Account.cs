using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppTeamB.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string IBAN { get; set; } = "";
        public string Currency { get; set; } = "";
        public decimal Balance { get; set; }
        public string AccountName { get; set; } = "";
        public string Status { get; set; } = "";
        
    }
}
