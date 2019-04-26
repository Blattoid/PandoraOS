using System;
using System.IO;
using Sys = Cosmos.System;

namespace Pandora
{
    public class Kernel : Sys.Kernel
    {
        public double SYS_VERSION = 0.2;
        protected override void BeforeRun()
        {
            Sys.PCSpeaker.Beep();
            Success("Kernel execution started.");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Screen res is " + Console.WindowWidth + "x" + Console.WindowHeight + ".");
            Console.ResetColor();
            var fs = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
            Success("Initialised VFS.");

            Success("PandoraOS V" + SYS_VERSION + " booted successfully.");
        }

        protected override void Run()
        {
            try
            {
                //read user command
                Console.Write(">"); //readline
                string[] input = Console.ReadLine().Split(" "); //split by spaces
                string command = input[0].ToLower(); //grab lowercase of command

                switch (command)
                {
                    case "help":
                        foreach (string line in new string[]
                            {
                                "help",
                                "",
                                "list",
                                "",
                                "reboot",
                                "shutdown"
                            }
                        ) Console.WriteLine(line);
                        break;
                    case "list":
                        string cd = Directory.GetCurrentDirectory();
                        Console.WriteLine("Directory listing for " + cd);

                        ScrollWithPauses(Directory.GetDirectories(cd), "<DIR>  ", ConsoleColor.Magenta);
                        ScrollWithPauses(Directory.GetFiles(cd), "<FILE> ", ConsoleColor.Green);
                        Console.ResetColor();
                        break;
                    case ".":
                        Success(ConvertToInt(input[1]).ToString());
                        break;
                    case "reboot":
                        Sys.Power.Reboot();
                        break;
                    case "shutdown":
                        Sys.Power.Shutdown();
                        break;
                    default:
                        Error("Unknown command.");
                        break;
                }
            }
            catch (Exception err) { Error(err.Message); }
        }

        //message printing
        void Error(string errormesg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errormesg);
            Console.ResetColor();
        }
        void Success(string errormesg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(errormesg);
            Console.ResetColor();
        }

        //equivalent of 'less' in unix
        void ScrollWithPauses(string[] scrolltext, string messageprefix = "", ConsoleColor textcolour = ConsoleColor.White)
        {
            int line = 0;
            foreach (string entry in scrolltext) {
                Console.ForegroundColor = textcolour;
                Console.WriteLine(messageprefix + entry);

                if (line >= Console.WindowHeight - 4)
                {
                    line = 0;
                    Console.ResetColor();
                    Console.Write("Press any key for more.");
                    Console.ReadKey();
                    Console.WriteLine();
                }
                line++;
            }
            Console.ResetColor();
        }

        //Convert.ToInt32 doesn't work, so I have to implement it myself in such a way that it does work. It will not be efficient.
        int ConvertToInt(string input)
        {
            Int32 output = 0 ;
            int multiplier = 1;
            for (int i = input.Length - 1; i >= 0; i -= 1)
            {
                int x=0;
                string y = input[i].ToString();
                if (y == "0") x = 0;
                if (y == "1") x = 1;
                if (y == "2") x = 2;
                if (y == "3") x = 3;
                if (y == "4") x = 4;
                if (y == "5") x = 5;
                if (y == "6") x = 6;
                if (y == "7") x = 7;
                if (y == "8") x = 8;
                if (y == "9") x = 9;
                output += x * multiplier;
                multiplier *= 10;
            }
            return output;
        }
    }
}