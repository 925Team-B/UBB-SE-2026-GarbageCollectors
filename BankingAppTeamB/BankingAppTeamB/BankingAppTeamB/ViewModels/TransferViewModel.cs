using BankingAppTeamB.Models;
using BankingAppTeamB.Models.DTOs;
using BankingAppTeamB.Services;
using BankingAppTeamB.Commands;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BankingAppTeamB.Mocks;

namespace BankingAppTeamB.ViewModels
{
    public class TransferViewModel : ViewModelBase
    {
        private readonly TransferService transferService;

        private int currentStep;
        public int CurrentStep
        {
            get => currentStep;
            set => SetProperty(ref currentStep, value);
        }

        private ObservableCollection<Account> accounts;
        public ObservableCollection<Account> Accounts
        {
            get => accounts;
            set => SetProperty(ref accounts, value);
        }

        private Account selectedAccount;
        public Account SelectedAccount
        {
            get => selectedAccount;
            set => SetProperty(ref selectedAccount, value);
        }

        private string recipientName;
        public string RecipientName
        {
            get => recipientName;
            set => SetProperty(ref recipientName, value);
        }

        private string recipientIBAN;
        public string RecipientIBAN
        {
            get => recipientIBAN;
            set
            {
                SetProperty(ref recipientIBAN, value);
                UpdateIBANValidation(value);
            }
        }

        private bool isIBANValid;
        public bool IsIBANValid
        {
            get => isIBANValid;
            set => SetProperty(ref isIBANValid, value);
        }

        private string bankName;
        public string BankName
        {
            get => bankName;
            set => SetProperty(ref bankName, value);
        }

        private decimal amount;
        public decimal Amount
        {
            get => amount;
            set
            {
                SetProperty(ref amount, value);
                UpdateFxPreview();
                UpdateRequires2FA();
            }
        }

        private string currency;
        public string Currency
        {
            get => currency;
            set
            {
                SetProperty(ref currency, value);
                UpdateFxPreview();
            }
        }

        private string fxPreviewText;
        public string FxPreviewText
        {
            get => fxPreviewText;
            set => SetProperty(ref fxPreviewText, value);
        }

        private string twoFAToken;
        public string TwoFAToken
        {
            get => twoFAToken;
            set => SetProperty(ref twoFAToken, value);
        }

        private bool requires2FA;
        public bool Requires2FA
        {
            get => requires2FA;
            set => SetProperty(ref requires2FA, value);
        }

        private string transactionRef;
        public string TransactionRef
        {
            get => transactionRef;
            set => SetProperty(ref transactionRef, value);
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        private string amountText;
        public string AmountText
        {
            get => amountText;
            set
            {
                SetProperty(ref amountText, value);
                if (decimal.TryParse(value, out decimal parsed))
                {
                    amount = parsed;
                    UpdateFxPreview();
                    UpdateRequires2FA();
                }
            }
        }

        public RelayCommand NextStepCommand { get; }
        public AsyncRelayCommand TransferCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand SendAgainCommand { get; }

        public TransferViewModel(TransferService transferService)
        {
            this.transferService = transferService;
            Accounts = new ObservableCollection<Account>();
            CurrentStep = 1;

            NextStepCommand = new RelayCommand(_ => ExecuteNextStep());
            TransferCommand = new AsyncRelayCommand(_ => ExecuteTransferAsync());
            CancelCommand = new RelayCommand(_ => ExecuteCancel());
            SendAgainCommand = new RelayCommand(_ => ExecuteSendAgain());
        }

        public void LoadAccounts()
        {
            var userAccounts = UserSession.GetAccounts();
            Accounts.Clear();
            foreach (var account in userAccounts)
                Accounts.Add(account);
        }

        private void ExecuteNextStep()
        {
            ErrorMessage = string.Empty;

            switch (CurrentStep)
            {
                case 1:
                    if (SelectedAccount == null)
                    {
                        ErrorMessage = "Please select a source account.";
                        return;
                    }
                    break;

                case 2:
                    if (string.IsNullOrWhiteSpace(RecipientName))
                    {
                        ErrorMessage = "Please enter the recipient name.";
                        return;
                    }
                    if (!IsIBANValid)
                    {
                        ErrorMessage = "Invalid IBAN.";
                        return;
                    }
                    break;

                case 3:
                    if (Amount <= 0)
                    {
                        ErrorMessage = "Amount must be greater than zero.";
                        return;
                    }
                    break;
            }

            CurrentStep++;
        }

        private async Task ExecuteTransferAsync()
        {
            CurrentStep = 5;

            var dto = new TransferDto
            {
                UserId = UserSession.CurrentUserId,
                SourceAccountId = SelectedAccount.Id,
                RecipientName = RecipientName,
                RecipientIBAN = RecipientIBAN,
                Amount = Amount,
                Currency = Currency,
                TwoFAToken = TwoFAToken
            };

            try
            {
                var result = transferService.ExecuteTransfer(dto);
                TransactionRef = result.TransactionId.HasValue
                    ? $"TXN-{result.CreatedAt:yyyyMMdd}-{result.TransactionId:D4}"
                    : result.Id.ToString();

                CurrentStep = 6;
            }
            catch (System.Exception ex)
            {
                ErrorMessage = ex.Message;
                CurrentStep = 4;
            }
        }

        private void ExecuteCancel()
        {
            // TODO: replace with correct view type when Views are implemented
            // NavigationService.NavigateTo<HomeView>();
        }

        private void ExecuteSendAgain()
        {
            SelectedAccount = null;
            RecipientName = string.Empty;
            RecipientIBAN = string.Empty;
            IsIBANValid = false;
            BankName = string.Empty;
            Amount = 0;
            Currency = string.Empty;
            FxPreviewText = string.Empty;
            TwoFAToken = string.Empty;
            Requires2FA = false;
            TransactionRef = string.Empty;
            ErrorMessage = string.Empty;
            AmountText = string.Empty;

            CurrentStep = 1;
        }

        private void UpdateIBANValidation(string iban)
        {
            IsIBANValid = transferService.ValidateIBAN(iban);
            BankName = IsIBANValid
                ? transferService.GetBankNameFromIBAN(iban)
                : string.Empty;
        }

        private void UpdateFxPreview()
        {
            if (Amount <= 0 || string.IsNullOrWhiteSpace(Currency))
            {
                FxPreviewText = string.Empty;
                return;
            }

            var preview = transferService.GetFxPreview(
                SelectedAccount?.Currency ?? Currency,
                Currency,
                Amount);

            if (preview.Rate == 1)
                FxPreviewText = $"{Amount:F2} {Currency}";
            else
                FxPreviewText = $"{Amount:F2} {SelectedAccount?.Currency} → {preview.ConvertedAmount:F2} {Currency} (rate: {preview.Rate:F4})";
        }

        private void UpdateRequires2FA()
        {
            Requires2FA = transferService.Requires2FA(Amount);
        }
    }
}