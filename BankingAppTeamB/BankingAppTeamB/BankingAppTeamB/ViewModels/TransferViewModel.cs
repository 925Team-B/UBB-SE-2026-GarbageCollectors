using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BankingAppTeamB.Commands;
using BankingAppTeamB.Mocks;
using BankingAppTeamB.Models;
using BankingAppTeamB.Models.DTOs;
using BankingAppTeamB.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankingAppTeamB.Commands;
using BankingAppTeamB.Models;
using BankingAppTeamB.Services;
using BankingAppTeamB.ViewModels;


public class TransferViewModel : ViewModelBase
{
    private readonly TransferService transferService;

    public TransferViewModel(TransferService transferService)
    {
        this.transferService = transferService ?? throw new ArgumentNullException(nameof(transferService));

        Accounts = new ObservableCollection<Account>();
        CurrentStep = 1;

        // ✅ default values (IMPORTANT)
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

            // 🔥 IMPORTANT: notify dependent property
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

                // ✅ select first account automatically
                if (Accounts.Count > 0)
                    SelectedAccount = Accounts[0];
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    private void ExecuteNextStep()
    {
        ErrorMessage = string.Empty;
        
        

        CurrentStep++;
    }

    private async Task ExecuteTransferAsync()
    {
        try
        {
            CurrentStep = 5;

            if (SelectedAccount == null)
                throw new Exception("No account selected.");

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

            var result = await Task.Run(() => transferService.ExecuteTransfer(dto));

            TransactionRef = result.TransactionId.HasValue
                ? $"TXN-{result.CreatedAt:yyyyMMdd}-{result.TransactionId:D4}"
                : result.Id.ToString();

            CurrentStep = 6;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            CurrentStep = 4;
        }
    }

    private void ExecuteCancel()
    {
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
            BankName = IsIBANValid
                ? transferService.GetBankNameFromIBAN(iban)
                : string.Empty;
        }
        catch
        {
            IsIBANValid = false;
            BankName = string.Empty;
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