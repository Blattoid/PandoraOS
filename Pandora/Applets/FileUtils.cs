using System;
using System.Collections.Generic;
using System.IO;
using Sys = Cosmos.System;

namespace Pandora.Applets
{
    class FileUtils
    {
        private  HelperFunctions func = new HelperFunctions();

        public void InitVFS()
        {
            if (Kernel.IsVFSInit)
            {
                func.Error("VFS is already initialised. Reboot if you want to unload it.");
                return;
            }

            //display func.func.Warning about possible data corruption
            func.Error("-=!!Warning!!=-\nThe CosmosOS FAT driver is still in experimental stages.\nPROCEEDING MAY CAUSE A LOSS OF DATA!");
            func.Warning("Initialise anyway? y/N ", false);

            //read user input
            if (!(Console.ReadKey().Key == ConsoleKey.Y))
            {
                Console.WriteLine("\nAborted.");
                return;
            }

            Kernel.filesys = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(Kernel.filesys);
            func.Success("Initialised VFS.");
            Kernel.IsVFSInit = true;
        }
        public void List()
        {
            if (!Kernel.IsVFSInit) { func.Error("VFS not initialised!"); return; } //refuse to proceed if the VFS has not been initialised
            string cd = Directory.GetCurrentDirectory();
            Console.WriteLine("Directory listing for " + cd);

            func.ScrollWithPauses(Directory.GetDirectories(cd), "<DIR>  ", ConsoleColor.Magenta);
            func.ScrollWithPauses(Directory.GetFiles(cd), "<FILE> ", ConsoleColor.Green);
            Console.ResetColor();
        }
        public void Edit()
        {
            if (!Kernel.IsVFSInit) { func.Error("VFS not initialised!"); return; } //refuse to proceed if the VFS has not been initialised

            List<string> filecontent = new List<string>();
            Console.WriteLine("-=File Editor=-");
            for (; ; )
            {
                //read user command
                Console.Write("EDIT:"); //command prefix
                string[] input = Console.ReadLine().Split(" "); //split by spaces
                string command = input[0].ToLower(); //grab lowercase of command

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
                    ) func.OutputHelpText(line);
                }
                else if (command == "load")
                {
                    //check if a filename was specified.
                    if (input.Length < 2)
                    {
                        func.Error("No filename was specified.");
                        continue;
                    }

                    //check if we are about to overwrite any unsaved work
                    if (filecontent.Count > 0)
                    {
                        func.Warning("You are about to discard your unsaved work.\nProceed? y/N ", false);

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
                        func.Success(string.Format("Loaded {0} lines!", filecontent.Count));
                    }
                    else
                    {
                        func.Error("File not found.");
                    }
                }

                else if (command == "line")
                {
                    //idiot-proofing
                    //parameter count checks
                    if (input.Length < 2)
                    {
                        func.Error("Insufficient parameters.");
                        continue;
                    }
                    else if (input.Length > 3) func.Warning("Excessive parameters, proceeding anyway.");
                    //checks the line number is actually an integer
                    if (int.TryParse(input[1], out int linenumber))
                    {
                        //it is an integer, is it less than 1?
                        if (linenumber < 1)
                        {
                            func.Error("Line number is less than 1.");
                            continue;
                        }
                    }
                    else
                    {
                        //it is not an integer
                        func.Error("Invalid line number.");
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
                                func.Error("Line number is less than 1.");
                                continue;
                            }
                        }
                        else
                        {
                            //it is not an integer
                            func.Error("Invalid line number.");
                            continue;
                        }

                        if (linenumber > filecontent.Count)
                        {
                            //the specified line exceeds the line count of the file
                            func.Error("Out of range.");
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
                            func.Warning(string.Format("You are about to overwrite {0}\nDo you want to overwrite? y/N ", filename), false);

                            //read user input
                            if (!(Console.ReadKey().Key == ConsoleKey.Y))
                            {
                                Console.WriteLine("\nAborted.");
                                continue;
                            }

                            File.Delete(filename); //delete the file 
                        }

                        //write all data to the file
                        int totalsize = filecontent.Count;
                        int i = 1;
                        while (filecontent.Count > 0)
                        {
                            //'pop' line from the top of filecontent
                            string line = filecontent[0];
                            filecontent.RemoveAt(0);
                            //write the line
                            Console.Write(string.Format("\r{0}/{1} {2}%   ", i, totalsize, i / totalsize * 100)); //output progress and percentage
                            i++;
                        }
                        func.Success("\nSaved!");
                    }
                }
                else if (command == "discard" || command == "exit") break; //exit the loop
                else func.Error("Unknown command. Type 'help' for a list of editor commands.");
            }
        }
    }
}
