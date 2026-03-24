using BankingAppTeamB.Commands;
using BankingAppTeamB.Configuration;
using BankingAppTeamB.Mocks;
using BankingAppTeamB.Models;
using BankingAppTeamB.Models.DTOs;
using BankingAppTeamB.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BankingAppTeamB.ViewModels
{
    public class RecurringPaymentViewModel : ViewModelBase
    {
        private readonly RecurringPaymentService _recurringPaymentService;

        private ObservableCollection<RecurringPayment> _payments;
        private RecurringPayment? _selectedPayment;
        private int _selectedBillerId;
        private decimal _amount;
        private RecurringFrequency _frequency;
        private DateTime _startDate;
        private DateTime? _endDate;
        private bool _isPayInFull;
        private string _errorMessage;

        // helper properties needed by the form
        private ObservableCollection<Account> _accounts;
        private Account? _selectedAccount;

        private ObservableCollection<Biller> _billers;
        private Biller? _selectedBiller;

        private ObservableCollection<RecurringFrequency> _frequencies;

        public RecurringPaymentViewModel(RecurringPaymentService recurringPaymentService)
        {
            _recurringPaymentService = recurringPaymentService;

            _payments = new ObservableCollection<RecurringPayment>();
            _accounts = new ObservableCollection<Account>(UserSession.GetAccounts());
            _billers = new ObservableCollection<Biller>();
            _frequencies = new ObservableCollection<RecurringFrequency>(
                Enum.GetValues(typeof(RecurringFrequency)).Cast<RecurringFrequency>());

            _selectedPayment = null;
            _selectedBillerId = 0;
            _amount = 0;
            _frequency = RecurringFrequency.Weekly;
            _startDate = DateTime.Today;
            _endDate = null;
            _isPayInFull = false;
            _errorMessage = string.Empty;
            _selectedAccount = null;
            _selectedBiller = null;

            CreateCommand = new AsyncRelayCommand(_ => ExecuteCreateAsync());
            PauseCommand = new RelayCommand(ExecutePause);
            ResumeCommand = new RelayCommand(ExecuteResume);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        public ObservableCollection<RecurringPayment> Payments
        {
            get => _payments;
            set => SetProperty(ref _payments, value);
        }

        public RecurringPayment? SelectedPayment
        {
            get => _selectedPayment;
            set => SetProperty(ref _selectedPayment, value);
        }

        public int SelectedBillerId
        {
            get => _selectedBillerId;
            set => SetProperty(ref _selectedBillerId, value);
        }

        public decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        public RecurringFrequency Frequency
        {
            get => _frequency;
            set => SetProperty(ref _frequency, value);
        }

        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        public bool IsPayInFull
        {
            get => _isPayInFull;
            set => SetProperty(ref _isPayInFull, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ObservableCollection<Account> Accounts
        {
            get => _accounts;
            set => SetProperty(ref _accounts, value);
        }

        public Account? SelectedAccount
        {
            get => _selectedAccount;
            set => SetProperty(ref _selectedAccount, value);
        }

        public ObservableCollection<Biller> Billers
        {
            get => _billers;
            set => SetProperty(ref _billers, value);
        }

        public Biller? SelectedBiller
        {
            get => _selectedBiller;
            set
            {
                if (SetProperty(ref _selectedBiller, value))
                {
                    SelectedBillerId = value?.Id ?? 0;
                }
            }
        }

        public ObservableCollection<RecurringFrequency> Frequencies
        {
            get => _frequencies;
            set => SetProperty(ref _frequencies, value);
        }

        public ICommand CreateCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand ResumeCommand { get; }
        public ICommand CancelCommand { get; }

        public async Task LoadAsync()
        {
            try
            {
                ErrorMessage = string.Empty;

                var payments = await Task.Run(() =>
                    _recurringPaymentService.GetByUser(UserSession.CurrentUserId));

                var billers = await Task.Run(() =>
                    ServiceLocator.BillPaymentService.GetBillerDirectory(null));

                Payments = new ObservableCollection<RecurringPayment>(payments);
                Billers = new ObservableCollection<Biller>(billers);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load recurring payments: {ex.Message}";
            }
        }

        private async Task ExecuteCreateAsync()
        {
            try
            {
                ErrorMessage = string.Empty;

                if (SelectedBiller == null)
                {
                    ErrorMessage = "Please select a biller.";
                    return;
                }

                if (SelectedAccount == null)
                {
                    ErrorMessage = "Please select a source account.";
                    return;
                }

                if (Amount <= 0)
                {
                    ErrorMessage = "Please enter a valid amount.";
                    return;
                }

                if (EndDate.HasValue && EndDate.Value.Date < StartDate.Date)
                {
                    ErrorMessage = "End date cannot be earlier than start date.";
                    return;
                }

                var dto = new RecurringPaymentDto
                {
                    UserId = UserSession.CurrentUserId,
                    BillerId = SelectedBiller.Id,
                    SourceAccountId = SelectedAccount.Id,
                    Amount = Amount,
                    IsPayInFull = IsPayInFull,
                    Frequency = Frequency,
                    StartDate = StartDate,
                    EndDate = EndDate
                };

                var createdPayment = await Task.Run(() =>
                    _recurringPaymentService.Create(dto));

                Payments.Add(createdPayment);
                ClearForm();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to create recurring payment: {ex.Message}";
            }
        }

        private void ExecutePause(object? parameter)
        {
            try
            {
                ErrorMessage = string.Empty;

                if (parameter is not RecurringPayment payment)
                {
                    ErrorMessage = "Please select a recurring payment to pause.";
                    return;
                }

                _recurringPaymentService.Pause(payment.Id);

                var existing = Payments.FirstOrDefault(p => p.Id == payment.Id);
                if (existing != null)
                {
                    var index = Payments.IndexOf(existing);
                    existing.Status = PaymentStatus.Paused;
                    Payments[index] = existing;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to pause recurring payment: {ex.Message}";
            }
        }

        private void ExecuteResume(object? parameter)
        {
            try
            {
                ErrorMessage = string.Empty;

                if (parameter is not RecurringPayment payment)
                {
                    ErrorMessage = "Please select a recurring payment to resume.";
                    return;
                }

                _recurringPaymentService.Resume(payment.Id);

                var existing = Payments.FirstOrDefault(p => p.Id == payment.Id);
                if (existing != null)
                {
                    var index = Payments.IndexOf(existing);
                    existing.Status = PaymentStatus.Active;
                    Payments[index] = existing;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to resume recurring payment: {ex.Message}";
            }
        }

        private void ExecuteCancel(object? parameter)
        {
            try
            {
                ErrorMessage = string.Empty;

                if (parameter is not RecurringPayment payment)
                {
                    ErrorMessage = "Please select a recurring payment to cancel.";
                    return;
                }

                _recurringPaymentService.Cancel(payment.Id);

                var existing = Payments.FirstOrDefault(p => p.Id == payment.Id);
                if (existing != null)
                {
                    var index = Payments.IndexOf(existing);
                    existing.Status = PaymentStatus.Cancelled;
                    Payments[index] = existing;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to cancel recurring payment: {ex.Message}";
            }
        }

        private void ClearForm()
        {
            SelectedPayment = null;
            SelectedBiller = null;
            SelectedBillerId = 0;
            SelectedAccount = null;
            Amount = 0;
            Frequency = RecurringFrequency.Weekly;
            StartDate = DateTime.Today;
            EndDate = null;
            IsPayInFull = false;
            ErrorMessage = string.Empty;
        }
    }
}