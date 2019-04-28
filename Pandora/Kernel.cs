using System;
using System.IO;
using Sys = Cosmos.System;

namespace Pandora
{
    public class Kernel : Sys.Kernel
    {
        public double SYS_VERSION = 0.3;
        MissingFunctions functions = new MissingFunctions();

        protected override void BeforeRun()
        {
            Sys.PCSpeaker.Beep();
            Success("Kernel execution started.");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Screen res is " + Console.WindowWidth + "x" + Console.WindowHeight + ".");
            Console.ResetColor();

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
                                "init_vfs",
                                "list",
                                "memopad",
                                "",
                                "reboot",
                                "shutdown"
                            }
                        ) Console.WriteLine(line);
                        break;
                    case "init_vfs":
                        var fs = new Sys.FileSystem.CosmosVFS();
                        Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
                        Success("Initialised VFS.");
                        break;
                    case "list":
                        string cd = Directory.GetCurrentDirectory();
                        Console.WriteLine("Directory listing for " + cd);

                        ScrollWithPauses(Directory.GetDirectories(cd), "<DIR>  ", ConsoleColor.Magenta);
                        ScrollWithPauses(Directory.GetFiles(cd), "<FILE> ", ConsoleColor.Green);
                        Console.ResetColor();
                        break;
                    case "memopad":
                        Console.WriteLine("ALT+C to exit.");
                        ConsoleKeyInfo key; //key pressed

                        for (; ; )
                        {
                            int x = Console.CursorLeft;
                            int y = Console.CursorTop;
                            key = Console.ReadKey(true);

                            //update modifier states
                            bool ALT = false;
                            bool SHIFT = false;
                            if ((key.Modifiers & ConsoleModifiers.Alt) != 0) ALT = true;
                            if ((key.Modifiers & ConsoleModifiers.Shift) != 0) SHIFT = true;

                            if (ALT && key.KeyChar == 'c') break; //exit program
                            //backspace implementation
                            else if (key.Key == ConsoleKey.Backspace)
                            {
                                Console.CursorLeft--;
                                Console.Write(" ");
                                x--;
                            }
                            else if (key.Key == ConsoleKey.Enter)
                            {
                                x = 0;
                                y++;
                            }
                            //cursor key handling
                            else if (key.Key == ConsoleKey.RightArrow) x++;
                            else if (key.Key == ConsoleKey.LeftArrow) x--;
                            else if (key.Key == ConsoleKey.DownArrow) y++;
                            else if (key.Key == ConsoleKey.UpArrow) y--;

                            else
                            {
                                Console.Write(key.KeyChar); //if the key pressed was a normal character, print it.
                                x++;
                            }

                            //scroll if y exceeds screen height
                            if (y > Console.WindowHeight)
                            {
                                y = Console.WindowHeight - 3;
                                Console.WriteLine();
                            }
                            else if (y < 0) y = 0;
                            else
                            {
                                Console.SetCursorPosition(x, y); //update cursor position
                            }

                        }
                        Console.WriteLine();
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