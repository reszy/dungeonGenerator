using DungeonGenerator.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerator.Structure
{
    public class GeneratorSettings : ISettings
    {
        public GeneratorSettings()
        {
            AllowDoubleDoors = true;
            AllowToMergeRooms = true;
            QuantityOfRooms = 10;
            RoomMaxSize = 8;
            RoomMinSize = 3;
            MapHeight = 41;
            MapWidth = 41;
            SaveStateBeforeGarbageClean = false;
            StepByStepGeneration = false;
        }

        public ISettings GetClone()
        {
            return (GeneratorSettings)this.MemberwiseClone();
        }

        public void SetSettings(ISettings settings)
        {
            if (settings.GetType() == typeof(GeneratorSettings))
            {
                var properties = typeof(GeneratorSettings).GetProperties();
                foreach (var property in properties)
                {
                    if (property.CanRead && property.CanWrite)
                    {
                        property.SetValue(this, property.GetValue((GeneratorSettings)settings));
                    }
                }
            }
            else
            {
                throw new InvalidCastException("Type must be of " + typeof(GeneratorSettings).ToString() + " instead of " + settings.GetType());
            }
        }

        public bool AllowDoubleDoors { get; set; }
        public bool AllowToMergeRooms { get; set; }
        public int QuantityOfRooms { get; set; }
        public int RoomMaxSize { get; set; }
        public int RoomMinSize { get; set; }
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }

        //DEBUG SETTING
        public bool SaveStateBeforeGarbageClean { get; set; }
        public bool StepByStepGeneration { get; set; }
    }
}
