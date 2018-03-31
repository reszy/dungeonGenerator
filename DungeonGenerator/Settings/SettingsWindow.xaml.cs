using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DungeonGenerator.Settings
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        ISettings refSettings;
        PropertyInfo[] properties;
        ISettings localSettings;

        const string NAME_PREFIX = "ValueOf_";

        public SettingsWindow(ISettings settings)
        {
            InitializeComponent();

            Grid grid = new Grid()
            {
                Background = new SolidColorBrush(Colors.DarkKhaki)
            };
            this.Content = grid;

            properties = settings.GetType().GetProperties();
            refSettings = settings;
            localSettings = settings.GetClone();

            const int offset = 30;
            int listedProperties = 0;

            foreach (var property in properties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    int verticalPosition = offset * listedProperties + 15;

                    string labelContent = ConvertToHumanReadable(property.Name);
                    grid.Children.Add(new Label
                    {
                        Content = labelContent,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = 230,
                        Height = offset,
                        Margin = new Thickness(20, verticalPosition, 0, 20),
                        Visibility = Visibility.Visible
                    });
                    Thickness valueThickness = new Thickness(250, verticalPosition, 20, 20);
                    string valueName = NAME_PREFIX + property.Name;
                    if (property.PropertyType == typeof(bool))
                    {
                        grid.Children.Add(new CheckBox
                        {
                            Name = valueName,
                            IsChecked = (bool)property.GetValue(localSettings),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = valueThickness,
                            Width = 20,
                            Height = 20,
                            Visibility = Visibility.Visible
                        });
                    }
                    else
                    {
                        grid.Children.Add(new TextBox
                        {
                            Name = valueName,
                            Text = property.GetValue(localSettings).ToString(),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Width = 150,
                            Height = 20,
                            Margin = valueThickness,
                            Visibility = Visibility.Visible
                        });
                    }
                    listedProperties++;
                }
            }
            this.SizeToContent = SizeToContent.Height;
            Button okButton = new Button
            {
                Content = "OK",
                Margin = new Thickness(250, offset * listedProperties + 40, 20, 10),
                Height = 20,
                Width = 50,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            okButton.Click += (source, e) =>
            {
                SetSettings();
                this.Close();
            };
            grid.Children.Add(okButton);
            Button cancelButton = new Button
            {
                Content = "Cancel",
                Margin = new Thickness(360, offset * listedProperties + 40, 20, 10),
                Height = 20,
                Width = 50,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            cancelButton.Click += (source, e) =>
            {
                this.Close();
            };
            grid.Children.Add(cancelButton);
        }

        private string ConvertToHumanReadable(string input)
        {
            return string.Concat(input.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + char.ToLower(x).ToString() : x.ToString()));
        }

        private void SetSettings()
        {
            foreach (var item in ((Grid)this.Content).Children)
            {
                Control control = (Control)item;
                if (control.Name.StartsWith(NAME_PREFIX))
                {
                    string propertyName = control.Name.Substring(NAME_PREFIX.Length);
                    if (control.GetType() == typeof(CheckBox))
                    {
                        var property = properties.First(p => p.Name == propertyName);
                        property.SetValue(localSettings, ((CheckBox)control).IsChecked);
                    }
                }
            }
            refSettings.SetSettings(localSettings);
        }
    }
}
