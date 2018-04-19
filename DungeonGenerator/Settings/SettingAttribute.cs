using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerator.Settings
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SettingAttribute : Attribute
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        public bool IsValid(int value)
        {
            return !(value < MinValue || value > MaxValue);
        }
    }
}
