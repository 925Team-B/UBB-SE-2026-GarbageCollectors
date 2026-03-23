
using Microsoft.UI.Xaml.Controls;

using BankingAppTeamB.Configuration;
using BankingAppTeamB.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BankingAppTeamB.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FXPage : Page
    {
        public FXPage()
        {
            InitializeComponent();
            
            DataContext = new FXViewModel(ServiceLocator.ExchangeService);
        }
    }
}
