//
// System.Drawing.SystemColors
//
// Copyright (C) 2002 Ximian, Inc (http://www.ximian.com)
// Copyright (C) 2004-2005, 2007 Novell, Inc (http://www.novell.com)
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
// Authors:
//	Gonzalo Paniagua Javier (gonzalo@ximian.com)
//	Peter Dennis Bartok (pbartok@novell.com)
//	Sebastien Pouliot  <sebastien@ximian.com>
//

namespace Rocket.API.Drawing
{
    public sealed class SystemColors
    {
        private SystemColors() { }

        public static Color ActiveBorder => KnownColors.FromKnownColor(KnownColor.ActiveBorder);

        public static Color ActiveCaption => KnownColors.FromKnownColor(KnownColor.ActiveCaption);

        public static Color ActiveCaptionText => KnownColors.FromKnownColor(KnownColor.ActiveCaptionText);

        public static Color AppWorkspace => KnownColors.FromKnownColor(KnownColor.AppWorkspace);

        public static Color Control => KnownColors.FromKnownColor(KnownColor.Control);

        public static Color ControlDark => KnownColors.FromKnownColor(KnownColor.ControlDark);

        public static Color ControlDarkDark => KnownColors.FromKnownColor(KnownColor.ControlDarkDark);

        public static Color ControlLight => KnownColors.FromKnownColor(KnownColor.ControlLight);

        public static Color ControlLightLight => KnownColors.FromKnownColor(KnownColor.ControlLightLight);

        public static Color ControlText => KnownColors.FromKnownColor(KnownColor.ControlText);

        public static Color Desktop => KnownColors.FromKnownColor(KnownColor.Desktop);

        public static Color GrayText => KnownColors.FromKnownColor(KnownColor.GrayText);

        public static Color Highlight => KnownColors.FromKnownColor(KnownColor.Highlight);

        public static Color HighlightText => KnownColors.FromKnownColor(KnownColor.HighlightText);

        public static Color HotTrack => KnownColors.FromKnownColor(KnownColor.HotTrack);

        public static Color InactiveBorder => KnownColors.FromKnownColor(KnownColor.InactiveBorder);

        public static Color InactiveCaption => KnownColors.FromKnownColor(KnownColor.InactiveCaption);

        public static Color InactiveCaptionText => KnownColors.FromKnownColor(KnownColor.InactiveCaptionText);

        public static Color Info => KnownColors.FromKnownColor(KnownColor.Info);

        public static Color InfoText => KnownColors.FromKnownColor(KnownColor.InfoText);

        public static Color Menu => KnownColors.FromKnownColor(KnownColor.Menu);

        public static Color MenuText => KnownColors.FromKnownColor(KnownColor.MenuText);

        public static Color ScrollBar => KnownColors.FromKnownColor(KnownColor.ScrollBar);

        public static Color Window => KnownColors.FromKnownColor(KnownColor.Window);

        public static Color WindowFrame => KnownColors.FromKnownColor(KnownColor.WindowFrame);

        public static Color WindowText => KnownColors.FromKnownColor(KnownColor.WindowText);

        public static Color ButtonFace => KnownColors.FromKnownColor(KnownColor.ButtonFace);

        public static Color ButtonHighlight => KnownColors.FromKnownColor(KnownColor.ButtonHighlight);

        public static Color ButtonShadow => KnownColors.FromKnownColor(KnownColor.ButtonShadow);

        public static Color GradientActiveCaption => KnownColors.FromKnownColor(KnownColor.GradientActiveCaption);

        public static Color GradientInactiveCaption => KnownColors.FromKnownColor(KnownColor.GradientInactiveCaption);

        public static Color MenuBar => KnownColors.FromKnownColor(KnownColor.MenuBar);

        public static Color MenuHighlight => KnownColors.FromKnownColor(KnownColor.MenuHighlight);
    }
}