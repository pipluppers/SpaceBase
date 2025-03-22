using SpaceBaseApplication.PlayWindow;

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

            PlayWindowViewModel playWindowViewModel = new();
            playWindowViewModel.Show();
        }
    }
}
