using System;
using Sys = Cosmos.System;

namespace Pandora
{
    public class Kernel : Sys.Kernel
    {
        public const double SYS_VERSION = 0.33;
        public static bool IsVFSInit = false; //Has the VFS been initialised? (needed for any disk access functions)
        public static Sys.FileSystem.CosmosVFS filesys;

        //These classes contain functions and applets that we need/execute.
        //An 'Applet' is a command that can be run from the main menu.
        MissingFunctions missingfunctions;
        HelperFunctions func;
        Applets.FileUtils Applets_Files;
        Applets.Misc Applets_Misc; 
 
        protected override void BeforeRun()
        {
            //at this point, our code is executing. print a message to inform the user of this.
            func.Success("Kernel execution started.");

            //cute startup beep tune :)
            for (uint i = 600; i <= 1000; i += 200) Sys.PCSpeaker.Beep(i, 200);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Screen res is " + Console.WindowWidth + "x" + Console.WindowHeight + ".");

            for (uint i = 0; i <= 10000000; i += 1) { } //false delay
            missingfunctions = new MissingFunctions();
            Console.WriteLine("Loaded 'MissingFunctions'.");
            //////////////////////////
            for (uint i = 0; i <= 10000000; i += 1) { } //false delay
            func =  new HelperFunctions();
            Console.WriteLine("Loaded 'HelperFunctions'.");
            //////////////////////////
            for (uint i = 0; i <= 50000000; i += 1) { } //false delay
            Applets_Files = new Applets.FileUtils();
            Console.WriteLine("Loaded 'Applets.FileUtils'.");
            //////////////////////////
            for (uint i = 0; i <= 20000000; i += 1) { } //false delay
            Applets_Misc = new Applets.Misc();
            Console.WriteLine("Loaded 'Applets.Misc'.");

            Console.ResetColor();

            //func.Success(string.Format("-=PandoraOS V{0} booted successfully=-", SYS_VERSION));
            Console.WriteLine(string.Format("-=PandoraOS V{0} booted successfully=-", SYS_VERSION));
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
                                new string[] { "move <file> <new>", "Renames a file to a new path." },
                                new string[] { "rename <file> <new>", "Renames a file to a new filename." },
                                new string[] { "delete <file>", "Deletes a specfied file." },
                                new string[] { "copy <file> <new>", "Copies a file to a new filename." },
                                new string[] { "edit [file]", "Allows rudimentary file editing." },
                                new string[] { "type <file>", "Outputs the content of a file." },
                                new string[] { "size <file>", "Outputs the size of a file." },
                                new string[] {""},
                                new string[] {"reboot","Restarts the system." },
                                new string[] {"shutdown","Turns the system off." }
                        }
                    ) func.OutputHelpText(line);
                }
                else if (command == "memopad") Applets_Misc.Memopad();

                else if (command == "init_vfs" || command == "!") Applets_Files.InitVFS();
                else if (command == "list" || command == "ls") Applets_Files.List(input);
                else if (command == "move" || command == "rename") Applets_Files.Move(input);
                else if (command == "delete" || command == "del") Applets_Files.Delete(input);
                else if (command == "copy" || command == "cp") Applets_Files.Copy(input);
                else if (command == "edit") Applets_Files.Edit(input);
                else if (command == "type" || command == "cat") Applets_Files.Type(input);
                else if (command == "size" || command == "du") Applets_Files.Size(input);

                else if (command == "reboot") Sys.Power.Reboot();
                else if (command == "shutdown") Sys.Power.Shutdown();
                else func.Error("Unknown command. Type 'help' for a list of commands.");
            }
            catch (Exception err) { Console.WriteLine(err.Message); } //{ func.Error(err.Message); }
        }
    }
}