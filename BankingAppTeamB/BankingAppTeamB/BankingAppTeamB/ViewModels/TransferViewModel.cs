using BankingAppTeamB.Commands;
using BankingAppTeamB.Mocks;
using BankingAppTeamB.Models;
using BankingAppTeamB.Models.DTOs;
using BankingAppTeamB.Services;
using BankingAppTeamB.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

public class TransferViewModel : ViewModelBase
{
    private readonly TransferService transferService;

    public TransferViewModel(TransferService transferService)
    {
        this.transferService = transferService ?? throw new ArgumentNullException(nameof(transferService));

        Accounts = new ObservableCollection<Account>();
        CurrentStep = 1;

        Currency = "EUR";
        AmountText = "";

        NextStepCommand = new RelayCommand(_ => ExecuteNextStep());
        TransferCommand = new AsyncRelayCommand(_ => ExecuteTransferAsync());
        CancelCommand = new RelayCommand(_ => ExecuteCancel());
        SendAgainCommand = new RelayCommand(_ => ExecuteSendAgain());

        LoadAccounts();
    }

    public string SelectedAccountName => SelectedAccount?.AccountName ?? "";

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
        set
        {
            SetProperty(ref selectedAccount, value);
            OnPropertyChanged(nameof(SelectedAccountName));
            UpdateFxPreview();
        }
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

    private bool is2FAConfirmed;
    public bool Is2FAConfirmed
    {
        get => is2FAConfirmed;
        set => SetProperty(ref is2FAConfirmed, value);
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
                Amount = parsed;
            }
        }
    }

    public RelayCommand NextStepCommand { get; }
    public AsyncRelayCommand TransferCommand { get; }
    public RelayCommand CancelCommand { get; }
    public RelayCommand SendAgainCommand { get; }

    public void LoadAccounts()
    {
        try
        {
            var userAccounts = UserSession.GetAccounts();

            Accounts.Clear();

            if (userAccounts != null)
            {
                foreach (var account in userAccounts)
                    Accounts.Add(account);

                if (Accounts.Count > 0)
                    SelectedAccount = Accounts[0];
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    private string GenerateTwoFAToken()
    {
        var rnd = new Random();
        return rnd.Next(100000, 999999).ToString();
    }

    private void ExecuteNextStep()
    {
        ErrorMessage = string.Empty;

        if (CurrentStep == 2 && !IsIBANValid)
        {
            ErrorMessage = "Cannot continue: IBAN is invalid. Please correct or start again.";
            return; // stop advancing
        }

        if (CurrentStep == 3)
        {
            CurrentStep = Requires2FA ? 4 : 5;
            return;
        }

        if (CurrentStep == 4)
        {
            if (!Is2FAConfirmed)
            {
                ErrorMessage = "You must confirm the 2FA step.";
                return;
            }

            if (Requires2FA && string.IsNullOrWhiteSpace(TwoFAToken))
            {
                TwoFAToken = GenerateTwoFAToken();
            }

            CurrentStep = 5;
            return;
        }

        CurrentStep++;
    }

    private async Task ExecuteTransferAsync()
    {
        try
        {
            ErrorMessage = string.Empty;

            if (SelectedAccount == null)
                throw new Exception("No account selected.");

            // Send the generated token for 2FA transfers
            var dto = new TransferDto
            {
                UserId = UserSession.CurrentUserId,
                SourceAccountId = SelectedAccount.Id,
                RecipientName = RecipientName,
                RecipientIBAN = RecipientIBAN,
                Amount = Amount,
                Currency = Currency,
                TwoFAToken = Requires2FA ? TwoFAToken : null
            };

            var result = await Task.Run(() => transferService.ExecuteTransfer(dto));

            TransactionRef = result.TransactionId.HasValue
                ? $"TXN-{result.CreatedAt:yyyyMMdd}-{result.TransactionId:D4}"
                : result.Id.ToString();

            CurrentStep = 6; // success screen
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;

            // stay on 2FA confirmation if required
            CurrentStep = Requires2FA ? 4 : 5;
        }
    }

    private void ExecuteCancel()
    {
        // Optional: implement cancel logic
    }

    private void ExecuteSendAgain()
    {
        SelectedAccount = Accounts.Count > 0 ? Accounts[0] : null;
        RecipientName = string.Empty;
        RecipientIBAN = string.Empty;
        IsIBANValid = false;
        BankName = string.Empty;
        Amount = 0;
        Currency = "EUR";
        FxPreviewText = string.Empty;
        TwoFAToken = string.Empty;
        Requires2FA = false;
        Is2FAConfirmed = false;
        TransactionRef = string.Empty;
        ErrorMessage = string.Empty;
        AmountText = string.Empty;

        CurrentStep = 1;
    }

    private void UpdateIBANValidation(string iban)
    {
        try
        {
            IsIBANValid = transferService.ValidateIBAN(iban);
            if (!IsIBANValid)
            {
                // Navigate to Invalid IBAN step
                CurrentStep = 7;
            }
            else
            {
                BankName = transferService.GetBankNameFromIBAN(iban);
            }
        }
        catch
        {
            IsIBANValid = false;
            BankName = string.Empty;
            CurrentStep = 7; // go to Invalid IBAN step if exception
        }
    }

    private void UpdateFxPreview()
    {
        try
        {
            if (SelectedAccount == null || Amount <= 0 || string.IsNullOrWhiteSpace(Currency))
            {
                FxPreviewText = string.Empty;
                return;
            }

            var preview = transferService.GetFxPreview(
                SelectedAccount.Currency,
                Currency,
                Amount);

            if (preview.Rate == 1)
            {
                FxPreviewText = $"{Amount:F2} {Currency}";
            }
            else
            {
                FxPreviewText =
                    $"{Amount:F2} {SelectedAccount.Currency} → {preview.ConvertedAmount:F2} {Currency} (rate: {preview.Rate:F4})";
            }
        }
        catch
        {
            FxPreviewText = string.Empty;
        }
    }

    private void UpdateRequires2FA()
    {
        try
        {
            Requires2FA = transferService.Requires2FA(Amount);
        }
        catch
        {
            Requires2FA = false;
        }
    }
}