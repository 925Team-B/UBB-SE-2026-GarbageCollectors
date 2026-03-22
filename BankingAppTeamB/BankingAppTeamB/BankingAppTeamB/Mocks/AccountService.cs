using System;
using BankingAppTeamB.Models;

namespace BankingAppTeamB.Mocks
{
    public class AccountService
    {
        public void DebitAccount(int accountId, decimal amount) { }
        public void CreditAccount(int accountId, decimal amount) { }

        public AccountService() { }

        public bool IsAccountValid(int id)
        {
            return true;
        }

        public decimal GetBalance(int id)
        {
            return 0;
        }
    }
}