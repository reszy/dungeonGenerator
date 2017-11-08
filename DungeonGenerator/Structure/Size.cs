using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerator.Structure
{
    public class Size
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public Size() { }

        public Size(int width, int height)
        {
            Height = height;
            Width = width;
        }
        
        public override string ToString()
        {
            return "Width: " + Width + ", Height: " + Height;
        }
    }
}
