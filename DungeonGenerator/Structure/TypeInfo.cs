using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DungeonGenerator.Structure
{
    public class TypeInfo
    {
        static private Dictionary<TileType, TypeInfo> info;
        static public Dictionary<TileType, TypeInfo> Info
        {
            get
            {
                if(info == null)
                {
                    info = new Dictionary<TileType, TypeInfo>
                    {
                        { TileType.FLOOR, new TypeInfo(Colors.LightGray) },
                        { TileType.DOOR, new TypeInfo(Colors.Brown) },
                        { TileType.WALL, new TypeInfo(Colors.DimGray) },
                        { TileType.STAIRS, new TypeInfo(Colors.Purple) },
                        { TileType.CONNECTOR, new TypeInfo(Colors.Orange) },
                        { TileType.DESTRUCTED, new TypeInfo(Colors.IndianRed) },
                    };
                }
                return info;
            }
        }

        private readonly Color color;
        public Color Color { get => color; }

        public TypeInfo(Color color)
        {
            this.color = color;
        }
    }
}
