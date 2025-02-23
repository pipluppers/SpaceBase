using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SpaceBase
{
    /// <summary>
    /// Interaction logic for PlayWindow.xaml
    /// </summary>
    public partial class PlayWindow : Window
    {
        public PlayWindow()
        {
            InitializeComponent();

            string backgroundPath = @"C:\Users\nguye\Pictures\Space_Base.png";
            string iconPath = @"C:\Users\nguye\Pictures\Star.png";

            Title = "Space Base";
            Background = new ImageBrush(new BitmapImage(new Uri(backgroundPath)));
            Icon = new BitmapImage(new Uri(iconPath));
        }
    }
}
