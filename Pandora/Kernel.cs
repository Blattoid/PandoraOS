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

            //cute startup beep tune :)
			for (uint i = 600; i <= 1000; i += 200) Sys.PCSpeaker.Beep(i, 200);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Screen res is " + Console.WindowWidth + "x" + Console.WindowHeight + ".");
            Console.ResetColor();

            Success(string.Format("-=PandoraOS V{0} booted successfully=-", SYS_VERSION));
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
                    foreach (string[] line in new string[][]
                        {
                                new string[] {"help","Displays this help" },
                                new string[] {"memopad","Allows you to write anywhere on the screen." },
                                new string[] {""},
                                new string[] { "init_vfs", "Initialises the Virtual Filesystem Manager." },
                                new string[] { "list", "Lists the files in the current directory." },
                                new string[] { "edit", "Allows rudimentary file editing." },
                                new string[] {""},
                                new string[] {"reboot","Restarts the system." },
                                new string[] {"shutdown","Turns the system off." }
                        }
                    ) OutputHelpText(line);
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

                    //display warning about possible data corruption
                    Error("-=!!WARNING!!=-\nThe CosmosOS FAT driver is still in experimental stages.\nPROCEEDING MAY CAUSE A LOSS OF DATA!");
                    Warning("Initialise anyway? y/N ", false);

                    //read user input
                    if (!(Console.ReadKey().Key == ConsoleKey.Y))
                    {
                        Console.WriteLine("\nAborted.");
                        return;
                    }

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

                    List<string> filecontent = new List<string>();
                    Console.WriteLine("-=File Editor=-");
                    for (; ; )
                    {
                        //read user command
                        Console.Write("EDIT:"); //command prefix
                        input = Console.ReadLine().Split(" "); //split by spaces
                        command = input[0].ToLower(); //grab lowercase of command

                        if (command == "help")
                        {
                            foreach (string[] line in new string[][]
                                {
                                    new string[] {"help","Displays this help." },
                                    new string[] {"load <filename>","Loads a file from disk." },
                                    new string[] {"" },
                                    new string[] {"line <line no>","Sets the text on a given line to some text." },
                                    new string[] {"list [line no]","Lists the contents of either the whole file" },
                                    new string[] {"", "or a specific line."},
                                    new string[] {"count", "Counts the number of lines in the file."},
                                    new string[] {"" },
                                    new string[] {"save <filename>","Saves the file to disk." },
                                    new string[] {"discard / exit","Exit without saving." }
                                }
                            ) OutputHelpText(line);
                        }
                        else if (command == "load")
                        {
                            //check if a filename was specified.
                            if (input.Length < 2)
                            {
                                Error("No filename was specified.");
                                continue;
                            }

                            //check if we are about to overwrite any unsaved work
                            if (filecontent.Count > 0)
                            {
                                Warning("You are about to discard your unsaved work.\nProceed? y/N ", false);

                                //read user input
                                if (!(Console.ReadKey().Key == ConsoleKey.Y))
                                {
                                    Console.WriteLine("\nAborted load.");
                                    continue;
                                }
                            }

                            if (File.Exists(input[1]))
                            {
                                Console.WriteLine("Loading file...");
                                string filename = input[1]; //update the filename
                                filecontent.Clear(); //clear any content that might already be in the buffer.
                                foreach (string line in File.ReadAllLines(filename)) filecontent.Add(line); //add all the lines
                                Success(string.Format("Loaded {0} lines!", filecontent.Count));
                            }
                            else
                            {
                                Error("File not found.");
                            }
                        }

                        else if (command == "line")
                        {
                            //idiot-proofing
                            //parameter count checks
                            if (input.Length < 2)
                            {
                                Error("Insufficient parameters.");
                                continue;
                            }
                            else if (input.Length > 3) Warning("Excessive parameters, proceeding anyway.");
                            //checks the line number is actually an integer
                            if (int.TryParse(input[1], out int linenumber))
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
                            filecontent[linenumber - 1] = Console.ReadLine();

                        }
                        else if (command == "list")
                        {
                            if (input.Length > 1)
                            {
                                //idiot-proofing
                                //checks the line number is actually an integer
                                if (int.TryParse(input[1], out int linenumber))
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
                        else if (command == "count")
                        {
                            uint i = 0;
                            foreach (string line in filecontent) i += (uint)line.Length;
                            Console.WriteLine(string.Format("{0} lines, with {1} characters.", filecontent.Count, i));
                        }

                        else if (command == "save")
                        {
                            if (input.Length > 1)
                            {
                                //user has specifed a filename, check if it already exists.
                                string filename = input[1];
                                if (File.Exists(filename))
                                {
                                    Warning(string.Format("You are about to overwrite {0}\nDo you want to overwrite? y/N ", filename), false);

                                    //read user input
                                    if (!(Console.ReadKey().Key == ConsoleKey.Y))
                                    {
                                        Console.WriteLine("\nAborted.");
                                        continue;
                                    }

                                    File.Delete(filename); //delete the file 
                                }
                                
                                //write all data to the file
                                int totalsize =  filecontent.Count;
                                int i = 1;
                                while (filecontent.Count > 0)
                                {
                                    //'pop' line from the top of filecontent
                                    string line = filecontent[0];
                                    filecontent.RemoveAt(0);
                                    //write the line
                                    Console.Write(string.Format("\r{0}/{1} {2}%   ", i, totalsize, i/totalsize*100)); //output progress and percentage
                                    i++;
                                }
                                Success("\nSaved!");
                            }
                        }
                        else if (command == "discard" || command == "exit") break; //exit the loop
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
        void Error(string mesg, bool newline = true)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (newline) Console.WriteLine(mesg);
            else Console.Write(mesg);
            Console.ResetColor();
        }
        void Warning(string mesg, bool newline = true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (newline) Console.WriteLine(mesg);
            else Console.Write(mesg);

            Console.ResetColor();
        }
        void Success(string mesg, bool newline = true)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            if (newline) Console.WriteLine(mesg);
            else Console.Write(mesg);
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

        //Properly outputs help text
        void OutputHelpText(string[] commandinfo)
        {
            Console.Write(commandinfo[0]); //output command name

            //output optional command description and spacing
            if (commandinfo.Length > 1)
            {
                Console.Write(functions.EnumerableRepeat(" ", 20 - commandinfo[0].Length)); //20 is the length of the padding.
                Console.Write(commandinfo[1]);
            }

            Console.WriteLine(); //newline regardless of if there was a command description or not
        }
    }
}