using DungeonGenerator.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerator.Structure
{
    public class MazeSettings : ISettings
    {
        public MazeSettings()
        {
            MazeGenerationMethod = MazeEngine.Simple;
        }

        public MazeEngine MazeGenerationMethod { get; set; }

        public ISettings GetClone()
        {
            return (MazeSettings)this.MemberwiseClone();
        }

        public void SetSettings(ISettings settings)
        {
            if (settings.GetType() == typeof(MazeSettings))
            {
                var properties = typeof(MazeSettings).GetProperties();
                foreach (var property in properties)
                {
                    if (property.CanRead && property.CanWrite)
                    {
                        property.SetValue(this, property.GetValue((MazeSettings)settings));
                    }
                }
            }
            else
            {
                throw new InvalidCastException("Type must be of " + typeof(MazeSettings).ToString() + " instead of " + settings.GetType());
            }
        }
    }
}
