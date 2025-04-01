using System.Windows.Media;

namespace SpaceBaseApplication
{
    /// <summary>
    /// If the value is not null, set visible. Otherwise, collapse.
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
    /// If the value is true, set visible. Otherwise, collapse. If "Invert" is passed as a parameter, reverse the visbility result.
    /// </summary>
    public class BoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string parameterString && string.Equals(parameterString, "Invert", StringComparison.OrdinalIgnoreCase))
                return (bool)value ? Visibility.Collapsed : Visibility.Visible;

            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// If the value is equal to the integer parameter, collapse. Otherwise, set visible.
    /// </summary>
    public class CollapsedIfEqualIntegerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!int.TryParse(value?.ToString(), out int intValue) || !int.TryParse(parameter?.ToString(), out int intParameter))
                return Visibility.Collapsed;

            return intValue == intParameter ? Visibility.Collapsed : Visibility.Visible;
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

    /// <summary>
    /// Gets the appropriate background color for the card effect.
    /// </summary>
    public class ActionTypeBackgroundConverter : IValueConverter
    {
        private static readonly SolidColorBrush CreditsBrush = Brushes.Yellow;
        private static readonly SolidColorBrush IncomeBrush = Brushes.LightGreen;
        private static readonly SolidColorBrush VictoryPointsBrush = Brushes.DodgerBlue;
        private static readonly SolidColorBrush ArrowBrush = Brushes.LightGray;
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
            if (value is null)
            {
                // Colony card
                return VictoryPointsBrush;
            }

            if (value is not Action<Player, Card, int, int> action)
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

            if (action == CardActions.AddRewardFromLeftOrRightSector)
                return ArrowBrush;

            if (action == CardActions.AddCreditsRewardFromAdjacentSector)
                return parameterString == "1" ? CreditsBrush : ArrowBrush;

            if (action == CardActions.AddVictoryPointsRewardFromAdjacentSector)
                return parameterString == "1" ? VictoryPointsBrush : ArrowBrush;

            return InvalidBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// If the cost is equal to the value of a designated Null card, then set visible. Otherwise, set collapsed.
    /// </summary>
    public class NullCardVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not int intValue)
                return Visibility.Visible;

            if (!string.Equals(parameter?.ToString(), "Invert"))
                return intValue == Constants.NullCardCost ? Visibility.Collapsed : Visibility.Visible;
            else
                return intValue == Constants.NullCardCost ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Gets a value of the form "Sector n" where n is the sector ID.
    /// </summary>
    public class SectorTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
                return $"Sector {stringValue}";

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Gets a value of the form "Player n" where n is the player ID.
    /// </summary>
    public class IDToPlayerTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
                return $"Player {intValue}";

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Gets a value of the form "Round n" where n is the round number.
    /// </summary>
    public class RoundNumberTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
                return $"Round {intValue}";

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Gets a value of the form "Active: Player n" where n is the player ID.
    /// </summary>
    public class IDToPlayerTurnTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
                return $"Active: Player {intValue}";

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Gets a value of the form "Dice a: b" where a is the dice ID and b is the rolled value of the dice.
    /// </summary>
    public class DiceToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not int intValue || parameter is not string stringParameter)
                return string.Empty;

            return $"Dice {stringParameter}: {intValue}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Set visible if the card type matches the provided type. Otherwise, set collapsed.
    /// </summary>
    public class CardTypeVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not CardType cardType || !int.TryParse(parameter?.ToString(), out int paramInteger))
                return Visibility.Collapsed;

            CardType parameterType = (CardType)paramInteger;

            return cardType == parameterType ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class WinningPlayerIDsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ObservableCollection<int> winningPlayerIDs || winningPlayerIDs.Count == 0)
                return string.Empty;

            if (winningPlayerIDs.Count == 1)
            {
                return $"Player {winningPlayerIDs[0]}";
            }
            else
            {
                return $"Player {string.Join(", ", winningPlayerIDs.Select(id => id.ToString()))}";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
