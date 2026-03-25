using BankingAppTeamB.Configuration;
using BankingAppTeamB.Models;
using BankingAppTeamB.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace BankingAppTeamB.Views
{
    public sealed partial class BillPayPage : Page
    {
        private readonly BillPayViewModel _viewModel;

        public BillPayPage()
        {
            InitializeComponent();
            _viewModel = new BillPayViewModel(ServiceLocator.BillPaymentService);
            DataContext = _viewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await _viewModel.LoadAsync();
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                _viewModel.SearchCommand.Execute(null);
            }
        }

        private void CategoryCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.SearchCommand.Execute(null);
        }

        private void BillersList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Biller biller)
            {
                _viewModel.SelectBillerCommand.Execute(biller);
            }
        }

        private void SavedBillersList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is SavedBiller savedBiller)
            {
                _viewModel.SelectBillerCommand.Execute(savedBiller);
            }
        }

        private void AmountBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (!double.IsNaN(sender.Value) && !double.IsInfinity(sender.Value))
            {
                _viewModel.Amount = Convert.ToDecimal(sender.Value);
            }
            else
            {
                _viewModel.Amount = 0;
            }
        }
    }
}



/*using BankingAppTeamB.Configuration;
using BankingAppTeamB.Models;
using BankingAppTeamB.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Specialized;
using System.ComponentModel;

namespace BankingAppTeamB.Views
{
    public sealed partial class BillPayPage : Page
    {
        private readonly BillPayViewModel _viewModel;

        public BillPayPage()
        {
            InitializeComponent();
            _viewModel = new BillPayViewModel(ServiceLocator.BillPaymentService);
            DataContext = _viewModel;

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await _viewModel.LoadAsync();

            SubscribeToSavedBillersCollection();
            UpdateSavedBillersVisibility();
            UpdateAmountVisibility();
        }

        private void SubscribeToSavedBillersCollection()
        {
            if (_viewModel.SavedBillers != null)
            {
                _viewModel.SavedBillers.CollectionChanged -= SavedBillers_CollectionChanged;
                _viewModel.SavedBillers.CollectionChanged += SavedBillers_CollectionChanged;
            }
        }

        private void SavedBillers_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateSavedBillersVisibility();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BillPayViewModel.IsPayInFull))
            {
                UpdateAmountVisibility();
            }

            if (e.PropertyName == nameof(BillPayViewModel.SavedBillers))
            {
                SubscribeToSavedBillersCollection();
                UpdateSavedBillersVisibility();
            }
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (DataContext is BillPayViewModel vm)
            {
                vm.SearchCommand.Execute(null);
            }
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is BillPayViewModel vm)
            {
                vm.SearchCommand.Execute(null);
            }
        }

        private void BillersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (DataContext is BillPayViewModel vm && e.ClickedItem is Biller biller)
            {
                vm.SelectBillerCommand.Execute(biller);
            }
        }

        private void SavedBillersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (DataContext is BillPayViewModel vm && e.ClickedItem is SavedBiller savedBiller)
            {
                vm.SelectSavedBiller(savedBiller);
            }
        }

        private void UpdateSavedBillersVisibility()
        {
            SavedBillersSection.Visibility =
                _viewModel.SavedBillers != null && _viewModel.SavedBillers.Count > 0
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void UpdateAmountVisibility()
        {
            AmountContainer.Visibility = _viewModel.IsPayInFull
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }
}


using BankingAppTeamB.Configuration;
using BankingAppTeamB.Models;
using BankingAppTeamB.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace BankingAppTeamB.Views
{
    public sealed partial class BillPayPage : Page
    {
        private readonly BillPayViewModel _viewModel;

        public BillPayPage()
        {
            InitializeComponent();
            _viewModel = new BillPayViewModel(ServiceLocator.BillPaymentService);
            DataContext = _viewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await _viewModel.LoadAsync();
            UpdateAmountVisibility();
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (DataContext is BillPayViewModel vm)
            {
                vm.SearchCommand.Execute(null);
            }
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is BillPayViewModel vm)
            {
                vm.SearchCommand.Execute(null);
            }
        }

        private void BillersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (DataContext is BillPayViewModel vm && e.ClickedItem is Biller biller)
            {
                vm.SelectBillerCommand.Execute(biller);
            }
        }

        private void SavedBillersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (DataContext is BillPayViewModel vm && e.ClickedItem is SavedBiller savedBiller)
            {
                vm.SelectSavedBiller(savedBiller);
            }
        }

        private void PayInFullToggle_Toggled(object sender, RoutedEventArgs e)
        {
            UpdateAmountVisibility();
        }

        private void UpdateAmountVisibility()
        {
            if (DataContext is BillPayViewModel vm)
            {
                AmountContainer.Visibility = vm.IsPayInFull
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }
    }
}
using BankingAppTeamB.Configuration;
using BankingAppTeamB.Models;
using BankingAppTeamB.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace BankingAppTeamB.Views
{
    public sealed partial class BillPayPage : Page
    {
        private readonly BillPayViewModel _viewModel;

        public BillPayPage()
        {
            this.InitializeComponent();
            _viewModel = new BillPayViewModel(ServiceLocator.BillPaymentService);
            DataContext = _viewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await _viewModel.LoadAsync();
            UpdateAmountVisibility();
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (DataContext is BillPayViewModel vm)
            {
                vm.SearchCommand.Execute(null);
            }
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is BillPayViewModel vm)
            {
                vm.SearchCommand.Execute(null);
            }
        }

        private void BillersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (DataContext is BillPayViewModel vm && e.ClickedItem is Biller biller)
            {
                vm.SelectBiller(biller);
            }
        }

        private void SavedBillersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (DataContext is BillPayViewModel vm && e.ClickedItem is SavedBiller savedBiller)
            {
                vm.SelectSavedBiller(savedBiller);
            }
        }

        private void PayInFullToggle_Toggled(object sender, RoutedEventArgs e)
        {
            UpdateAmountVisibility();
        }

        private void UpdateAmountVisibility()
        {
            if (DataContext is BillPayViewModel vm)
            {
                AmountContainer.Visibility = vm.IsPayInFull
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }
    }
}
*/
//using BankingAppTeamB.Configuration;
//using BankingAppTeamB.ViewModels;
//using Microsoft.UI.Xaml.Controls;
//using Microsoft.UI.Xaml.Navigation;

//namespace BankingAppTeamB.Views
//{
//    public sealed partial class BillPayPage : Page
//    {
//        private readonly BillPayViewModel _viewModel;

//        public BillPayPage()
//        {
//            this.InitializeComponent();
//            _viewModel = new BillPayViewModel(ServiceLocator.BillPaymentService);
//            this.DataContext = _viewModel;
//        }

//        protected override async void OnNavigatedTo(NavigationEventArgs e)
//        {
//            base.OnNavigatedTo(e);
//            await _viewModel.LoadAsync();
//        }

//        private void AmountBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
//        {
//            if (DataContext is BillPayViewModel vm)
//            {
//                vm.Amount = (decimal)sender.Value;
//            }
//        }
//    }
//}