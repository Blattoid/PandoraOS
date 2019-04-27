using System;
using System.IO;
using Sys = Cosmos.System;

namespace Pandora
{
    public class Kernel : Sys.Kernel
    {
        public double SYS_VERSION = 0.2;
        MissingFunctions functions = new MissingFunctions();

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
    }
}