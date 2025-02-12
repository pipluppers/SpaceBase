using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private bool _isDragging = false;
        private Point _dragStartPoint;
        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Rectangle rectangle)
                return;

            _isDragging = true;
            _dragStartPoint = e.GetPosition(rectangle);
            //rectangle.CaptureMouse();
            e.Handled = true;
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (/*_isDragging && */sender is Rectangle rectangle && e.LeftButton == MouseButtonState.Pressed)
            {
                Canvas? canvas = FindAncestor<Canvas>(rectangle);
                if (canvas == null)
                    return;

                Point currentPosition = e.GetPosition(canvas); // Relative to the Canvas

                double left = currentPosition.X - _dragStartPoint.X;
                double top = currentPosition.Y - _dragStartPoint.Y;

                // Keep rectangle within the bounds of the parent Canvas.
                left = Math.Max(0, Math.Min(canvas.ActualWidth - rectangle.ActualWidth, left));
                top = Math.Max(0, Math.Min(canvas.ActualHeight - rectangle.ActualHeight, top));

                Canvas.SetLeft(rectangle, left);
                Canvas.SetTop(rectangle, top);

                e.Handled = true;
                DragDrop.DoDragDrop(rectangle, SerializeCard(rectangle), DragDropEffects.Move);
            }
        }

        private void SectorRectangle_DragOver(object sender, DragEventArgs e)
        {
            if (sender is not Rectangle sectorRectangle)
                return;

            if (e.Source is not Rectangle cardRectangle)
                return;
        }

        private void SectorRectangle_Drop(object sender, DragEventArgs e)
        {
            if (sender is not Rectangle sectorRectangle || sectorRectangle.DataContext is not Sector sector || e.Source is not Rectangle cardRectangle)
                return;

            Canvas? canvas = FindAncestor<Canvas>(sectorRectangle);
            if (canvas == null || !string.Equals(canvas.Name, Constants.MainCanvasName))
                return;

            Debug.WriteLine("Dropped");

            string? data = e.Data.GetData(DataFormats.Text)?.ToString();
            Debug.Assert(data != null);

            Rectangle rectangle = DeserializeCard(data);

            int sectorID = sector.ID;
            double leftValue = 20 + ((sectorID - 1) * 80) + (2 * (sectorID-1));

            rectangle.SetValue(Canvas.LeftProperty, leftValue);
            rectangle.SetValue(Canvas.TopProperty, 690.0);

            canvas.Children.Add(rectangle);

            Debug.WriteLine($"Data: {data}");

            e.Handled = true;
        }

        #region Serialization

        private string SerializeCard(Rectangle rectangle)
        {
            // Should be a card
            var dataContext = rectangle.DataContext;

            return "TODO CARD";
        }

        private Rectangle DeserializeCard(string cardJson)
        {
            Rectangle rectangle = new Rectangle()
            {
                Stroke = Brushes.Black,
                Fill = Brushes.Blue,
                Height = 200,
                Width = 60,
            };

            rectangle.SetValue(Panel.ZIndexProperty, 2);

            return rectangle;
        }

        #endregion Serialization
    }
}
