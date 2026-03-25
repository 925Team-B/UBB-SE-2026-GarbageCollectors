using BankingAppTeamB.Repositories;
using BankingAppTeamB.Services;
using BankingAppTeamB.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace BankingAppTeamB.Views
{
    public sealed partial class BeneficiariesPage : Page
    {
        public BeneficiariesPage()
        {
            InitializeComponent();
            DataContext = new BeneficiariesViewModel(new BeneficiaryService(new BeneficiaryRepository()));
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (DataContext is BeneficiariesViewModel viewModel)
            {
                await viewModel.LoadAsync();
            }
        }

        private void CancelAdd_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is BeneficiariesViewModel viewModel)
            {
                viewModel.IsAddFormVisible = false;
            }
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isVisible)
            {
                return isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }

    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return string.IsNullOrWhiteSpace(value as string) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}