using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SpaceBase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int Left1 = 21;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private bool _isDragging = false;
        private Point _dragStartPoint;

        private void CardControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not CardControl cardControl)
                return;

            _isDragging = true;
            _dragStartPoint = e.GetPosition(cardControl);
            //rectangle.CaptureMouse();
            e.Handled = true;
        }

        private void CardControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || sender is not CardControl cardControl || cardControl.DataContext is not Card card || e.LeftButton != MouseButtonState.Pressed)
                return;

            //Canvas? canvas = Utilities.FindAncestor<Canvas>(cardControl);
            //if (canvas == null)
            //    return;

            //Point currentPosition = e.GetPosition(canvas); // Relative to the Canvas

            //double left = currentPosition.X - _dragStartPoint.X;
            //double top = currentPosition.Y - _dragStartPoint.Y;

            //// Keep rectangle within the bounds of the parent Canvas.
            //left = Math.Max(0, Math.Min(canvas.ActualWidth - cardControl.ActualWidth, left));
            //top = Math.Max(0, Math.Min(canvas.ActualHeight - cardControl.ActualHeight, top));

            //Canvas.SetLeft(cardControl, left);
            //Canvas.SetTop(cardControl, top);

            //e.Handled = true;
            DragDrop.DoDragDrop(cardControl, Utilities.Serialize(card), DragDropEffects.Move);
        }

        /// <summary>
        /// Adds the dragged card to the sector at the dropped location.
        /// </summary>
        /// <param name="sender">The dropped location.</param>
        /// <param name="e">The arguments with the dragged card.</param>
        private void Border_Drop(object sender, DragEventArgs e)
        {
            if (sender is not Border border || border.DataContext is not Sector sector || e.Source is not CardControl)
                return;

            string serializedString = (string)e.Data.GetData(DataFormats.Text);

            Card? card = JsonSerializer.Deserialize<Card>(serializedString);

            if (card == null)
                return;

            Grid? grid = Utilities.FindAncestor<Grid>(border);
            if (grid == null)
                return;

            if (grid.DataContext is not Player player)
                return;

            try
            {
                player.AddCard(card);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"STRAUGHT TO JAIL. {ex.Message}");
            }
        }
    }
}
