using DungeonGenerator.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DungeonGenerator.Generator.MazeGenerator;

namespace DungeonGenerator.Generator
{
    class RoomsGenerator
    {
        readonly private Random rand = new Random();
        private MapStructure map;
        private List<Tile> doors = new List<Tile>();
        public RoomsGenerator(ref MapStructure map)
        {
            this.map = map;
        }

        public void GenerateRooms()
        {
            CreateRoom(new Position(1, 1), new Size(3, 3));
            CreateRoom(new Position(4, 4), new Size(3, 3), true);
            CreateRoom(new Position(6, 1), new Size(3, 3), true);
            CreateRoom(new Position(12, 2), new Size(3, 3));
            CreateRoom(new Position(10, 4), new Size(7, 4));

            MergeRegions();
        }

        private Position getRoomRandomCoords(Size size)
        {
            return new Position(
            rand.Next(0, (int)((map.Size.Width - size.Width) / 2)) * 2 + 1,
            rand.Next(0, (int)((map.Size.Height - size.Height) / 2)) * 2 + 1
            );
        }

        public void GenerateRooms(int rooms)
        {
            for (int i = 0; i < rooms; i++)
            {
                Size size = new Size()
                {
                    Width = rand.Next(1, 3) * 2 + 1,
                    Height = rand.Next(1, 3) * 2 + 1
                };
                CreateRoom(getRoomRandomCoords(size), size, false);
            }
            for (int i = 0; i < 2; i++)
            {
                Size size = new Size(3, 3);
                CreateRoom(getRoomRandomCoords(size), size, true);
            }
            MergeRegions();
        }

        private int PutOnMap(int x, int y, TileType type, int roomId)
        {
            if (map.IsInBounds(x, y))
            {
                if (map.Map[x, y] == null)
                {
                    map.Map[x, y] = new Tile(x, y, type)
                    {
                        RegionId = map.GetRoom(roomId).RegionId,
                        RegionParentId = roomId
                    };
                }
                else
                {
                    map.Map[x, y].Type = type;
                    int oldRegion = map.Map[x, y].RegionId;
                    map.Map[x, y].RegionId = map.GetRoom(roomId).RegionId;
                    map.Map[x, y].RegionParentId = roomId;
                    return oldRegion;
                }
                return map.Map[x, y].RegionId;
            }
            return -10;
        }

        private void ReplaceRegionOnMap(int x, int y, int region)
        {
            if (map.IsInBounds(x, y) && map.Map[x, y] != null)
            {
                map.Map[x, y].RegionId = region;
            }
        }

        private void PutOnMapRoomWall(int x, int y, TileType type, int roomId)
        {
            if (map.IsInBounds(x, y) && map.Map[x, y] == null)
            {
                PutOnMap(x, y, type, roomId);
                map.Map[x, y].RegionId = -1;
            }
        }

        public bool CheckIfConnectorPossible(Tile tile)
        {
            if (tile.Type == TileType.WALL)
            {
                if (tile.X > 0 && tile.X < map.Size.Width - 1)
                {
                    Tile neighbour1 = map.Map[tile.X + 1, tile.Y];
                    Tile neighbour2 = map.Map[tile.X - 1, tile.Y];
                    if (neighbour1 != null && neighbour2 != null)
                    {
                        if (neighbour1.RegionId > 0 && neighbour2.RegionId > 0
                            && neighbour1.RegionId != neighbour2.RegionId)
                            return true;
                    }
                }
                if (tile.Y > 0 && tile.Y < map.Size.Height - 1)
                {
                    Tile neighbour1 = map.Map[tile.X, tile.Y + 1];
                    Tile neighbour2 = map.Map[tile.X, tile.Y - 1];
                    if (neighbour1 != null && neighbour2 != null)
                    {
                        if (neighbour1.RegionId > 0 && neighbour2.RegionId > 0
                            && neighbour1.RegionId != neighbour2.RegionId)
                            return true;
                    }
                }
            }
            return false;
        }

        bool PutConnectorOnMap(int x, int y)
        {
            if (CheckIfConnectorPossible(map.Map[x, y]))
            {
                map.Map[x, y].Type = TileType.CONNECTOR;
                return true;
            }
            return false;
        }

        public void GenerateConnectors()
        {
            foreach (var room in map.Rooms)
            {
                List<Position> connectors = new List<Position>();
                for (int i = 0; i < room.Size.Width; i++)
                {
                    int x = i + room.Position.X;
                    int y = room.Position.Y - 1;
                    if (PutConnectorOnMap(x, y))
                        connectors.Add(new Position(x, y));
                    y = room.Position.Y + room.Size.Height;
                    if (PutConnectorOnMap(x, y))
                        connectors.Add(new Position(x, y));
                }

                for (int i = 0; i < room.Size.Height; i++)
                {
                    int x = room.Position.X - 1;
                    int y = i + room.Position.Y;
                    if (PutConnectorOnMap(x, y))
                        connectors.Add(new Position(x, y));
                    x = room.Position.X + room.Size.Width;
                    if (PutConnectorOnMap(x, y))
                        connectors.Add(new Position(x, y));
                }

                if (connectors.Count > 0)
                {
                    Position choosed = connectors[rand.Next(0, connectors.Count)];
                    connectors.Remove(choosed);
                    doors.Add(map.Map[choosed.X, choosed.Y]);
                }
            }
        }

        public void CreateDoors()
        {
            foreach (Tile tile in doors)
            {
                if (tile != null && tile.Type == TileType.CONNECTOR)
                {
                    tile.Type = TileType.DOOR;
                }
            }
            foreach (Tile tile in map.Map)
            {
                if(tile != null && tile.Type == TileType.CONNECTOR)
                {
                    tile.Type = TileType.WALL;
                }
            }
        }

        private void MergeRegions()
        {
            foreach (var room in map.Rooms)
            {
                var collidingRooms = map.Rooms.Where(anotherRoom => room != anotherRoom && room.IsColliding(anotherRoom) && room.RegionId != anotherRoom.RegionId).ToList();
                collidingRooms.ForEach(collidingRoom => ChangeRoomRegion(collidingRoom, room.RegionId));
            }
        }

        void ChangeRoomRegion(Room room, int region)
        {
            room.RegionId = region;
            for (int i = 0; i < room.Size.Width; i++)
            {
                for (int j = 0; j < room.Size.Height; j++)
                {
                    ReplaceRegionOnMap(i + room.Position.X, j + room.Position.Y, region);
                }
            }
        }

        private void CreateRoom(Position position, Size size, bool putStairs = false)
        {
            int region = Regions.Instance.GetNewRegion();
            var room = new Room(position, size, region);
            var roomId = map.AddRoom(room);

            int regionFound = region;

            for (int i = 0; i < room.Size.Width; i++)
            {
                for (int j = 0; j < room.Size.Height; j++)
                {
                    int tmpRegion = PutOnMap(i + room.Position.X, j + room.Position.Y, TileType.FLOOR, roomId);
                    if (tmpRegion < regionFound && tmpRegion >= 0)
                    {
                        regionFound = tmpRegion;
                    }
                }
            }

            for (int i = -1; i < room.Size.Width + 1; i++)
            {
                int x = i + room.Position.X;
                int y = room.Position.Y - 1;
                PutOnMapRoomWall(x, y, TileType.WALL, roomId);
                y = room.Position.Y + room.Size.Height;
                PutOnMapRoomWall(x, y, TileType.WALL, roomId);
            }

            for (int i = -1; i < room.Size.Height + 1; i++)
            {
                int x = room.Position.X - 1;
                int y = i + room.Position.Y;
                PutOnMapRoomWall(x, y, TileType.WALL, roomId);
                x = room.Position.X + room.Size.Width;
                PutOnMapRoomWall(x, y, TileType.WALL, roomId);
            }

            if (putStairs)
            {
                PutOnMap(room.Position.X + 1, room.Position.Y + 1, TileType.STAIRS, roomId);
            }
        }
    }
}
