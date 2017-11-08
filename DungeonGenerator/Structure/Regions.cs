using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DungeonGenerator.Structure
{
    public class Regions
    {
        List<int> regions;
        int regionCounter = 0;

        static private Regions instance;

        List<Color> color;

        private Regions()
        {
            regions = new List<int>();
            color = new List<Color>()
            {
                Colors.Red,
                Colors.Violet,
                Colors.Yellow,
                Colors.PaleGreen,
                Colors.Tan,
                Colors.DimGray,
                Colors.SandyBrown,
                Colors.RoyalBlue,
                Colors.Purple,
                Colors.PeachPuff,
                Colors.OliveDrab,
                Colors.MintCream,
                Colors.MediumSpringGreen,
                Colors.Thistle,
                Colors.Indigo,
                Colors.Magenta
            };
        }

        static public Regions Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Regions();
                }
                return instance;
            }
        }

        static public void Reset()
        {
            instance = new Regions();
        }

        public int GetNewRegion()
        {
            regionCounter++;
            regions.Add(regionCounter);
            return regionCounter;
        }

        public Color GetColor(int i)
        {
            return color[i % color.Count];
        }
    }
}
