namespace BankingAppTeamB.Mocks
{
    public static class AccountService
    {
        public static void DebitAccount(int accountId, decimal amount) { }
        public static void CreditAccount(int accountId, decimal amount) { }
        public static bool IsAccountValid(int accountId) => true;
        public static decimal GetBalance(int accountId) => 10000m;
    }
}