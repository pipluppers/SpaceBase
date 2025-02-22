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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Action<Player, int, int> action)
                return Brushes.Red;

            if (action == CardActions.AddCredits)
                return Brushes.Yellow;

            if (action == CardActions.AddIncome)
                return Brushes.LightGreen;

            if (action == CardActions.AddVictoryPoints)
                return Brushes.DodgerBlue;

            return Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
