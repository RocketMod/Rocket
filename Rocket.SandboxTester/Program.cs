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

            var asm = Assembly.LoadFile(Path.GetFullPath(name));
            var res = SafeCodeHandler.IsSafeAssembly(asm);
            if (res.Passed)
            {
                WriteLine("All checks passed. File is safe.", ConsoleColor.DarkGreen);
                goto start;
            }

            var blockedIns = res.IllegalInstruction;
            var pos = res.Position;

            WriteLine("Check not passed!", ConsoleColor.Red);
            WriteLine("> Failed on: " + pos.InstructionName);
            WriteLine("> Reason: " + blockedIns.BlockReason + " on " + blockedIns.InstructionName);
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
