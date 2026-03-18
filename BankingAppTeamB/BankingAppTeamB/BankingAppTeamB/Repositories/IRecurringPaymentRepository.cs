using BankingAppTeamB.Models;
using System;
using System.Collections.Generic;

namespace BankingAppTeamB.Repositories
{
    public interface IRecurringPaymentRepository
    {
        RecurringPayment Add(RecurringPayment recurringPayment);
        RecurringPayment? GetById(int id);
        List<RecurringPayment> GetByUserId(int userId);
        List<RecurringPayment> GetDueBefore(DateTime datetime);
        void Update(RecurringPayment recurringPayment);
        void Delete(int id);
    }
}