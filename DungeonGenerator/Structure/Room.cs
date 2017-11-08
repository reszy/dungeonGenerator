using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerator.Structure
{
    public class Room
    {
        public int RegionId { get; set; }
        public Position Position { get; set; }
        public Size Size { get; set; }


        public Room(Position position, Size size, int region)
        {
            RegionId = region;
            this.Position = position;
            this.Size = size;
        }

        public bool IsColliding(Room otherRoom)
        {
            if(otherRoom.Position.X > Position.X + Size.Width )
            {
                return false;
            }
            if (otherRoom.Position.X + otherRoom.Size.Width < Position.X)
            {
                return false;
            }

            if (otherRoom.Position.Y > Position.Y + Size.Height)
            {
                return false;
            }
            if (otherRoom.Position.Y + otherRoom.Size.Height < Position.Y)
            {
                return false;
            }
            return true;
        }
    }
}
