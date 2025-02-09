using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBase
{
    public class MainWindowViewModel : ViewModelBase
    {
        private HumanPlayer _player;

        public MainWindowViewModel()
        {
            Game = new Game();
            _player = (HumanPlayer)Game.Players.First(p => p is HumanPlayer);
        }

        public Game Game { get; }

        public HumanPlayer Player { get => _player; private set => SetProperty(ref _player, value); }
    }
}
