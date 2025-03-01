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

            Title = Constants.GameTitle;
            Background = new ImageBrush(new BitmapImage(new Uri(Constants.BackgroundSplashScreenPath)));
            Icon = new BitmapImage(new Uri(Constants.IconPath));
        }
    }
}
