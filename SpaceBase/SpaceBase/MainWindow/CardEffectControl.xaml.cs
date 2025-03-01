using System.Windows.Data;

namespace SpaceBase
{
    /// <summary>
    /// Interaction logic for CardEffectControl.xaml
    /// </summary>
    public partial class CardEffectControl : UserControl
    {
        public CardEffectControl()
        {
            InitializeComponent();

            if (!IsStationedEffect)
                SetBindings(this, "DeployedEffect", "DeployedAmount", "DeployedSecondaryAmount");
        }

        public static readonly DependencyProperty IsStationedEffectProperty =
            DependencyProperty.Register("IsStationedEffect", typeof(bool), typeof(CardEffectControl), new PropertyMetadata(Callback));

        public bool IsStationedEffect { get => (bool)GetValue(IsStationedEffectProperty); set => SetValue(IsStationedEffectProperty, value); }

        private static void Callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CardEffectControl cardEffectControl = (CardEffectControl)d;

            if ((bool)e.NewValue)
                SetBindings(cardEffectControl, "Effect", "Amount", "SecondaryAmount");
        }

        /// <summary>
        /// Sets the background bindings for each border and text amount for the text blocks.
        /// </summary>
        /// <param name="control">The CardEffectControl.</param>
        /// <param name="effectName">Either "Effect" or "DeployedEffect".</param>
        /// <param name="amountName">Either "Amount" or "DeployedAmount".</param>
        /// <param name="secondaryAmountName">Either "SecondaryAmount" or "DeployedSecondaryAmount".</param>
        private static void SetBindings(CardEffectControl control, string effectName, string amountName, string secondaryAmountName)
        {
            var primaryBorderBackgroundBinding = new Binding(effectName)
            {
                Converter = new ActionTypeBackgroundConverter(),
                ConverterParameter = 1,
                Mode = BindingMode.OneWay
            };
            var primaryTextBlockBinding = new Binding(amountName);

            BindingOperations.SetBinding(control.PrimaryBorder, Border.BackgroundProperty, primaryBorderBackgroundBinding);
            BindingOperations.SetBinding(control.PrimaryTextBlock, TextBlock.TextProperty, primaryTextBlockBinding);

            var secondaryBorderVisibilityBinding = new Binding(secondaryAmountName)
            {
                Converter = new CollapsedIfEqualIntegerConverter(),
                ConverterParameter = 0,
                Mode = BindingMode.OneWay
            };
            var secondaryBorderBackgroundBinding = new Binding(effectName)
            {
                Converter = new ActionTypeBackgroundConverter(),
                ConverterParameter = 2,
                Mode = BindingMode.OneWay
            };
            var secondaryTextBlockTextBinding = new Binding(secondaryAmountName);

            BindingOperations.SetBinding(control.SecondaryBorder, Border.VisibilityProperty, secondaryBorderVisibilityBinding);
            BindingOperations.SetBinding(control.SecondaryBorder, Border.BackgroundProperty, secondaryBorderBackgroundBinding);
            BindingOperations.SetBinding(control.SecondaryTextBlock, TextBlock.TextProperty, secondaryTextBlockTextBinding);
        }
    }
}
