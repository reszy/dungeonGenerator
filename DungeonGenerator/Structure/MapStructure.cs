using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerator.Structure
{
    public class MapStructure
    {
        public Tile[,] Map { get; set; }
        public Size Size { get; set; }

        private readonly List<Room> rooms = new List<Room>();
        public List<Room> Rooms { get => rooms; }

        public int AddRoom(Room room)
        {
            rooms.Add(room);
            return rooms.Count - 1;
        }

        public Room GetRoom(int id)
        {
            return rooms[id];
        }

        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Size.Width && y >= 0 && y < Size.Height;
        }
    }
}
