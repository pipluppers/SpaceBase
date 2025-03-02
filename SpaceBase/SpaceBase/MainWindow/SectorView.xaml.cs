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
    /// Interaction logic for SectorView.xaml
    /// </summary>
    public partial class SectorView : UserControl
    {
        public SectorView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Adds the dragged card to the sector at the dropped location.
        /// </summary>
        /// <param name="sender">The dropped location.</param>
        /// <param name="e">The arguments with the dragged card.</param>
        private void Border_Drop(object sender, DragEventArgs e)
        {
            if (sender is not Border border || e.Source is not CardControl)
                return;

            if (border.DataContext is not Sector sector)
                return;

            string serializedString = (string)e.Data.GetData(DataFormats.Text);

            Card? card = JsonSerializer.Deserialize<Card>(serializedString);

            if (card == null)
                return;

            if (card.SectorID != sector.ID)
                return;

            Grid? grid = Utilities.FindAncestor<Grid>(border, 2);
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
