using System;
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
        private Brush bgBrush = new SolidColorBrush(Color.FromRgb(45, 45, 50));
        private Brush fontBrush = new SolidColorBrush(Color.FromRgb(220, 220, 220));
        private Brush validBgBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        private Brush invalidBgBrush = new SolidColorBrush(Color.FromRgb(220, 110, 110));

        private const string NAME_PREFIX = "ValueOf_";

        public SettingsWindow(ISettings settings)
        {
            InitializeComponent();

            Grid grid = new Grid()
            {
                Background = bgBrush
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

                    string labelContent = TextUtils.ConvertToHumanReadable(property.Name);
                    grid.Children.Add(new Label
                    {
                        Content = labelContent,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = 230,
                        Height = offset,
                        Margin = new Thickness(20, verticalPosition, 0, 20),
                        Visibility = Visibility.Visible,
                        Foreground = fontBrush
                    });
                    Thickness valueThickness = new Thickness(250, verticalPosition, 20, 20);
                    string valueName = NAME_PREFIX + property.Name;
                    dynamic value = property.GetValue(localSettings);
                    dynamic field = property.GetCustomAttribute(typeof(SettingAttribute));
                    grid.Children.Add(GenerateControl(value, valueThickness, valueName, field));
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

        private void SetSettings()
        {
            foreach (var item in ((Grid)this.Content).Children)
            {
                Control control = (Control)item;
                if (control.Name.StartsWith(NAME_PREFIX))
                {
                    string propertyName = control.Name.Substring(NAME_PREFIX.Length);
                    var property = properties.First(p => p.Name == propertyName);
                    if (property.GetCustomAttribute(typeof(SettingAttribute)) is SettingAttribute attr)
                    {
                        bool next = false;
                        if (control is TextBox)
                        {
                            if (!attr.IsValid(Int32.Parse((control as TextBox).Text)))
                                next = true;
                        }
                        if (next) continue;
                    }
                    if (control is TextBox)
                    {
                        property.SetValue(localSettings, Int32.Parse((control as TextBox).Text));
                    }
                    if (control is CheckBox)
                    {
                        property.SetValue(localSettings, ((CheckBox)control).IsChecked);
                    }
                    if (control is ComboBox)
                    {
                        property.SetValue(localSettings, Enum.Parse(property.PropertyType, TextUtils.ConvertFromHumanReadable(((ComboBox)control).Text)));
                    }
                }
            }
            refSettings.SetSettings(localSettings);
        }

        private Control GenerateControl(bool value, Thickness valueThickness, string name, Object o)
        {
            return new CheckBox
            {
                Name = name,
                IsChecked = value,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = valueThickness,
                Width = 20,
                Height = 20,
                Visibility = Visibility.Visible
            };
        }

        private Control GenerateControl(int value, Thickness valueThickness, string name, Object o)
        {
            var textBox = new TextBox
            {
                Name = name,
                Text = value.ToString(),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 150,
                Height = 20,
                Margin = valueThickness,
                Visibility = Visibility.Visible
            };
            if (o != null && o is SettingAttribute)
            {
                textBox.LostFocus += (sender, e) =>
                {
                    var tb = (TextBox)sender;
                    var attr = (SettingAttribute)o;
                    int output;
                    if (attr.IsValid(output = Int32.Parse(textBox.Text)))
                    {
                        tb.Text = output.ToString();
                        tb.Background = validBgBrush;
                    }
                    else
                    {
                        tb.Background = invalidBgBrush;
                        MessageBox.Show("Wrong value. Please enter value between " + attr.MinValue + " and " + attr.MaxValue);
                    }
                };
            }
            return textBox;
        }

        private Control GenerateControl(ISettings value, Thickness valueThickness, string name, Object o)
        {
            var button = new Button()
            {
                Content = "Open settings",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 25,
                Margin = valueThickness,
            };
            button.Click += (sender, e) =>
            {
                var settingsDialog = new SettingsWindow(value);
                settingsDialog.ShowDialog();
            };
            return button;
        }

        private Control GenerateControl(Enum value, Thickness valueThickness, string name, Object o)
        {
            var comboBox = new ComboBox
            {
                Name = name,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 150,
                Height = 24,
                Margin = valueThickness,
                Visibility = Visibility.Visible
            };
            foreach (var enumName in Enum.GetNames(value.GetType()))
            {
                comboBox.Items.Add(TextUtils.ConvertToHumanReadable(enumName));
            }
            comboBox.SelectedItem = TextUtils.ConvertToHumanReadable(value.ToString());
            return comboBox;
        }
    }
}
