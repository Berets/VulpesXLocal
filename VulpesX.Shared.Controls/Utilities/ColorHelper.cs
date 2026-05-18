using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VulpesX.Shared.Controls.Utilities
{
    public static class ColorUtilities
    {
        public static Color ConvertStringToColor(string Color)
        {
            Color color = new Color();
            string alpha = Color.Substring(1, 2);
            int alphaI = Int32.Parse(alpha, System.Globalization.NumberStyles.HexNumber);

            string r = Color.Substring(3, 2);
            int rI = Int32.Parse(r, System.Globalization.NumberStyles.HexNumber);

            string g = Color.Substring(5, 2);
            int gI = Int32.Parse(g, System.Globalization.NumberStyles.HexNumber);

            string b = Color.Substring(7, 2);
            int bI = Int32.Parse(b, System.Globalization.NumberStyles.HexNumber);

            color.A = (byte)alphaI;
            color.R = (byte)rI;
            color.G = (byte)gI;
            color.B = (byte)bI;

            return color;
        }
    }
}
