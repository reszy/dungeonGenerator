using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonGenerator.Structure;
using System.Windows.Media;

namespace DungeonGenerator.Generator
{
    public class MazeGenerator
    {
        public enum Direction
        {
            UNKNOWN = 0,
            UP = 1,
            RIGHT = 2,
            DOWN = 3,
            LEFT = 4
        };

        readonly private MapStructure map;
        readonly private Random rand = new Random();
        
        int id = 0;

        public MazeGenerator(ref MapStructure map)
        {
            this.map = map;
        }

        public void GenerateMaze()
        {
            for (int x = 1; x < map.Size.Width; x += 2)
            {
                for (int y = 1; y < map.Size.Height; y += 2)
                {
                    if (map.Map[x, y] == null)
                    {
                        var newregion = Regions.Instance.GetNewRegion();
                        if (CarveCorridor(x, y, newregion))
                        {
                            DoMaze(x, y, Direction.UNKNOWN, newregion);
                        }
                    }
                }
            }
        }

        private Direction GetFreeNeighbour(int x, int y)
        {
            //TODO randomize output
            List<Direction> freeDirections = new List<Direction>();
            int freeY = y;
            int freeX = x + 2;
            if (IsValid(freeX, freeY))
            {
                freeDirections.Add(Direction.RIGHT);
            }
            freeX = x - 2;
            if (IsValid(freeX, freeY))
            {
                freeDirections.Add(Direction.LEFT);
            }

            freeX = x;
            freeY = y + 2;
            if (IsValid(freeX, freeY))
            {
                freeDirections.Add(Direction.DOWN);
            }
            freeY = y - 2;
            if (IsValid(freeX, freeY))
            {
                freeDirections.Add(Direction.UP);
            }
            if (freeDirections.Count > 0)
            {
                return freeDirections[rand.Next(0, freeDirections.Count - 1)];
            }
            else
            {
                return Direction.UNKNOWN;
            }
        }

        private bool IsValid(int x, int y)
        {
            return map.IsInBounds(x, y) && (map.Map[x, y] == null || map.Map[x, y].Type != TileType.FLOOR);
        }

        private bool CarveCorridor(int x, int y, int region = -1, int tileId = 0)
        {
            if (!map.IsInBounds(x, y) || map.Map[x, y] != null)
            {
                return false;
            }

            map.Map[x, y] = new Tile(x, y, TileType.FLOOR)
            {
                RegionId = region,
                Id = tileId,
                RegionType = RegionType.CORRIDOR
            };

            return true;
        }

        private bool CarveInDirection(Direction direction, int x, int y, ref int newX, ref int newY, int region)
        {
            newX = -1;
            newY = -1;
            switch (direction)
            {
                case Direction.RIGHT:
                    x++;
                    newX = x + 1;
                    break;
                case Direction.LEFT:
                    x--;
                    newX = x - 1;
                    break;
                case Direction.DOWN:
                    y++;
                    newY = y + 1;
                    break;
                default:
                    y--;
                    newY = y - 1;
                    break;
            }
            if (newX == -1)
            {
                newX = x;
            }
            else if (newY == -1)
            {
                newY = y;
            }
            if (!map.IsInBounds(newX, newY))
            {
                return false;
            }
            if (CarveCorridor(newX, newY, region, id + 2))
            {
                id += 2;
                return CarveCorridor(x, y, region, id - 1);
            }
            else
            {
                return false;
            }
        }

        public void EliminateDeadEnds()
        {
            foreach (Tile tile in map.Map)
            {
                if (tile != null && tile.RegionType == RegionType.CORRIDOR)
                {
                    EliminateDeadEnd(tile);
                }
            }
        }

        private void EliminateDeadEnd(Tile tile)
        {
            if (tile == null)
            {
                return;
            }
            int emptyNeighbour = 0;
            Tile lastNotEmptyNeighbour = null;
            if (!IsNeighbourImportantPath(tile.X, tile.Y - 1, ref lastNotEmptyNeighbour))
            {
                emptyNeighbour++;
            }
            if (!IsNeighbourImportantPath(tile.X, tile.Y + 1, ref lastNotEmptyNeighbour))
            {
                emptyNeighbour++;
            }
            if (!IsNeighbourImportantPath(tile.X - 1, tile.Y, ref lastNotEmptyNeighbour))
            {
                emptyNeighbour++;
            }
            if (!IsNeighbourImportantPath(tile.X + 1, tile.Y, ref lastNotEmptyNeighbour))
            {
                emptyNeighbour++;
            }

            if (emptyNeighbour > 2)
            {
                map.Map[tile.X, tile.Y].Type = TileType.DESTRUCTED;
                EliminateDeadEnd(lastNotEmptyNeighbour);
            }
        }

        bool IsNeighbourImportantPath(int x, int y, ref Tile lastNotEmptyNeighbour)
        {
            Tile neighbour;
            if (map.IsInBounds(x, y))
                neighbour = map.Map[x, y];
            else
                return false;

            if (neighbour == null)
            {
                return false;
            }
            else
            {
                if (neighbour.Type == TileType.WALL || neighbour.Type == TileType.DESTRUCTED)
                {
                    return false;
                }
                else
                {
                    if (neighbour.RegionType == RegionType.CORRIDOR)
                    {
                        lastNotEmptyNeighbour = neighbour;
                    }
                    return true;
                }

            }
        }

        private void DoMaze(int x, int y, Direction lastDirection = Direction.UNKNOWN, int region = -1)
        {
            Direction direction;
            if (lastDirection == Direction.UNKNOWN)
            {
                direction = (Direction)rand.Next(1, 4);
            }
            else
            {
                int random = rand.Next(1, 8);
                if (random > 4)
                {
                    direction = lastDirection;
                }
                else
                {
                    direction = (Direction)random;
                }
            }

            int newX = 0;
            int newY = 0;
            if (CarveInDirection(direction, x, y, ref newX, ref newY, region))
            {
                DoMaze(newX, newY, direction, region);
            }

            while ((direction = GetFreeNeighbour(x, y)) != Direction.UNKNOWN)
            {
                if (CarveInDirection(direction, x, y, ref newX, ref newY, region))
                {
                    DoMaze(newX, newY, direction, region);
                }
            }
        }

        internal void GenerateCorridor(Position position, int region, Direction direction)
        {
            var newregion = Regions.Instance.GetNewRegion();
            if (CarveCorridor(position.X, position.Y, Regions.Instance.GetNewRegion()))
            {
                DoMaze(position.X, position.Y, direction, newregion);
            }
        }
    }
}
