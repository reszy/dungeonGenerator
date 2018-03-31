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
        int regionCounter = 0;

        static private Regions instance;

        List<Color> color;

        private Regions()
        {
            color = new List<Color>()
            {
                Color.FromRgb(17,141,240),
                Color.FromRgb(251,255,163),
                Color.FromRgb(255,75,104),
                Color.FromRgb(97,187,182),
                Color.FromRgb(218,92,83),
                Color.FromRgb(239,221,178),
                Color.FromRgb(171,58,76),
                Color.FromRgb(97,82,159),
                Color.FromRgb(251,186,64),
                Color.FromRgb(168,228,177),
                Color.FromRgb(218,218,218),
                Color.FromRgb(131,115,87),
                Color.FromRgb(171,206,206),
                Color.FromRgb(103,140,64),
                Color.FromRgb(239,97,207),
                Color.FromRgb(251,209,36),
                Color.FromRgb(29,209,36)
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
            return ++regionCounter;
        }

        public Color GetColor(int i)
        {
            return color[i % color.Count];
        }

        public void Squash(int squashTo)
        {
            regionCounter = squashTo;
        }
    }
}
