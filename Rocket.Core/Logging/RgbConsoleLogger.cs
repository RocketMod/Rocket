using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using Rocket.API.DependencyInjection;

namespace Rocket.Core.Logging
{
    public class RgbConsoleLogger : ConsoleLogger
    {
        private const ConsoleColor placeHolder = ConsoleColor.Blue;

        private const int STD_OUTPUT_HANDLE = -11;                                        // per WinBase.h
        internal static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);    // per WinBase.h

        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleTextAttribute(IntPtr hConsoleOutput, int wAttributes);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleScreenBufferInfoEx(IntPtr ConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX ConsoleScreenBufferInfoEx);

        [StructLayout(LayoutKind.Sequential)]
        internal struct COORD
        {
            internal short X;
            internal short Y;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct SMALL_RECT
        {
            internal short Left;
            internal short Top;
            internal short Right;
            internal short Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct COLORREF
        {
            internal uint ColorDWORD;

            internal COLORREF(Color color)
            {
                ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
            }

            internal COLORREF(uint r, uint g, uint b)
            {
                ColorDWORD = r + (g << 8) + (b << 16);
            }

            internal Color GetColor()
            {
                return Color.FromArgb((int)(0x000000FFU & ColorDWORD),
                   (int)(0x0000FF00U & ColorDWORD) >> 8, (int)(0x00FF0000U & ColorDWORD) >> 16);
            }

            internal void SetColor(Color color)
            {
                ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CONSOLE_SCREEN_BUFFER_INFO_EX
        {
            internal int cbSize;
            internal COORD dwSize;
            internal COORD dwCursorPosition;
            internal ushort wAttributes;
            internal SMALL_RECT srWindow;
            internal COORD dwMaximumWindowSize;
            internal ushort wPopupAttributes;
            internal bool bFullscreenSupported;
            internal COLORREF black;
            internal COLORREF darkBlue;
            internal COLORREF darkGreen;
            internal COLORREF darkCyan;
            internal COLORREF darkRed;
            internal COLORREF darkMagenta;
            internal COLORREF darkYellow;
            internal COLORREF gray;
            internal COLORREF darkGray;
            internal COLORREF blue;
            internal COLORREF green;
            internal COLORREF cyan;
            internal COLORREF red;
            internal COLORREF magenta;
            internal COLORREF yellow;
            internal COLORREF white;
        };


        public static void SetColor( uint r, uint g, uint b)
        {
            CONSOLE_SCREEN_BUFFER_INFO_EX csbe = new CONSOLE_SCREEN_BUFFER_INFO_EX();
            csbe.cbSize = (int)Marshal.SizeOf(csbe);                    // 96 = 0x60
            IntPtr hConsoleOutput = GetStdHandle(STD_OUTPUT_HANDLE);    // 7
            if (hConsoleOutput == INVALID_HANDLE_VALUE)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            bool brc = GetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
            if (!brc)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            csbe.blue = new COLORREF(r, g, b);
            ++csbe.srWindow.Bottom;
            ++csbe.srWindow.Right;
            brc = SetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
            if (!brc)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public RgbConsoleLogger(IDependencyContainer container) : base(container)
        {
        }

        protected override void WriteColored(string format, Color? color = null, params object[] bindings)
        {
            if (color == null)
            {
                Console.Write(format, bindings);
                return;
            }

            var orgColor = Console.ForegroundColor;
            SetColor(color.Value.R, color.Value.G, color.Value.B);
            Console.ForegroundColor = placeHolder;
            Console.Write(format, bindings);
            RestoreColor();
            Console.ForegroundColor = orgColor;
        }

        protected override void WriteLineColored(string format, Color? color = null, params object[] bindings)
        {
            if (color == null)
            {
                Console.WriteLine(format, bindings);
                return;
            }

            var orgColor = Console.ForegroundColor;
            SetColor(color.Value.R, color.Value.G, color.Value.B);
            Console.ForegroundColor = placeHolder;
            Console.WriteLine(format, bindings);
            RestoreColor();
            Console.ForegroundColor = orgColor;
        }

        private void RestoreColor()
        {
            SetColor(Color.Blue.R, Color.Blue.G, Color.Blue.B);
        }

        public override string ServiceName => "RgbConsole";
    }
}