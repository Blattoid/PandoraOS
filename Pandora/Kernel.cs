using System;
using System.IO;
using Sys = Cosmos.System;

namespace Pandora
{
    public class Kernel : Sys.Kernel
    {
        public double SYS_VERSION = 0.31;
        MissingFunctions functions = new MissingFunctions();
        public Sys.FileSystem.CosmosVFS filesys;

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
            Console.WriteLine(Console.ReadLine().Length+" characters");
            try
            {
                //read user command
                Console.Write(">"); //readline
                string[] input = Console.ReadLine().Split(" "); //split by spaces
                string command = input[0].ToLower(); //grab lowercase of command

                if (command == "help")
                {
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
                }
                else if (command == "init_vfs")
                {
                    filesys = new Sys.FileSystem.CosmosVFS();
                    Sys.FileSystem.VFS.VFSManager.RegisterVFS(filesys);
                    Success("Initialised VFS.");
                }
                else if (command == "list")
                {
                    string cd = Directory.GetCurrentDirectory();
                    Console.WriteLine("Directory listing for " + cd);

                    ScrollWithPauses(Directory.GetDirectories(cd), "<DIR>  ", ConsoleColor.Magenta);
                    ScrollWithPauses(Directory.GetFiles(cd), "<FILE> ", ConsoleColor.Green);
                    Console.ResetColor();
                }
                else if (command == "memopad")
                {
                    ConsoleKeyInfo key; //key pressed

                    Console.WriteLine("Press ALT+C to exit.");
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

                        Console.SetCursorPosition(0, 0);
                        Console.Write(x + ", " + y + " ");

                        //loop cursor if it goes past the screen edge
                        if (y > Console.WindowHeight - 1) y = 0;
                        if (x < 0)
                        {
                            //cursor should go to the end of the previous line
                            x = Console.WindowWidth;
                            y--;
                        }
                        else if (y < 0) y = 0;
                        Console.SetCursorPosition(x, y); //update cursor position
                    }
                    Console.WriteLine();
                }
                else if (command == "reboot") Sys.Power.Reboot();
                else if (command == "shutdown") Sys.Power.Shutdown();
                else Error("Unknown command.");
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
            foreach (string entry in scrolltext)
            {
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