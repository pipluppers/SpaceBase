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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpaceBase
{
    /// <summary>
    /// Interaction logic for CardControl.xaml
    /// </summary>
    public partial class CardControl : UserControl
    {
        public CardControl()
        {
            InitializeComponent();
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
            return;
            //if (sender is not Rectangle sectorRectangle || sectorRectangle.DataContext is not Card card || e.Source is not Rectangle cardRectangle)
            //    return;

            //Canvas? canvas = Utilities.FindAncestor<Canvas>(sectorRectangle);
            //if (canvas == null || !string.Equals(canvas.Name, Constants.MainCanvasName))
            //    return;

            //Debug.WriteLine("Dropped");

            //string? data = e.Data.GetData(DataFormats.Text)?.ToString();
            //Debug.Assert(data != null);

            //Rectangle rectangle = DeserializeCard(data);

            //int sectorID = card.SectorID;
            //double leftValue = 20 + ((sectorID - 1) * 80) + (2 * (sectorID - 1));

            //rectangle.SetValue(Canvas.LeftProperty, leftValue);
            //rectangle.SetValue(Canvas.TopProperty, 690.0);

            //canvas.Children.Add(rectangle);

            //Debug.WriteLine($"Data: {data}");

            //e.Handled = true;
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

        private void Border_Drop(object sender, DragEventArgs e)
        {
            return;
            if (sender is not Border border || border.DataContext is not Card card || e.Source is not Border cardBorder)
                return;

            string data = (string)e.Data.GetData(DataFormats.Text);

            var deser = Card.DeserializeCard(data);

            Canvas? canvas = Utilities.FindAncestor<Canvas>(border);
            if (canvas == null || !string.Equals(canvas.Name, Constants.MainCanvasName))
                return;

            Debug.WriteLine("Dropped");

            string? data2 = e.Data.GetData(DataFormats.Text)?.ToString();
            Debug.Assert(data != null);

            Rectangle rectangle = DeserializeCard(data);

            int sectorID = card.SectorID;
            double leftValue = 20 + ((sectorID - 1) * 80) + (2 * (sectorID - 1));

            rectangle.SetValue(Canvas.LeftProperty, leftValue);
            rectangle.SetValue(Canvas.TopProperty, 690.0);

            canvas.Children.Add(rectangle);

            Debug.WriteLine($"Data: {data}");

            e.Handled = true;
        }
    }
}
