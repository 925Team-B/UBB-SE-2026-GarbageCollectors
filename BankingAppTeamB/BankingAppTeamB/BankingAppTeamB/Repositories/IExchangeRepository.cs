using System.Collections.Generic;
using BankingAppTeamB.Models;

namespace BankingAppTeamB.Repositories;

public interface IExchangeRepository
{
    public ExchangeTransaction Add(ExchangeTransaction transaction);
    public List<ExchangeTransaction> GetByUserId(int userId);
    public List<RateAlert> GetUserActiveAlerts(int userId);
    public List<RateAlert> GetAllActiveAlerts();
    public RateAlert AddAlert(RateAlert alert);
    public void DeleteAlert(int id);
    public void MarkAlertTriggered(int id);
    
}