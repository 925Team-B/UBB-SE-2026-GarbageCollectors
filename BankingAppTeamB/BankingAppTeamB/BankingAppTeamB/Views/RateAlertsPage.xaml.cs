using BankingAppTeamB.Configuration;
using BankingAppTeamB.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using BankingAppTeamB.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BankingAppTeamB.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RateAlertsPage : Page
    {   
        public RateAlertViewModel ViewModel => (RateAlertViewModel) DataContext;
        public RateAlertsPage()
        {
            InitializeComponent();
            this.DataContext = new RateAlertViewModel(ServiceLocator.ExchangeService, 0);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(ViewModel != null)
            {
                await ViewModel.LoadAsync();
            }
        }
    }
}
