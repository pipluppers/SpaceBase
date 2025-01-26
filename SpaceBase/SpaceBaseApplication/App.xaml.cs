using SpaceBase;
using System.Windows;

namespace SpaceBaseApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            PlayWindow playWindow = new()
            {
                DataContext = new PlayWindowViewModel()
            };

            playWindow.Show();
        }
    }
}
