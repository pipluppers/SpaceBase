using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpaceBase
{
    public class MainWindowViewModel : ViewModelBase
    {
        private HumanPlayer _player;
        private bool _showDiceRollControl;
        private readonly MainWindow _mainWindow;
        private int _dice1;
        private int _dice2;

        public MainWindowViewModel()
        {
            Game = new Game();
            _player = (HumanPlayer)Game.Players.First(p => p is HumanPlayer);
            _showDiceRollControl = false;

            _startRoundCommand = new RelayCommand(StartRound, () => true);

            _mainWindow = new MainWindow() { DataContext = this };

            Game.WaitForPlayerInputEventHandler += Game_WaitForPlayerInputEventHandler;
            Game.DiceRollEvent += Game_DiceRollEvent;
        }

        /// <summary>
        /// Loop until the user interacts with the appropriate control based on the current state of the game.
        /// </summary>
        /// <remarks>This function spins on a worker thread.</remarks>
        /// <param name="sender">The game.</param>
        /// <param name="e">Unused arguments.</param>
        private void Game_WaitForPlayerInputEventHandler(object? sender, EventArgs e)
        {
            while (_waitForPlayerInput) { }
        }

        private void Game_DiceRollEvent(object sender, DiceRollEventArgs e)
        {
            Dice1 = e.Dice1;
            Dice2 = e.Dice2;
            ShowDiceRollControl = true;

            _waitForPlayerInput = true;
        }

        public Game Game { get; }

        public HumanPlayer Player { get => _player; private set => SetProperty(ref _player, value); }

        public bool ShowDiceRollControl { get => _showDiceRollControl; set => SetProperty(ref _showDiceRollControl, value); }

        public int Dice1 { get => _dice1; set => SetProperty(ref _dice1, value); }
        public int Dice2 { get => _dice2; set => SetProperty(ref _dice2, value); }

        public void Show()
        {
            _mainWindow.Show();
        }

        public async Task StartGame()
        {
            await Game.StartGame();
        }

        private bool _waitForPlayerInput = true;
        private readonly RelayCommand _startRoundCommand;
        public ICommand StartRoundCommand { get => _startRoundCommand; }
        private void StartRound()
        {
            _waitForPlayerInput = false;
        }
    }
}
