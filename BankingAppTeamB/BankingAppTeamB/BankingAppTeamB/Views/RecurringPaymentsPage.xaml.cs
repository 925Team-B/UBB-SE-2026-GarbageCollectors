using BankingAppTeamB.Configuration;
using BankingAppTeamB.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace BankingAppTeamB.Views
{
    public sealed partial class RecurringPaymentsPage : Page
    {
        private readonly RecurringPaymentViewModel _viewModel;

        public RecurringPaymentsPage()
        {
            InitializeComponent();
            _viewModel = new RecurringPaymentViewModel(ServiceLocator.RecurringPaymentService);
            DataContext = _viewModel;

            StartDatePicker.Date = DateTimeOffset.Now;
            EndDatePicker.Date = DateTimeOffset.Now;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await _viewModel.LoadAsync();
        }

        private void AmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is RecurringPaymentViewModel vm && sender is TextBox textBox)
            {
                if (decimal.TryParse(textBox.Text, out decimal value))
                {
                    vm.Amount = value;
                }
                else
                {
                    vm.Amount = 0;
                }
            }
        }

        private void StartDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs args)
        {
            if (DataContext is RecurringPaymentViewModel vm && sender is DatePicker picker)
            {
                vm.StartDate = picker.Date.DateTime.Date;
            }
        }

        private void EndDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs args)
        {
            if (DataContext is RecurringPaymentViewModel vm && sender is DatePicker picker)
            {
                vm.EndDate = picker.Date.DateTime.Date;
            }
        }
    }
}