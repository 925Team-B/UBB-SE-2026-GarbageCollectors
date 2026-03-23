using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace BankingAppTeamB.Converters
{
    public class StepToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int currentStep && parameter is string param && int.TryParse(param, out int stepNumber))
                return currentStep == stepNumber ? Visibility.Visible : Visibility.Collapsed;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}