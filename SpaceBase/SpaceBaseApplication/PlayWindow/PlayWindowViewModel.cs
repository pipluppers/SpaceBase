using SpaceBaseApplication.MainWindow;
using System.IO;
using System.Text;

namespace SpaceBaseApplication.PlayWindow
{
    public class PlayWindowViewModel : ViewModelBase
    {
        private readonly PlayWindow _playwindow;
        private readonly string _configDirectory;
        private readonly string _configPath;
        private readonly MainWindowViewModel _mainWindowViewModel;
        private bool _showOptions;
        private int _acceptedVictoryThreshold;
        private string _victoryThreshold;

        private readonly RelayCommand _playGameCommand;
        private readonly RelayCommand _quitGameCommand;
        private readonly RelayCommand _showOptionsCommand;
        private readonly RelayCommand _applyChangesCommand;
        private readonly RelayCommand _cancelChangesCommand;

        public PlayWindowViewModel()
        {
            _playwindow = new PlayWindow() { DataContext = this };
            _playwindow.InitializeEvent += Playwindow_InitializeEventHandler;
            _configDirectory = Environment.ExpandEnvironmentVariables(Constants.ConfigurationDirectory);
            _configPath = $"{_configDirectory}\\{Constants.ConfigurationFile}";

            _mainWindowViewModel = new MainWindowViewModel();
            _showOptions = false;
            _acceptedVictoryThreshold = 40;
            _victoryThreshold = _acceptedVictoryThreshold.ToString();

            _playGameCommand = new(PlayGame);
            _quitGameCommand = new(QuitGame);
            _showOptionsCommand = new(ShowOptionsDisplay);
            _applyChangesCommand = new(ApplyChanges, () => CanApplyChanges);
            _cancelChangesCommand = new(CancelChanges);
        }

        #region Properties

        /// <summary>
        /// If true, show the options display. Otherwise, hide it.
        /// </summary>
        public bool ShowOptions { get => _showOptions; set => SetProperty(ref _showOptions, value); }

        /// <summary>
        /// The amount of points in the textbox. The Apply button must be clicked to persist this value.
        /// </summary>
        public string VictoryThreshold { get => _victoryThreshold; set => SetProperty(ref _victoryThreshold, value); }

        /// <summary>
        /// True if there is a non-zero victory threshold integer.
        /// </summary>
        public bool CanApplyChanges { get => !string.IsNullOrEmpty(_victoryThreshold) && int.TryParse(_victoryThreshold, out int victoryThreshold) && victoryThreshold > 0; }

        #endregion Properties

        /// <summary>
        /// Shows the PlayWindow.
        /// </summary>
        public void Show()
        {
            Debug.Assert(_playwindow != null);
            _playwindow.Show();
        }

        /// <summary>
        /// Clean up resources.
        /// </summary>
        public void Close()
        {
            _mainWindowViewModel.Close();
        }

        #region Commands

        public ICommand PlayGameCommand { get => _playGameCommand; }

        /// <summary>
        /// Launches the Game UI and closes the PlayWindow.
        /// </summary>
        private async void PlayGame()
        {
            _playwindow.CloseAndShowMainWindow();

            _mainWindowViewModel.UpdateVictoryThreshold(_acceptedVictoryThreshold);
            _mainWindowViewModel.Show();
            await _mainWindowViewModel.StartGame();
        }

        public ICommand QuitGameCommand { get => _quitGameCommand; }

        /// <summary>
        /// Closes the application.
        /// </summary>
        private void QuitGame()
        {
            Application.Current.Shutdown();
        }

        public ICommand ShowOptionsCommand { get => _showOptionsCommand; }

        /// <summary>
        /// Shows the options display where players can customize the game.
        /// </summary>
        private void ShowOptionsDisplay()
        {
            VictoryThreshold = _acceptedVictoryThreshold.ToString();
            ShowOptions = true;
        }

        public ICommand ApplyChangesCommand { get => _applyChangesCommand; }

        /// <summary>
        /// Save any customizations made by the user and close the options display.
        /// </summary>
        private void ApplyChanges()
        {
            if (!int.TryParse(VictoryThreshold, out int victoryThreshold))
                return;

            _acceptedVictoryThreshold = victoryThreshold;

            WriteToConfigurationFile(victoryThreshold);

            ShowOptions = false;
        }

        public ICommand CancelChangesCommand { get => _cancelChangesCommand; }

        /// <summary>
        /// Ignore any customizations made by the user and close the options display.
        /// </summary>
        private void CancelChanges()
        {
            ShowOptions = false;
        }

        #endregion Commands

        #region Event handlers

        /// <summary>
        /// Import any saved configurations from the game configuration file.
        /// </summary>
        private void Playwindow_InitializeEventHandler(object _, EventArgs __)
        {
            try
            {
                if (!File.Exists(_configPath))
                    return;

                using StreamReader reader = new(_configPath);
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    ConfigData? data = JsonSerializer.Deserialize<ConfigData>(line);
                    if (data == null)
                        continue;

                    _acceptedVictoryThreshold = data.VictoryThreshold;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error opening file: {ex.Message}");
            }
        }

        /// <summary>
        /// Save any configurations to the game configuration file.
        /// </summary>
        /// <remarks>
        /// If the directory or configuration file does not exist, create them.
        /// </remarks>
        /// <param name="victoryThreshold">The data to write to the file.</param>
        private void WriteToConfigurationFile(int victoryThreshold)
        {
            FileStream? fileStream = null;
            try
            {
                if (!Directory.Exists(_configDirectory))
                    Directory.CreateDirectory(_configDirectory);

                if (!File.Exists(_configPath))
                    fileStream = File.Create(_configPath);
                else
                    fileStream = File.OpenWrite(_configPath);

                ConfigData data = new() { VictoryThreshold = victoryThreshold };
                string dataJson = JsonSerializer.Serialize(data);

                Byte[] text = new UTF8Encoding(true).GetBytes(dataJson);

                fileStream.Write(text, 0, text.Length);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Failed to write to config file: {ex.Message}");
            }
            finally
            {
                fileStream?.Dispose();
            }
        }

        #endregion Event handlers

    }

    internal class ConfigData
    {
        public int VictoryThreshold { get; set; }
    }
}
