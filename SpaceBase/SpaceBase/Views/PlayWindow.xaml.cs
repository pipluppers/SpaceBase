using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SpaceBase
{
    /// <summary>
    /// Interaction logic for PlayWindow.xaml
    /// </summary>
    public partial class PlayWindow : Window
    {
        private bool _disposeMainWindow = true;

        public PlayWindow()
        {
            InitializeComponent();

            Title = Constants.GameTitle;
            Background = new ImageBrush(new BitmapImage(new Uri(Constants.BackgroundSplashScreenPath)));
            Icon = new BitmapImage(new Uri(Constants.IconPath));
        }

        private void PlayWindow_Closed(object sender, EventArgs e)
        {
            if (!_disposeMainWindow)
                return;

            if (sender is not PlayWindow playWindow || playWindow.DataContext is not PlayWindowViewModel viewModel)
                return;

            viewModel.Close();
        }

        public void CloseAndShowMainWindow()
        {
            _disposeMainWindow = false;
            Close();
        }
    }
}
