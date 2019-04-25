using System;
using System.IO;
using Sys = Cosmos.System;

namespace Pandora
{
    public class Kernel : Sys.Kernel
    {
        protected override void BeforeRun()
        {
            Console.WriteLine("PandoraOS booted successfully.");
        }

        protected override void Run()
        {
            for (; ; )
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
                                "shutdown"
                            }
                        ) Console.WriteLine(line);
                        break;
                    case "shutdown":
                        Sys.Power.Shutdown();
                        break;
                    default:
                        Error("Unknown command.");
                        break;
                }
            }
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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errormesg);
            Console.ResetColor();
        }
    }
}
