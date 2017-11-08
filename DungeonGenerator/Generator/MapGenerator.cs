using System;
using DungeonGenerator.Structure;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace DungeonGenerator.Generator
{
    public class MapGenerator
    {
        private MapStructure map = new MapStructure();
        private int actualStep = 0;
        private int roomCount = 0;
        private bool isDone = false;
        public bool ByStep { get; set; }
        public bool IsDone { get => isDone; }

        private readonly RoomsGenerator roomsGenerator;
        private readonly MazeGenerator mazeGenerator;

        public void SetRoomCount(int roomCount)
        {
            this.roomCount = roomCount;
        }

        public MapGenerator(Size size)
        {
            map.Size = size;
            map.Map = new Tile[map.Size.Height, map.Size.Width];
            roomsGenerator = new RoomsGenerator(ref map);
            mazeGenerator = new MazeGenerator(ref map);
        }

        public void GenerateMap(int roomCount)
        {
            while (isDone == false)
            {
                DoStep();
            }
        }

        public void DoStep()
        {
            if(!isDone)
                actualStep++;

            switch (actualStep)
            {
                case 1:
                    roomsGenerator.GenerateRooms(roomCount);
                    break;
                case 2:
                    mazeGenerator.GenerateMaze();
                    break;
                case 3:
                    roomsGenerator.GenerateConnectors();
                    break;
                case 4:
                    roomsGenerator.CreateDoors();
                    break;
                case 5:
                    mazeGenerator.EliminateDeadEnds();
                    break;
                case 6:
                    ClearGarbage();
                    isDone = true;
                    break;
            }
        }

        void ClearGarbage()
        {
            for(int x = 0; x < map.Size.Width; x++)
            {
                for (int y = 0; y < map.Size.Height; y++)
                {
                    Tile tile = map.Map[x, y];
                    if (tile == null || tile.Type == TileType.DESTRUCTED)
                    {
                        map.Map[x, y] = new Tile(x, y, TileType.WALL)
                        {
                            RegionId = -1
                        };
                    }
                }
            }
        }

        public Tile getTile(int x, int y)
        {
            try
            {
                return map.Map[x, y];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void DrawMap(Canvas canvas, bool drawRegions)
        {
            for (int i = 0; i < map.Size.Width; i++)
            {
                for (int j = 0; j < map.Size.Height; j++)
                {
                    Tile tile = map.Map[i, j];
                    if (tile != null)
                    {
                        tile.Stroke = new SolidColorBrush(Colors.Black);
                        tile.StrokeThickness = 1;
                        tile.SetColor(drawRegions);
                        Canvas.SetLeft(tile, tile.X * tile.Width);
                        Canvas.SetTop(tile, tile.Y * tile.Height);
                        canvas.Children.Add(tile);
                    }
                }
            }
        }
    }
}
