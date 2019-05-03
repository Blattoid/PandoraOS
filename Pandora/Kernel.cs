using System;
using System.IO;
using System.Collections.Generic;
using Sys = Cosmos.System;

namespace Pandora
{
    public class Kernel : Sys.Kernel
    {
        public const double SYS_VERSION = 0.32;
        MissingFunctions functions = new MissingFunctions();
        bool IsVFSInit = false; //Has the VFS been initialised? (needed for any disk access functions)
        public Sys.FileSystem.CosmosVFS filesys;

        protected override void BeforeRun()
        {
            //at this point, our code is executing. print a message to inform the user of this.
            Success("Kernel execution started.");

            //startup beep tune :)
            Sys.PCSpeaker.Beep(600, 200);
            Sys.PCSpeaker.Beep(800, 200);
            Sys.PCSpeaker.Beep(1000, 200);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Screen res is " + Console.WindowWidth + "x" + Console.WindowHeight + ".");
            Console.ResetColor();

            Success(string.Format("-=PandoraOS V{0} booted successfully=-",SYS_VERSION));
        }

        protected override void Run()
        {
            try
            {
                //read user command
                Console.Write(">"); //line prefix
                string[] input = Console.ReadLine().Split(" "); //split by spaces
                string command = input[0].ToLower(); //grab lowercase of command

                if (command == "help")
                {
                    foreach (string line in new string[]
                        {
                                "help\t\tDisplays this help",
                                "memopad\t\tAllows you to write anywhere on the screen.",
                                "",
                                "init_vfs\t\tInitialises the Virtual Filesystem Manager.",
                                "list\t\tLists the files in the current directory.",
                                "edit\t\tAllows rudimentary file editing.",
                                "",
                                "reboot\t\tRestarts the system.",
                                "shutdown\t\tTurns the system off."
                        }
                    ) Console.WriteLine(line);
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

                else if (command == "init_vfs")
                {
                    if (IsVFSInit)
                    {
                        Error("VFS is already initialised. Reboot if you want to unload it.");
                        return;
                    }

                    Error("-=!!WARNING!!=-\nThe CosmosOS FAT driver is still in experimental stages.\nPROCEEDING MAY CAUSE A LOSS OF DATA!");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Initialise anyway? y/N ");
                    if (!(Console.ReadKey().Key == ConsoleKey.Y))
                    {
                        Console.WriteLine("\nAborted.");
                        return;
                    }
                    //reset colour
                    Console.ResetColor();

                    filesys = new Sys.FileSystem.CosmosVFS();
                    Sys.FileSystem.VFS.VFSManager.RegisterVFS(filesys);
                    Success("Initialised VFS.");
                    IsVFSInit = true;
                }
                else if (command == "list")
                {
                    if (!IsVFSInit) { Error("VFS not initialised!"); return; } //refuse to proceed if the VFS has not been initialised
                    string cd = Directory.GetCurrentDirectory();
                    Console.WriteLine("Directory listing for " + cd);

                    ScrollWithPauses(Directory.GetDirectories(cd), "<DIR>  ", ConsoleColor.Magenta);
                    ScrollWithPauses(Directory.GetFiles(cd), "<FILE> ", ConsoleColor.Green);
                    Console.ResetColor();
                }
                else if (command == "edit")
                {
                    if (!IsVFSInit) { Error("VFS not initialised!"); return; } //refuse to proceed if the VFS has not been initialised

                    string filename;
                    List<string> filecontent = new List<string>();
                    Console.WriteLine("-=File Editor=-");
                    for (; ; )
                    {
                        //read user command
                        Console.Write(":"); //command prefix
                        input = Console.ReadLine().Split(" "); //split by spaces
                        command = input[0].ToLower(); //grab lowercase of command

                        if (command == "help")
                        {
                            foreach (string line in new string[]
                                {
                                    "help\t\tDisplays this help.",
                                    "set_filename <filename>\t\tSets the filename to write to.",
                                    "load <filename>\t\tLoads a file from disk.",

                                    "line <line no>\t\tSets the text on a given line to some text.",
                                    "list [line no]\t\tLists the contents of either the whole file or a specific line.",

                                    "save\t\tSaves the file to disk and exits.",
                                    "discard\t\tExit without saving."
                                }
                            ) Console.WriteLine(line);
                        }
                        else if (command == "set_filename")
                        {
                            if (input.Length > 1)
                            {
                                filename = input[1];
                                Success(string.Format("Set filename to '{0}'", filename));
                            }
                        }

                        else if (command == "line")
                        {
                            //idiot-proofing
                            //parameter count checks
                            if (input.Length < 3)
                            {
                                Error("Insufficient parameters.");
                                continue;
                            }
                            else if (input.Length > 3) Warning("Excessive parameters, proceeding anyway.");
                            //checks the line number is actually an integer
                            int linenumber;
                            if (int.TryParse(input[1], out linenumber))
                            { 
                                //it is an integer, is it less than 1?
                                if (linenumber < 1)
                                {
                                    Error("Line number is less than 1.");
                                    continue;
                                }
                            }
                            else
                            {
                                //it is not an integer
                                Error("Invalid line number.");
                                continue;
                            }

                            //generate extra blank lines in the array if we need to.
                            while (filecontent.Count < linenumber) filecontent.Add("");

                            //Get the new content for the line 
                            Console.Write("? ");
                            filecontent[linenumber-1] = Console.ReadLine();

                        }
                        else if (command == "list")
                        {
                            if (input.Length > 1)
                            {
                                //idiot-proofing
                                //checks the line number is actually an integer
                                int linenumber;
                                if (int.TryParse(input[1], out linenumber))
                                {
                                    //it is an integer, is it less than 1?
                                    if (linenumber < 1)
                                    {
                                        Error("Line number is less than 1.");
                                        continue;
                                    }
                                }
                                else
                                {
                                    //it is not an integer
                                    Error("Invalid line number.");
                                    continue;
                                }

                                if (linenumber > filecontent.Count)
                                {
                                    //the specified line exceeds the line count of the file
                                    Error("Out of range.");
                                    continue;
                                }

                                //output the line they requested.
                                Console.WriteLine(filecontent[linenumber - 1]);
                            }
                            else
                            {
                                //Simply output every single line, lmao
                                foreach (string line in filecontent) Console.WriteLine(line);
                            }
                        }

                        else if (command == "discard") break; //exit the loop
                        else Error("Unknown command. Type 'help' for a list of editor commands.");
                    }
                }

                else if (command == "reboot") Sys.Power.Reboot();
                else if (command == "shutdown") Sys.Power.Shutdown();
                else Error("Unknown command. Type 'help' for a list of commands.");
            }
            catch (Exception err) { Error(err.Message); }
        }

        //colour-coded message printing
        void Error(string mesg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(mesg);
            Console.ResetColor();
        }
        void Warning(string mesg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(mesg);
            Console.ResetColor();
        }
        void Success(string mesg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(mesg);
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