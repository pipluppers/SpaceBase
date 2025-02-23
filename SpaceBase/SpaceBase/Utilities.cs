using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SpaceBase
{
    interface ISerializable
    {
        string Serialize();
        object? Deserialize(string str);
    }

    internal static class Utilities
    {
        public static T? FindAncestor<T>(DependencyObject current) where T : class
        {
            while (current != null)
            {
                if (current is T ancestor)
                {
                    return ancestor;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        public static string Serialize(ISerializable serializableObject)
        {
            return serializableObject.Serialize();
        }
    }

    /// <summary>
    /// If the value is null, collapse. Otherwise, set visible.
    /// </summary>
    public class NullVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CollapsedIfEqualIntegerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!int.TryParse(value?.ToString(), out int intValue) || !int.TryParse(parameter?.ToString(), out int intParameter))
                return Visibility.Collapsed;

            return intValue == intParameter ? Visibility.Collapsed: Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// If the cost is 0, collapse. Otherwise, set visible.
    /// </summary>
    public class CostVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not int cost)
                return Visibility.Collapsed;

            return cost > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ActionTypeBackgroundConverter : IValueConverter
    {
        private static readonly SolidColorBrush CreditsBrush = Brushes.Yellow;
        private static readonly SolidColorBrush IncomeBrush = Brushes.LightGreen;
        private static readonly SolidColorBrush VictoryPointsBrush = Brushes.DodgerBlue;
        private static readonly SolidColorBrush InvalidBrush = Brushes.Red;


        /// <summary>
        /// Returns the appropriate color given the action defined by <paramref name="value"/> and the <paramref name="parameter"/> describing whether it is a primary or secondary effect.
        /// </summary>
        /// <param name="value">The action.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter">"1" if primary effect or "2" if secondary effect.</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Action<Player, int, int> action)
                return InvalidBrush;

            string parameterString = parameter?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(parameterString) || (parameterString != "1" && parameterString != "2"))
                return InvalidBrush;

            if (action == CardActions.AddCredits)
                return CreditsBrush;

            if (action == CardActions.AddIncome)
                return IncomeBrush;

            if (action == CardActions.AddVictoryPoints)
                return VictoryPointsBrush;

            if (action == CardActions.AddCreditsIncome)
                return parameterString == "1" ? CreditsBrush : IncomeBrush;

            if (action == CardActions.AddCreditsVictoryPoints)
                return parameterString == "1" ? CreditsBrush : VictoryPointsBrush;

            return InvalidBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
