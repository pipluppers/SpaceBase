using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBase
{
    public class SectorViewModel(Sector sector) : ViewModelBase
    {
        public Sector Sector { get => sector; }

        public double Left { get; set; }
        public double Right { get; set; }
    }
}
