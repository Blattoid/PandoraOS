using System;
using System.Threading;
using Sys = Cosmos.System;

namespace Pandora
{
    public class Kernel : Sys.Kernel
    {
        public const double SYS_VERSION = 0.32;
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
            missingfunctions = new MissingFunctions();
            Console.WriteLine("Loaded 'MissingFunctions'.");
            //////////////////////////
            func =  new HelperFunctions();
            Console.WriteLine("Loaded 'HelperFunctions'.");
            //////////////////////////
            Applets_Files = new Applets.FileUtils();
            Console.WriteLine("Loaded 'Applets.FileUtils'.");
            //////////////////////////
            Applets_Misc = new Applets.Misc();
            Console.WriteLine("Loaded 'Applets.Misc'.");

            Console.WriteLine("Screen res is " + Console.WindowWidth + "x" + Console.WindowHeight + ".");
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
                                new string[] { "edit", "Allows rudimentary file editing." },
                                new string[] {""},
                                new string[] {"reboot","Restarts the system." },
                                new string[] {"shutdown","Turns the system off." }
                        }
                    ) func.OutputHelpText(line);
                }
                else if (command == "memopad") Applets_Misc.Memopad();

                else if (command == "init_vfs") Applets_Files.InitVFS();
                else if (command == "list") Applets_Files.List();
                else if (command == "edit") Applets_Files.Edit();

                else if (command == "reboot") Sys.Power.Reboot();
                else if (command == "shutdown") Sys.Power.Shutdown();
                else func.Error("Unknown command. Type 'help' for a list of commands.");
            }
            catch (Exception err) { Console.WriteLine(err.Message); } //{ func.Error(err.Message); }
        }
    }
}