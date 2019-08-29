//#define STARTUP_BEEPS
#define AUTOLOAD_VFS

using System;
using Sys = Cosmos.System;
using Pandora.Applets;
using Pandora.Functions;

namespace Pandora
{
    public class Kernel : Sys.Kernel
    {
        public const double SYS_VERSION = 0.4;
        public static bool IsVFSInit = false; //Has the VFS been initialised? (needed for any disk access functions)
        public static Sys.FileSystem.CosmosVFS vfs;

        //These classes contain functions that we need.
        HelperFunctions func = new HelperFunctions();

        //Applet index instance
        public static AppletIndex appletIndex;

        protected override void BeforeRun()
        {
            //at this point, our code is executing. print a message to inform the user of this.
            func.Success("Kernel execution started.");

            //cute startup beep tune :)
#if (STARTUP_BEEPS)
            for (uint i = 600; i <= 1000; i += 200) Sys.PCSpeaker.Beep(i, 200);
#endif

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Screen res is " + Console.WindowWidth + "x" + Console.WindowHeight + ".");

            appletIndex = new AppletIndex();
            Console.WriteLine("Loaded App Register.");

#if (AUTOLOAD_VFS)
            vfs = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(vfs);
            Console.WriteLine("Initialised VFS.");
            IsVFSInit = true;
#endif

            func.Success(string.Format("-=PandoraOS V{0} booted successfully=- {1}MB free!", SYS_VERSION, Cosmos.Core.CPU.GetAmountOfRAM()));
        }

        protected override void Run()
        {
            try
            {
                //read user command
                Console.Write(">"); //line prefix
                string text = Console.ReadLine();
                string[] input = func.SeparateStringIntoArguments(text); //split by spaces
                string command = input[0].ToLower(); //grab lowercase of command

                //search each app in the index to see if we need to execute it
                bool executed = false;
                foreach (var app in appletIndex.GenAppletIndex())
                {
                    if (func.DoesStringArrayHaveElement(app.CommandInvokers, command))
                    {
                        executed = true;
                        app.AppCode_Delegate(input);
                    }
                }
                if (!executed) func.Error("Unknown command. Type 'help' for a list of commands.");
            }
            catch (Exception err) { func.Error("System Error: " + err.Message); }
        }
    }
}