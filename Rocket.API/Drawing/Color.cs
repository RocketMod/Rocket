//
// System.Drawing.Color.cs
//
// Authors:
// 	Dennis Hayes (dennish@raytek.com)
// 	Ben Houston  (ben@exocortex.org)
// 	Gonzalo Paniagua (gonzalo@ximian.com)
// 	Juraj Skripsky (juraj@hotfeet.ch)
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// (C) 2002 Dennis Hayes
// (c) 2002 Ximian, Inc. (http://www.ximiam.com)
// (C) 2005 HotFeet GmbH (http://www.hotfeet.ch)
// Copyright (C) 2004,2006-2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.ComponentModel;

namespace Rocket.API.Drawing
{
    [TypeConverter(typeof(ColorConverter))]
    [Serializable]
    public struct Color
    {
        // Private transparency (A) and R,G,B fields.
        private long value;

        // The specs also indicate that all three of these properties are true
        // if created with FromKnownColor or FromNamedColor, false otherwise (FromARGB).
        // Per Microsoft and ECMA specs these varibles are set by which constructor is used, not by their values.
        [Flags]
        internal enum ColorType : short
        {
            Empty = 0,
            Known = 1,
            ARGB = 2,
            Named = 4,
            System = 8
        }

        internal short state;

        internal short knownColor;

        // #if ONLY_1_1
        // Mono bug #324144 is holding this change
        // MS 1.1 requires this member to be present for serialization (not so in 2.0)
        // however it's bad to keep a string (reference) in a struct
        internal string name;
        // #endif

        public string Name
        {
            get
            {
#if NET_2_0_ONCE_MONO_BUG_324144_IS_FIXED
				if (IsNamedColor)
					return KnownColors.GetName (knownColor);
				else
					return String.Format ("{0:x}", ToArgb ());
#else
                // name is required for serialization under 1.x, but not under 2.0
                if (name == null)
                    if (IsNamedColor)
                        name = KnownColors.GetName(knownColor);
                    else
                        name = string.Format("{0:x}", ToArgb());
                return name;
#endif
            }
        }

        public bool IsKnownColor => (state & (short)ColorType.Known) != 0;

        public bool IsSystemColor => (state & (short)ColorType.System) != 0;

        public bool IsNamedColor => (state & (short)(ColorType.Known | ColorType.Named)) != 0;

        internal long Value
        {
            get
            {
                // Optimization for known colors that were deserialized
                // from an MS serialized stream.  
                if (value == 0 && IsKnownColor) value = FromKnownColor((KnownColor)knownColor).ToArgb() & 0xFFFFFFFF;
                return value;
            }
            set => this.value = value;
        }

        public static Color FromArgb(int red, int green, int blue) => FromArgb(255, red, green, blue);

        public static Color FromArgb(int alpha, int red, int green, int blue)
        {
            CheckARGBValues(alpha, red, green, blue);
            Color color = new Color();
            color.state = (short)ColorType.ARGB;
            color.Value = (int)((uint)alpha << 24) + (red << 16) + (green << 8) + blue;
            return color;
        }

        public int ToArgb() => (int)Value;

        public static Color FromArgb(int alpha, Color baseColor)
            => FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B);

        public static Color FromArgb(int argb)
            => FromArgb((argb >> 24) & 0x0FF, (argb >> 16) & 0x0FF, (argb >> 8) & 0x0FF, argb & 0x0FF);

        public static Color FromKnownColor(KnownColor color)
        {
            Color c;
            short n = (short)color;
            if (n <= 0 || n >= KnownColors.ArgbValues.Length)
            {
                // This is what it returns!
                c = FromArgb(0, 0, 0, 0);
                c.state |= (short)ColorType.Named;
            }
            else
            {
                c = new Color();
                c.state = (short)(ColorType.ARGB | ColorType.Known | ColorType.Named);
                if (n < 27 || n > 169)
                    c.state |= (short)ColorType.System;
                c.Value = KnownColors.ArgbValues[n];
            }

            c.knownColor = n;
            return c;
        }

        public static Color FromName(string name)
        {
            try
            {
                KnownColor kc = (KnownColor)Enum.Parse(typeof(KnownColor), name, true);
                return FromKnownColor(kc);
            }
            catch
            {
                // This is what it returns! 	 
                Color d = FromArgb(0, 0, 0, 0);
                d.name = name;
                d.state |= (short)ColorType.Named;
                return d;
            }
        }

        // -----------------------
        // Public Shared Members
        // -----------------------

        /// <summary>
        ///     Empty Shared Field
        /// </summary>
        /// <remarks>
        ///     An uninitialized Color Structure
        /// </remarks>
        public static readonly Color Empty;

        /// <summary>
        ///     Equality Operator
        /// </summary>
        /// <remarks>
        ///     Compares two Color objects. The return value is
        ///     based on the equivalence of the A,R,G,B properties
        ///     of the two Colors.
        /// </remarks>
        public static bool operator ==(Color left, Color right)
        {
            if (left.Value != right.Value)
                return false;
            if (left.IsNamedColor != right.IsNamedColor)
                return false;
            if (left.IsSystemColor != right.IsSystemColor)
                return false;
            if (left.IsEmpty != right.IsEmpty)
                return false;
            if (left.IsNamedColor)
                if (left.Name != right.Name)
                    return false;
            return true;
        }

        /// <summary>
        ///     Inequality Operator
        /// </summary>
        /// <remarks>
        ///     Compares two Color objects. The return value is
        ///     based on the equivalence of the A,R,G,B properties
        ///     of the two colors.
        /// </remarks>
        public static bool operator !=(Color left, Color right) => !(left == right);

        public float GetBrightness()
        {
            byte minval = Math.Min(R, Math.Min(G, B));
            byte maxval = Math.Max(R, Math.Max(G, B));

            return (float)(maxval + minval) / 510;
        }

        public float GetSaturation()
        {
            byte minval = Math.Min(R, Math.Min(G, B));
            byte maxval = Math.Max(R, Math.Max(G, B));

            if (maxval == minval)
                return 0.0f;

            int sum = maxval + minval;
            if (sum > 255)
                sum = 510 - sum;

            return (float)(maxval - minval) / sum;
        }

        public float GetHue()
        {
            int r = R;
            int g = G;
            int b = B;
            byte minval = (byte)Math.Min(r, Math.Min(g, b));
            byte maxval = (byte)Math.Max(r, Math.Max(g, b));

            if (maxval == minval)
                return 0.0f;

            float diff = maxval - minval;
            float rnorm = (maxval - r) / diff;
            float gnorm = (maxval - g) / diff;
            float bnorm = (maxval - b) / diff;

            float hue = 0.0f;
            if (r == maxval)
                hue = 60.0f * (6.0f + bnorm - gnorm);
            if (g == maxval)
                hue = 60.0f * (2.0f + rnorm - bnorm);
            if (b == maxval)
                hue = 60.0f * (4.0f + gnorm - rnorm);
            if (hue > 360.0f)
                hue = hue - 360.0f;

            return hue;
        }

        // -----------------------
        // Public Instance Members
        // -----------------------

        /// <summary>
        ///     ToKnownColor method
        /// </summary>
        /// <remarks>
        ///     Returns the KnownColor enum value for this color, 0 if is not known.
        /// </remarks>
        public KnownColor ToKnownColor() => (KnownColor)knownColor;

        /// <summary>
        ///     IsEmpty Property
        /// </summary>
        /// <remarks>
        ///     Indicates transparent black. R,G,B = 0; A=0?
        /// </remarks>

        public bool IsEmpty => state == (short)ColorType.Empty;

        public byte A => (byte)(Value >> 24);

        public byte R => (byte)(Value >> 16);

        public byte G => (byte)(Value >> 8);

        public byte B => (byte)Value;

        /// <summary>
        ///     Equals Method
        /// </summary>
        /// <remarks>
        ///     Checks equivalence of this Color and another object.
        /// </remarks>
        public override bool Equals(object obj)
        {
            if (!(obj is Color))
                return false;
            Color c = (Color)obj;
            return this == c;
        }

        /// <summary>
        ///     Reference Equals Method
        ///     Is commented out because this is handled by the base class.
        ///     TODO: Is it correct to let the base class handel reference equals
        /// </summary>
        /// <remarks>
        ///     Checks equivalence of this Color and another object.
        /// </remarks>
        /// <summary>
        ///     GetHashCode Method
        /// </summary>
        /// <remarks>
        ///     Calculates a hashing value.
        /// </remarks>
        public override int GetHashCode()
        {
            int hc = (int)(Value ^ (Value >> 32) ^ state ^ (knownColor >> 16));
            if (IsNamedColor)
                hc ^= Name.GetHashCode();
            return hc;
        }

        /// <summary>
        ///     ToString Method
        /// </summary>
        /// <remarks>
        ///     Formats the Color as a string in ARGB notation.
        /// </remarks>
        public override string ToString()
        {
            if (IsEmpty)
                return "Color [Empty]";

            // Use the property here, not the field.
            if (IsNamedColor)
                return "Color [" + Name + "]";

            return string.Format("Color [A={0}, R={1}, G={2}, B={3}]", A, R, G, B);
        }

        private static void CheckRGBValues(int red, int green, int blue)
        {
            if (red > 255 || red < 0)
                throw CreateColorArgumentException(red, "red");
            if (green > 255 || green < 0)
                throw CreateColorArgumentException(green, "green");
            if (blue > 255 || blue < 0)
                throw CreateColorArgumentException(blue, "blue");
        }

        private static ArgumentException CreateColorArgumentException(int value, string color) => new ArgumentException(
            string.Format("'{0}' is not a valid"
                + " value for '{1}'. '{1}' should be greater or equal to 0 and"
                + " less than or equal to 255.", value, color));

        private static void CheckARGBValues(int alpha, int red, int green, int blue)
        {
            if (alpha > 255 || alpha < 0)
                throw CreateColorArgumentException(alpha, "alpha");
            CheckRGBValues(red, green, blue);
        }

        public static Color Transparent => FromKnownColor(KnownColor.Transparent);

        public static Color AliceBlue => FromKnownColor(KnownColor.AliceBlue);

        public static Color AntiqueWhite => FromKnownColor(KnownColor.AntiqueWhite);

        public static Color Aqua => FromKnownColor(KnownColor.Aqua);

        public static Color Aquamarine => FromKnownColor(KnownColor.Aquamarine);

        public static Color Azure => FromKnownColor(KnownColor.Azure);

        public static Color Beige => FromKnownColor(KnownColor.Beige);

        public static Color Bisque => FromKnownColor(KnownColor.Bisque);

        public static Color Black => FromKnownColor(KnownColor.Black);

        public static Color BlanchedAlmond => FromKnownColor(KnownColor.BlanchedAlmond);

        public static Color Blue => FromKnownColor(KnownColor.Blue);

        public static Color BlueViolet => FromKnownColor(KnownColor.BlueViolet);

        public static Color Brown => FromKnownColor(KnownColor.Brown);

        public static Color BurlyWood => FromKnownColor(KnownColor.BurlyWood);

        public static Color CadetBlue => FromKnownColor(KnownColor.CadetBlue);

        public static Color Chartreuse => FromKnownColor(KnownColor.Chartreuse);

        public static Color Chocolate => FromKnownColor(KnownColor.Chocolate);

        public static Color Coral => FromKnownColor(KnownColor.Coral);

        public static Color CornflowerBlue => FromKnownColor(KnownColor.CornflowerBlue);

        public static Color Cornsilk => FromKnownColor(KnownColor.Cornsilk);

        public static Color Crimson => FromKnownColor(KnownColor.Crimson);

        public static Color Cyan => FromKnownColor(KnownColor.Cyan);

        public static Color DarkBlue => FromKnownColor(KnownColor.DarkBlue);

        public static Color DarkCyan => FromKnownColor(KnownColor.DarkCyan);

        public static Color DarkGoldenrod => FromKnownColor(KnownColor.DarkGoldenrod);

        public static Color DarkGray => FromKnownColor(KnownColor.DarkGray);

        public static Color DarkGreen => FromKnownColor(KnownColor.DarkGreen);

        public static Color DarkKhaki => FromKnownColor(KnownColor.DarkKhaki);

        public static Color DarkMagenta => FromKnownColor(KnownColor.DarkMagenta);

        public static Color DarkOliveGreen => FromKnownColor(KnownColor.DarkOliveGreen);

        public static Color DarkOrange => FromKnownColor(KnownColor.DarkOrange);

        public static Color DarkOrchid => FromKnownColor(KnownColor.DarkOrchid);

        public static Color DarkRed => FromKnownColor(KnownColor.DarkRed);

        public static Color DarkSalmon => FromKnownColor(KnownColor.DarkSalmon);

        public static Color DarkSeaGreen => FromKnownColor(KnownColor.DarkSeaGreen);

        public static Color DarkSlateBlue => FromKnownColor(KnownColor.DarkSlateBlue);

        public static Color DarkSlateGray => FromKnownColor(KnownColor.DarkSlateGray);

        public static Color DarkTurquoise => FromKnownColor(KnownColor.DarkTurquoise);

        public static Color DarkViolet => FromKnownColor(KnownColor.DarkViolet);

        public static Color DeepPink => FromKnownColor(KnownColor.DeepPink);

        public static Color DeepSkyBlue => FromKnownColor(KnownColor.DeepSkyBlue);

        public static Color DimGray => FromKnownColor(KnownColor.DimGray);

        public static Color DodgerBlue => FromKnownColor(KnownColor.DodgerBlue);

        public static Color Firebrick => FromKnownColor(KnownColor.Firebrick);

        public static Color FloralWhite => FromKnownColor(KnownColor.FloralWhite);

        public static Color ForestGreen => FromKnownColor(KnownColor.ForestGreen);

        public static Color Fuchsia => FromKnownColor(KnownColor.Fuchsia);

        public static Color Gainsboro => FromKnownColor(KnownColor.Gainsboro);

        public static Color GhostWhite => FromKnownColor(KnownColor.GhostWhite);

        public static Color Gold => FromKnownColor(KnownColor.Gold);

        public static Color Goldenrod => FromKnownColor(KnownColor.Goldenrod);

        public static Color Gray => FromKnownColor(KnownColor.Gray);

        public static Color Green => FromKnownColor(KnownColor.Green);

        public static Color GreenYellow => FromKnownColor(KnownColor.GreenYellow);

        public static Color Honeydew => FromKnownColor(KnownColor.Honeydew);

        public static Color HotPink => FromKnownColor(KnownColor.HotPink);

        public static Color IndianRed => FromKnownColor(KnownColor.IndianRed);

        public static Color Indigo => FromKnownColor(KnownColor.Indigo);

        public static Color Ivory => FromKnownColor(KnownColor.Ivory);

        public static Color Khaki => FromKnownColor(KnownColor.Khaki);

        public static Color Lavender => FromKnownColor(KnownColor.Lavender);

        public static Color LavenderBlush => FromKnownColor(KnownColor.LavenderBlush);

        public static Color LawnGreen => FromKnownColor(KnownColor.LawnGreen);

        public static Color LemonChiffon => FromKnownColor(KnownColor.LemonChiffon);

        public static Color LightBlue => FromKnownColor(KnownColor.LightBlue);

        public static Color LightCoral => FromKnownColor(KnownColor.LightCoral);

        public static Color LightCyan => FromKnownColor(KnownColor.LightCyan);

        public static Color LightGoldenrodYellow => FromKnownColor(KnownColor.LightGoldenrodYellow);

        public static Color LightGreen => FromKnownColor(KnownColor.LightGreen);

        public static Color LightGray => FromKnownColor(KnownColor.LightGray);

        public static Color LightPink => FromKnownColor(KnownColor.LightPink);

        public static Color LightSalmon => FromKnownColor(KnownColor.LightSalmon);

        public static Color LightSeaGreen => FromKnownColor(KnownColor.LightSeaGreen);

        public static Color LightSkyBlue => FromKnownColor(KnownColor.LightSkyBlue);

        public static Color LightSlateGray => FromKnownColor(KnownColor.LightSlateGray);

        public static Color LightSteelBlue => FromKnownColor(KnownColor.LightSteelBlue);

        public static Color LightYellow => FromKnownColor(KnownColor.LightYellow);

        public static Color Lime => FromKnownColor(KnownColor.Lime);

        public static Color LimeGreen => FromKnownColor(KnownColor.LimeGreen);

        public static Color Linen => FromKnownColor(KnownColor.Linen);

        public static Color Magenta => FromKnownColor(KnownColor.Magenta);

        public static Color Maroon => FromKnownColor(KnownColor.Maroon);

        public static Color MediumAquamarine => FromKnownColor(KnownColor.MediumAquamarine);

        public static Color MediumBlue => FromKnownColor(KnownColor.MediumBlue);

        public static Color MediumOrchid => FromKnownColor(KnownColor.MediumOrchid);

        public static Color MediumPurple => FromKnownColor(KnownColor.MediumPurple);

        public static Color MediumSeaGreen => FromKnownColor(KnownColor.MediumSeaGreen);

        public static Color MediumSlateBlue => FromKnownColor(KnownColor.MediumSlateBlue);

        public static Color MediumSpringGreen => FromKnownColor(KnownColor.MediumSpringGreen);

        public static Color MediumTurquoise => FromKnownColor(KnownColor.MediumTurquoise);

        public static Color MediumVioletRed => FromKnownColor(KnownColor.MediumVioletRed);

        public static Color MidnightBlue => FromKnownColor(KnownColor.MidnightBlue);

        public static Color MintCream => FromKnownColor(KnownColor.MintCream);

        public static Color MistyRose => FromKnownColor(KnownColor.MistyRose);

        public static Color Moccasin => FromKnownColor(KnownColor.Moccasin);

        public static Color NavajoWhite => FromKnownColor(KnownColor.NavajoWhite);

        public static Color Navy => FromKnownColor(KnownColor.Navy);

        public static Color OldLace => FromKnownColor(KnownColor.OldLace);

        public static Color Olive => FromKnownColor(KnownColor.Olive);

        public static Color OliveDrab => FromKnownColor(KnownColor.OliveDrab);

        public static Color Orange => FromKnownColor(KnownColor.Orange);

        public static Color OrangeRed => FromKnownColor(KnownColor.OrangeRed);

        public static Color Orchid => FromKnownColor(KnownColor.Orchid);

        public static Color PaleGoldenrod => FromKnownColor(KnownColor.PaleGoldenrod);

        public static Color PaleGreen => FromKnownColor(KnownColor.PaleGreen);

        public static Color PaleTurquoise => FromKnownColor(KnownColor.PaleTurquoise);

        public static Color PaleVioletRed => FromKnownColor(KnownColor.PaleVioletRed);

        public static Color PapayaWhip => FromKnownColor(KnownColor.PapayaWhip);

        public static Color PeachPuff => FromKnownColor(KnownColor.PeachPuff);

        public static Color Peru => FromKnownColor(KnownColor.Peru);

        public static Color Pink => FromKnownColor(KnownColor.Pink);

        public static Color Plum => FromKnownColor(KnownColor.Plum);

        public static Color PowderBlue => FromKnownColor(KnownColor.PowderBlue);

        public static Color Purple => FromKnownColor(KnownColor.Purple);

        public static Color Red => FromKnownColor(KnownColor.Red);

        public static Color RosyBrown => FromKnownColor(KnownColor.RosyBrown);

        public static Color RoyalBlue => FromKnownColor(KnownColor.RoyalBlue);

        public static Color SaddleBrown => FromKnownColor(KnownColor.SaddleBrown);

        public static Color Salmon => FromKnownColor(KnownColor.Salmon);

        public static Color SandyBrown => FromKnownColor(KnownColor.SandyBrown);

        public static Color SeaGreen => FromKnownColor(KnownColor.SeaGreen);

        public static Color SeaShell => FromKnownColor(KnownColor.SeaShell);

        public static Color Sienna => FromKnownColor(KnownColor.Sienna);

        public static Color Silver => FromKnownColor(KnownColor.Silver);

        public static Color SkyBlue => FromKnownColor(KnownColor.SkyBlue);

        public static Color SlateBlue => FromKnownColor(KnownColor.SlateBlue);

        public static Color SlateGray => FromKnownColor(KnownColor.SlateGray);

        public static Color Snow => FromKnownColor(KnownColor.Snow);

        public static Color SpringGreen => FromKnownColor(KnownColor.SpringGreen);

        public static Color SteelBlue => FromKnownColor(KnownColor.SteelBlue);

        public static Color Tan => FromKnownColor(KnownColor.Tan);

        public static Color Teal => FromKnownColor(KnownColor.Teal);

        public static Color Thistle => FromKnownColor(KnownColor.Thistle);

        public static Color Tomato => FromKnownColor(KnownColor.Tomato);

        public static Color Turquoise => FromKnownColor(KnownColor.Turquoise);

        public static Color Violet => FromKnownColor(KnownColor.Violet);

        public static Color Wheat => FromKnownColor(KnownColor.Wheat);

        public static Color White => FromKnownColor(KnownColor.White);

        public static Color WhiteSmoke => FromKnownColor(KnownColor.WhiteSmoke);

        public static Color Yellow => FromKnownColor(KnownColor.Yellow);

        public static Color YellowGreen => FromKnownColor(KnownColor.YellowGreen);
    }
}