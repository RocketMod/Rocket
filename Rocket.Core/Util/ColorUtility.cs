using System.Drawing;
using System.Globalization;

namespace Rocket.Core.Util
{
    public class ColorUtility
    {
        public Color? GetColorFromString(string colorString)
        {
            return GetColorFromName(colorString) ?? GetColorFromHex(colorString);
        }

        public Color? GetColorFromHex(string colorHex)
        {
            if (int.TryParse(colorHex.Replace("#", ""), NumberStyles.HexNumber, null, out int argb))
            {
                return Color.FromArgb(argb);
            }

            return null;
        }

        public Color? GetColorFromName(string colorName)
        {
            var color = Color.FromName(colorName);
            return color == default(Color) ? (Color?) null : color;
        }
    }
}