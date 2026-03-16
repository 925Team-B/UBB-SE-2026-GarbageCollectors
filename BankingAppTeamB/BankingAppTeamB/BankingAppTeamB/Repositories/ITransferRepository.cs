using BankingAppTeamB.Models;
using System.Collections.Generic;

namespace BankingAppTeamB.Repositories
{
    public interface ITransferRepository
    {
        void Add(Transfer t);
        Transfer GetById(int id);
        List<Transfer> GetByUserId(int uid);
        void UpdateStatus(int id, string s);
    }
}
