using BankingAppTeamB.Models.DTOs;
using BankingAppTeamB.Services;
using BankingAppTeamB.ViewModels;
using BankingAppTeamB.Repositories;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Controls;
using BankingAppTeamB.Configuration;

namespace BankingAppTeamB.Views
{
    public sealed partial class TransferPage : Page
    {
        public TransferViewModel ViewModel { get; }

        public TransferPage()
        {
            this.InitializeComponent();

            ViewModel = new TransferViewModel(ServiceLocator.TransferService);
            this.DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel.LoadAccounts();

            if (e.Parameter is TransferDto dto)
            {
                ViewModel.RecipientName = dto.RecipientName;
                ViewModel.RecipientIBAN = dto.RecipientIBAN;
                ViewModel.Amount        = dto.Amount;
                ViewModel.Currency      = dto.Currency;
                ViewModel.TwoFAToken    = dto.TwoFAToken;
            }
        }
    }
}
