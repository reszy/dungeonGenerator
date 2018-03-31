using System.Windows;
using DungeonGenerator.Generator;
using System.Windows.Shapes;
using System.Windows.Media;
using DungeonGenerator.Structure;
using System.Windows.Controls;
using System.Windows.Input;
using System;

namespace DungeonGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MapGenerator mapGenerator;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            Regions.Reset();
            int size = (int)SizeSlider.Value;
            this.canvas.Children.Clear();
            mapGenerator = new MapGenerator(new Structure.Size(size, size));
            mapGenerator.SetRoomCount((int)RoomsSlider.Value);

            if (byStepsCheckBox.IsChecked ?? false)
            {
                NextStepButton_Copy.IsEnabled = true;
                byStepsCheckBox.IsEnabled = false;
                GenerateButton.IsEnabled = false;
                mapGenerator.DoStep();
            }
            else
            {
                mapGenerator.GenerateMap((int)RoomsSlider.Value);
            }

            mapGenerator.DrawMap(this.canvas, showRegionsCheckBox.IsChecked ?? false);
        }

        private void NextStep_Click(object sender, RoutedEventArgs e)
        {
            mapGenerator.DoStep();
            this.canvas.Children.Clear();
            mapGenerator.DrawMap(this.canvas, showRegionsCheckBox.IsChecked ?? false);
            if (mapGenerator.IsDone)
            {
                NextStepButton_Copy.IsEnabled = false;
                byStepsCheckBox.IsEnabled = true;
                GenerateButton.IsEnabled = true;
            }
        }

        private void ChangeDisplaying_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in this.canvas.Children)
            {
                Tile tile = (Tile)child;
                if (tile != null)
                {
                    tile.SetColor(showRegionsCheckBox.IsChecked ?? false);
                }
            }
        }

        private void MoveOverCanvas(object sender, MouseEventArgs e)
        {
            Point p = Mouse.GetPosition(canvas);
            Position pos = new Position((int)p.X / 15, (int)p.Y / 15);
            coordsLabel.Text = "X: " + pos.X + ", Y:" + pos.Y;

            if (mapGenerator != null)
            {
                Tile tile = mapGenerator.GetTile(pos.X, pos.Y);
                if (tile != null)
                {
                    coordsLabel.Text += "\nid:" + tile.Id +
                                        "\ntype: " + tile.Type +
                                        "\nREGION:" +
                                        "\nid:" + tile.RegionId +
                                        "\nParent:" + tile.RegionParentId +
                                        "\nType: " + tile.RegionType;
                }
            }
        }

        private void InitHelperCanvas(object sender, EventArgs e)
        {
            this.heleprPanel.Children.Clear();
            for (int i = 0; i < 20; i++)
            {
                Color color = Regions.Instance.GetColor(i);
                Rectangle square = new Rectangle()
                {
                    Width = 50,
                    Height = 20,
                    Fill = new SolidColorBrush(color)
                };
                Canvas.SetTop(square, i * square.Height);
                this.heleprPanel.Children.Add(square);
            }
        }

        private void SetRoomCount(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.RoomSliderTextBox != null)
            {
                this.RoomSliderTextBox.Text = e.NewValue.ToString();
            }
        }

        private void SetMapSize(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.MapSliderTextBox != null)
            {
                this.MapSliderTextBox.Text = e.NewValue.ToString();
            }
        }
    }
}
