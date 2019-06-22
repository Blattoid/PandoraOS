using System;
using System.Collections.Generic;
using System.IO;
using Sys = Cosmos.System;
using Pandora.Functions;

namespace Pandora.Applets
{
    class FileUtils
    {
        private HelperFunctions func = new HelperFunctions();

        public void List(string[] maincommand)
        {
            if (!Kernel.IsVFSInit) //refuse to proceed if the VFS has not been initialised
            {
                func.Error("VFS not initialised!");
                return;
            }

            string cd;
            if (maincommand.Length > 1)
            {
                if (Directory.Exists(maincommand[1])) cd = maincommand[1];
                else
                {
                    func.Error("Directory does not exist.");
                    return;
                }
            }
            else cd = Directory.GetCurrentDirectory();
            Console.WriteLine("Directory listing for " + cd);
 
            string[] list = Directory.GetDirectories(cd);
            int foldercount = list.Length;
            func.ScrollWithPauses(list, "<DIR>  ", ConsoleColor.Magenta);
            //////
            list = Directory.GetFiles(cd);
            int filecount = list.Length;
            ulong filebytes = 0;
            foreach (string filename in list) filebytes += (ulong)new FileInfo(filename).Length; //get the size of each file and total it.
            func.ScrollWithPauses(Directory.GetFiles(cd), "<FILE> ", ConsoleColor.Green);

            Console.ResetColor();

            Console.WriteLine(string.Format("{0} file(s) totalling {1} bytes.\n{2} folder(s). {3} bytes free total.",
                filecount,filebytes, foldercount, Kernel.filesys.GetAvailableFreeSpace("0"))); //use drive id 0 since idk how to select a drive id based on the directory.
        }
        public void Move(string[] maincommand)
        {
            //idiot-proofing
            if (!Kernel.IsVFSInit) //refuse to proceed if the VFS has not been initialised
            {
                func.Error("VFS not initialised!");
                return;
            }
            if (maincommand.Length < 3) //check if filenames are given.
            {
                //if not, error out.
                func.Error("Insufficient parameters.");
                return;
            }

            string source = maincommand[1];
            string dest = maincommand[2];
            Console.WriteLine(string.Format("Source: '{0}'\nDestination: '{1}'", source, dest));

            //perform an existence check on the source and an overwrite check on the destination
            if (!File.Exists(source))
            {
                func.Error(string.Format("File not found '{0}'.", source));
                return;
            }

            if (File.Exists(dest))
            {
                func.Warning(string.Format("You are about to overwrite '{0}'.\nDo you want to overwrite? y/N ", dest), false);
                //read user input
                if (!(Console.ReadKey().Key == ConsoleKey.Y))
                {
                    Console.WriteLine("\nAborted.");
                    return;
                }
            }

            Directory.Move(source, dest); //perform the move.
        }
        public void Delete(string[] maincommand)
        {
            //idiot-proofing
            if (!Kernel.IsVFSInit) //refuse to proceed if the VFS has not been initialised
            {
                func.Error("VFS not initialised!");
                return;
            }
            if (maincommand.Length < 2) //if filename was given. if not, error out.
            {
                func.Error("Please specify a file.");
                return;
            }

            string target = maincommand[1];
            Console.WriteLine(string.Format("Target: '{0}'", target));

            //perform an existence check on the first file and an optional overwrite check on file2 if it was specified.
            if (!File.Exists(target))
            {
                func.Error(string.Format("File not found '{0}'.", target));
                return;
            }

            //perform delete
            File.Delete(maincommand[1]);
            func.Success("Deleted!");
        }
        public void Copy(string[] maincommand)
        {
            //idiot-proofing
            if (!Kernel.IsVFSInit) //refuse to proceed if the VFS has not been initialised
            {
                func.Error("VFS not initialised!");
                return;
            }
            if (maincommand.Length < 3) //check if filenames are given.
            {
                //if not, error out.
                func.Error("Insufficient parameters.");
                return;
            }

            string source = maincommand[1];
            string dest = maincommand[2];
            Console.WriteLine(string.Format("Source: '{0}'\nDestination: '{1}'", source, dest));

            //perform an existence check on the source and an overwrite check on the destination
            if (!File.Exists(source))
            {
                func.Error(string.Format("File not found '{0}'.", source));
                return;
            }

            if (File.Exists(dest))
            {
                func.Warning(string.Format("You are about to overwrite '{0}'.\nDo you want to overwrite? y/N ", dest), false);
                //read user input
                if (!(Console.ReadKey().Key == ConsoleKey.Y))
                {
                    Console.WriteLine("\nAborted.");
                    return;
                }
            }

            Console.WriteLine(string.Format("Copying {0} bytes...",new FileInfo(source).Length));
            File.Copy(source, dest);
            func.Success("Copied!");
        }
        public void Edit(string[] maincommand)
        {
            if (!Kernel.IsVFSInit) { func.Error("VFS not initialised!"); return; } //refuse to proceed if the VFS has not been initialised
            string filename = "";
            List<string> filecontents = new List<string>();

            //if a filename was specified load it.
            if (maincommand.Length > 1)
            {
                if (File.Exists(maincommand[1]))
                {
                    Console.WriteLine("Loading file...");
                    filename = maincommand[1]; //update the filename
                    filecontents.Clear(); //clear any content that might already be in the buffer.
                    foreach (string line in File.ReadAllLines(filename)) filecontents.Add(line); //add all the lines
                    func.Success(string.Format("Loaded {0} lines!", filecontents.Count));
                }
                else
                {
                    func.Error("File not found.");
                }
            }

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
                    if (filecontents.Count > 0)
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
                        filename = input[1]; //update the filename
                        filecontents.Clear(); //clear any content that might already be in the buffer.
                        foreach (string line in File.ReadAllLines(filename)) filecontents.Add(line); //add all the lines
                        func.Success(string.Format("Loaded {0} lines!", filecontents.Count));
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
                    while (filecontents.Count < linenumber) filecontents.Add("");

                    //Get the new content for the line 
                    Console.Write("? ");
                    filecontents[linenumber - 1] = Console.ReadLine();

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

                        if (linenumber > filecontents.Count)
                        {
                            //the specified line exceeds the line count of the file
                            func.Error("Out of range.");
                            continue;
                        }

                        //output the line they requested.
                        Console.WriteLine(filecontents[linenumber - 1]);
                    }
                    else
                    {
                        //Simply output every single line, lmao
                        foreach (string line in filecontents) Console.WriteLine(line);
                    }
                }
                else if (command == "count")
                {
                    uint i = 0;
                    foreach (string line in filecontents) i += (uint)line.Length;
                    Console.WriteLine(string.Format("{0} lines, with {1} characters.", filecontents.Count, i));
                }

                else if (command == "save")
                {
                    if (input.Length > 1)
                    {
                        //user has specifed a filename, check if it already exists.
                        filename = input[1];
                        SaveFile(filename, filecontents);
                    }
                    else
                    {
                        //perhaps they just want to save to an already specified file. if so, check if a name was at one point specfied
                        if (filename.Length > 0)
                        {
                            SaveFile(filename, filecontents);
                        }
                    }
                }
                else if (command == "discard" || command == "exit") break; //exit the loop
                else func.Error("Unknown command. Type 'help' for a list of editor commands.");
            }
        }
        public void Type(string[] maincommand)
        {
            //idiot-proofing
            if (!Kernel.IsVFSInit) //refuse to proceed if the VFS has not been initialised
            {
                func.Error("VFS not initialised!");
                return;
            }
            if (maincommand.Length < 2) //if filename was given. if not, error out.
            {
                func.Error("Please specify a file.");
                return;
            }

            string target = maincommand[1];

            //perform an existence check on the first file and an optional overwrite check on file2 if it was specified.
            if (!File.Exists(target))
            {
                func.Error(string.Format("File not found '{0}'.", target));
                return;
            }

            //perform the read, pausing on every screenful
            func.ScrollWithPauses(File.ReadAllLines(maincommand[1]));
        }
        public void Size(string[] maincommand)
        {
            //idiot-proofing
            if (!Kernel.IsVFSInit) //refuse to proceed if the VFS has not been initialised
            {
                func.Error("VFS not initialised!");
                return;
            }
            if (maincommand.Length < 2) //if filename was given. if not, error out.
            {
                func.Error("Please specify a file.");
                return;
            }

            string target = maincommand[1];
            Console.WriteLine(string.Format("Target: '{0}'", target));

            //perform an existence check on the first file and an optional overwrite check on file2 if it was specified.
            if (!File.Exists(target))
            {
                func.Error(string.Format("File not found '{0}'.", target));
                return;
            }

            int lines = 0, chars = 0;

            foreach (string line in File.ReadAllLines(target)) //perform the read.
            {
                lines++;
                chars += line.Length;
            }
            Console.WriteLine(String.Format("{0} lines, {1} characters. ({2} bytes)", lines, chars, new FileInfo(target).Length));
        }

        private void SaveFile(string filename, List<string> contents)
        {
            if (File.Exists(filename))
            {
                func.Warning(string.Format("You are about to overwrite {0}.\nDo you want to overwrite? y/N ", filename), false);

                //read user input
                if (!(Console.ReadKey().Key == ConsoleKey.Y))
                {
                    Console.WriteLine("\nAborted.");
                    return;
                }

                File.Delete(filename); //delete the file 
            }

            //write all data to the file
            int totalsize = contents.Count;
            int i = 1;
            while (contents.Count > 0)
            {
                //'pop' line from the top of filecontent
                string line = contents[0];
                contents.RemoveAt(0);
                //write the line
                File.AppendAllText(filename, line + "\n");
                Console.Write(string.Format("\r{0}/{1} {2}%   ", i, totalsize, i / totalsize * 100)); //output progress and percentage
                i++; //keep track of lines for the output
            }
            func.Success("\nSaved!");
        }
    }
}