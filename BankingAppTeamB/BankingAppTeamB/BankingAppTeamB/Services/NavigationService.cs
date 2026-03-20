using Microsoft.UI.Xaml.Controls;

namespace BankingAppTeamB.Services
{
    public static class NavigationService
    {
        public static Frame? Frame { get; set; }

        public static void NavigateTo<T>(object? parameter = null)
        {
            Frame?.Navigate(typeof(T), parameter);
        }

        public static void GoBack()
        {
            if (Frame != null && Frame.CanGoBack)
                Frame.GoBack();
        }
    }
}