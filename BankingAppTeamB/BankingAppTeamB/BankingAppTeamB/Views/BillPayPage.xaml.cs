using BankingAppTeamB.Configuration;
using BankingAppTeamB.Models;
using BankingAppTeamB.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.ComponentModel;

namespace BankingAppTeamB.Views
{
    public sealed partial class BillPayPage : Page
    {
        public BillPayViewModel ViewModel { get; }

        public BillPayPage()
        {
            InitializeComponent();
            ViewModel = new BillPayViewModel(ServiceLocator.BillPaymentService);
            DataContext = ViewModel;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadAsync();
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                ViewModel.SearchCommand.Execute(null);
            }
        }

        private void CategoryCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SearchCommand.Execute(null);
        }

        private void BillersList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Biller biller)
            {
                ViewModel.SelectBillerCommand.Execute(biller);
            }
        }

        private void SavedBillersList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is SavedBiller savedBiller)
            {
                ViewModel.SelectBillerCommand.Execute(savedBiller);
            }
        }

        private void AmountBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (!double.IsNaN(sender.Value) && !double.IsInfinity(sender.Value))
            {
                ViewModel.Amount = Convert.ToDecimal(sender.Value);
            }
            else
            {
                ViewModel.Amount = 0;
            }
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BillPayViewModel.Amount))
            {
                if (AmountBox != null)
                {
                    double newValue = Convert.ToDouble(ViewModel.Amount);

                    if (AmountBox.Value != newValue)
                    {
                        AmountBox.Value = newValue;
                    }
                }
            }
        }
    }
}
