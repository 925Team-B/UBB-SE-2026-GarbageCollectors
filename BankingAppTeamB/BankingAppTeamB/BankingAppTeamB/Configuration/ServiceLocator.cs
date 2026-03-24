using System.Threading.Tasks;
using System.Timers;
using BankingAppTeamB.Mocks;
using BankingAppTeamB.Repositories;
using BankingAppTeamB.Services;

namespace BankingAppTeamB.Configuration;

public static class ServiceLocator
{

    private static ITransactionRepository _transactionRepository = new TransactionRepository();
    private static ITransferRepository _transferRepository = new TransferRepository();
    private static IBeneficiaryRepository _beneficiaryRepository = new BeneficiaryRepository();
    private static IBillPaymentRepository _billPaymentRepository = new BillPaymentRepository();
    private static IRecurringPaymentRepository _recurringPaymentRepository = new RecurringPaymentRepository();
    private static IExchangeRepository _exchangeRepository = new ExchangeRepository();

    //mock
    private static AccountService _accountService = new AccountService();

    private static TransactionPipelineService _pipelineService = new TransactionPipelineService(_transactionRepository, _accountService);

    private static ExchangeService _exchangeService = new ExchangeService(_exchangeRepository, _pipelineService, _accountService);
    private static TransferService _transferService = new TransferService(_transferRepository, _beneficiaryRepository, _pipelineService, _exchangeService);
    private static BillPaymentService _billPaymentService = new BillPaymentService(_billPaymentRepository, _pipelineService);
    private static RecurringPaymentService _recurringPaymentService = new RecurringPaymentService(_recurringPaymentRepository, _billPaymentService);

    //mock
    private static readonly Timer timer = new Timer(30000) //30secs
    {
        AutoReset = true
    };
    
    private static RecurringScheduler _recurringScheduler = new RecurringScheduler(_recurringPaymentService, _exchangeService, timer);
    
    public static TransferService TransferService => _transferService;
    public static ExchangeService ExchangeService => _exchangeService;
    public static BillPaymentService BillPaymentService => _billPaymentService;
    public static RecurringPaymentService RecurringPaymentService => _recurringPaymentService;
    public static RecurringScheduler RecurringScheduler => _recurringScheduler;

    public static void Initialize()
    {
        _recurringScheduler.Start();
    }

}