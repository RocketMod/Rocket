using System;
using System.IO;
using System.Reflection;

namespace Rocket.SandboxTester
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("Rocketmod Sandbox Tester");
            start:
            WriteLine(string.Empty);
            WriteLine("Input file name: ", ConsoleColor.Yellow);
            Write("> ");
            string name = Console.ReadLine();
            if (!File.Exists(name))
            {
                WriteLine("File not found", ConsoleColor.Red);
                goto start;
            }

            var asm = Assembly.LoadFrom(Path.GetFullPath(name));
            string illegalInstruction;
            string failReason;
            var res = SafeCodeHandler.IsSafeAssembly(asm, out illegalInstruction, out failReason);
            if (res)
            {
                WriteLine("All checks passed. File is safe.", ConsoleColor.DarkGreen);
                goto start;
            }

            WriteLine("Check not passed!", ConsoleColor.Red);
            WriteLine("> Failed on: " + illegalInstruction);
            WriteLine("> Reason: " + failReason);
            goto start;
        }

        public static void Write(string s, ConsoleColor color = ConsoleColor.White)
        {
            var orgColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(s);
            Console.ForegroundColor = orgColor;
        }

        public static void WriteLine(string s, ConsoleColor color = ConsoleColor.White)
        {
            var orgColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(s);
            Console.ForegroundColor = orgColor;
        }
    }
}
