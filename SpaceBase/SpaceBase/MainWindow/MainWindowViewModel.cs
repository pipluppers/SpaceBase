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

            Game.StartGame();

            Sector1 = new SectorViewModel(_player.Board.Sectors[0]);
            Sector2 = new SectorViewModel(_player.Board.Sectors[1]);
            Sector3 = new SectorViewModel(_player.Board.Sectors[2]);
            Sector4 = new SectorViewModel(_player.Board.Sectors[3]);
            Sector5 = new SectorViewModel(_player.Board.Sectors[4]);
            Sector6 = new SectorViewModel(_player.Board.Sectors[5]);
            Sector7 = new SectorViewModel(_player.Board.Sectors[6]);
            Sector8 = new SectorViewModel(_player.Board.Sectors[7]);
            Sector9 = new SectorViewModel(_player.Board.Sectors[8]);
            Sector10 = new SectorViewModel(_player.Board.Sectors[9]);
            Sector11 = new SectorViewModel(_player.Board.Sectors[10]);
            Sector12 = new SectorViewModel(_player.Board.Sectors[11]);
        }

        public Game Game { get; }

        public HumanPlayer Player { get => _player; private set => SetProperty(ref _player, value); }

        public SectorViewModel Sector1 { get; set; }
        public SectorViewModel Sector2 { get; set; }
        public SectorViewModel Sector3 { get; set; }
        public SectorViewModel Sector4 { get; set; }
        public SectorViewModel Sector5 { get; set; }
        public SectorViewModel Sector6 { get; set; }
        public SectorViewModel Sector7 { get; set; }
        public SectorViewModel Sector8 { get; set; }
        public SectorViewModel Sector9 { get; set; }
        public SectorViewModel Sector10 { get; set; }
        public SectorViewModel Sector11 { get; set; }
        public SectorViewModel Sector12 { get; set; }
    }
}
