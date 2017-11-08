using System.Windows.Media;
using System.Windows.Shapes;

namespace DungeonGenerator.Structure
{
    public class Tile: Shape
    {
        public TileType Type { get; set; }
        public int RegionId { get; set; }
        public RegionType RegionType { get; set; }
        public int RegionParentId { get; set; }
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        protected override Geometry DefiningGeometry => new RectangleGeometry(new System.Windows.Rect(new System.Windows.Size(this.Width,this.Height)));

        public Tile(int x, int y, TileType type)
        {
            Y = y;
            X = x;
            this.Type = type;
            this.Width = 15;
            this.Height = 15;
            RegionParentId = -1;
        }

        public void SetColor(bool region)
        {
            if (region && !(Type == TileType.CONNECTOR || Type == TileType.DOOR))
            {
                if (this.RegionId < 0)
                {
                    this.Fill = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    this.Fill = new SolidColorBrush(Regions.Instance.GetColor(this.RegionId));
                }
            }
            else
            {
                this.Fill = new SolidColorBrush(TypeInfo.Info[this.Type].Color);

            }
        }
    }
}
