using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BankingAppTeamB.Commands;
using Microsoft.UI.Xaml;
using BankingAppTeamB.Models;
using BankingAppTeamB.Services;
using BankingAppTeamB.Mocks;

namespace BankingAppTeamB.ViewModels;

public class FXViewModel : ViewModelBase
{
    private readonly ExchangeService _exchangeService;


    private int _currentStep;

    public int CurrentStep
    {
        get => _currentStep;
        set => SetProperty(ref _currentStep, value);
    }

    public ObservableCollection<Account> Accounts { get; } = new();

    private Account? _sourceAccount;

    public Account? SourceAccount
    {
        get => _sourceAccount;
        set
        {
            if (SetProperty(ref _sourceAccount, value))
            {
                SourceCurrency = value?.Currency ?? "";
            }
        }
    }

    private Account? _targetAccount;

    public Account? TargetAccount
    {
        get => _targetAccount;
        set
        {
            if (SetProperty(ref _targetAccount, value))
            {
                TargetCurrency = value?.Currency ?? "";
            }
        }
    }

    private string _sourceCurrency = "";

    public string SourceCurrency
    {
        get => _sourceCurrency;
        set
        {
            if (SetProperty(ref _sourceCurrency, value))
                Recalculate();
        }
    }

    private string _targetCurrency = "";

    public string TargetCurrency
    {
        get => _targetCurrency;
        set
        {
            if (SetProperty(ref _targetCurrency, value))
                Recalculate();
        }
    }

    private decimal _amount;

    public decimal Amount
    {
        get => _amount;
        set
        {
            if (SetProperty(ref _amount, value))
                Recalculate();
        }
    }

    private decimal _liveRate;

    public decimal LiveRate
    {
        get => _liveRate;
        set => SetProperty(ref _liveRate, value);
    }

    private decimal _commission;

    public decimal Commission
    {
        get => _commission;
        set => SetProperty(ref _commission, value);
    }

    private decimal _targetAmount;

    public decimal TargetAmount
    {
        get => _targetAmount;
        set => SetProperty(ref _targetAmount, value);
    }

    private int _secondsRemaining;

    public int SecondsRemaining
    {
        get => _secondsRemaining;
        set => SetProperty(ref _secondsRemaining, value);
    }

    private bool _isRateExpired;

    public bool IsRateExpired
    {
        get => _isRateExpired;
        set => SetProperty(ref _isRateExpired, value);
    }

    private string _transactionRef = "";

    public string TransactionRef
    {
        get => _transactionRef;
        set => SetProperty(ref _transactionRef, value);
    }

    private string _errorMessage = "";

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }


    public AsyncRelayCommand LoadRatesCommand { get; }
    public RelayCommand LockRateCommand { get; }


    private DispatcherTimer? _timer;
    private LockedRate? _lockedRate;


    public FXViewModel(ExchangeService exchangeService)
    {
        _exchangeService = exchangeService;

        LoadRatesCommand = new AsyncRelayCommand(LoadRatesAsync);
        LockRateCommand = new RelayCommand(LockRate);

        // Load mock accounts immediately
        _ = LoadAccountsAsync();
    }

    public Task LoadAccountsAsync()
    {
        Accounts.Clear();

        foreach (var account in UserSession.GetAccounts())
            Accounts.Add(account);

        return Task.CompletedTask;
    }


    private Task LoadRatesAsync(object? _)
    {
        try
        {
            ErrorMessage = "";

            if (string.IsNullOrWhiteSpace(SourceCurrency) ||
                string.IsNullOrWhiteSpace(TargetCurrency))
                return Task.CompletedTask;

            LiveRate = _exchangeService.GetRate(SourceCurrency, TargetCurrency);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }

        return Task.CompletedTask;
    }


    private void Recalculate()
    {
        try
        {
            ErrorMessage = "";

            if (string.IsNullOrWhiteSpace(SourceCurrency) ||
                string.IsNullOrWhiteSpace(TargetCurrency) ||
                Amount <= 0)
                return;

            // Same currency shortcut
            if (SourceCurrency == TargetCurrency)
            {
                LiveRate = 1;
                Commission = 0;
                TargetAmount = Amount;
                return;
            }

            LiveRate = _exchangeService.GetRate(SourceCurrency, TargetCurrency);
            Commission = _exchangeService.CalculateCommission(Amount);
            TargetAmount = _exchangeService.CalculateTargetAmount(Amount, LiveRate);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }


    private void LockRate(object? _)
    {
        try
        {
            ErrorMessage = "";

            _lockedRate = _exchangeService.LockRate(UserSession.CurrentUserId, SourceCurrency, TargetCurrency);

            CurrentStep = 4;

            StartCountdownTimer();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    private void StartCountdownTimer()
    {
        if (_lockedRate == null)
            return;

        IsRateExpired = false;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _timer.Tick += (s, e) =>
        {
            SecondsRemaining = _lockedRate.SecondsRemaining();

            if (SecondsRemaining <= 0)
            {
                _timer.Stop();
                IsRateExpired = true;
            }
        };

        _timer.Start();
    }
}