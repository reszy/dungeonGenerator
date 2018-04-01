using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DungeonGenerator.Settings
{
    public class TextUtils
    {
        private const string HUMAN_TO_CODE_REGEX = " ([a-z])";

        public static string ConvertToHumanReadable(string input)
        {
            return string.Concat(input.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + char.ToLower(x).ToString() : x.ToString()));
        }

        public static string ConvertFromHumanReadable(string input)
        {
            return Regex.Replace(input, HUMAN_TO_CODE_REGEX, m => m.ToString().ToUpper().Substring(1));
        }
    }
}
