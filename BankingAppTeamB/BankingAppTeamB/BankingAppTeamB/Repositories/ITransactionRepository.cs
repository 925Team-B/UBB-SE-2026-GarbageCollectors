using BankingAppTeamB.Models;
using System.Collections.Generic;

namespace BankingAppTeamB.Repositories
{
    public interface ITransactionRepository
    {
        void Add(Transaction t);
        Transaction GetById(int id);
        List<Transaction> GetByUserId(int uid);
    }
}
