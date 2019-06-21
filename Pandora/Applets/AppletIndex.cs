using System;
using Pandora.Functions;

namespace Pandora.Applets
{
    public class AppletIndex
    {
        private static HelperFunctions func = new HelperFunctions();

        private Misc misc = new Misc();
        FileUtils fileUtils = new FileUtils();

        public App[] GenAppletIndex() //public app index to reference any app in the system
        {
            //initialise app index
            //MODIFY THIS TO ADD YOUR OWN COMMAND INTO THE KERNEL
            return new App[]
            {
                new App {
                    CommandInvokers = new string[] {"help", "?"},
                    HelpTitle = "help",
                    HelpDescription = "Displays this help",
                    AppCode_Delegate = misc.Help
                },
                new App {
                    CommandInvokers = new string[] {"memopad"},
                    HelpTitle = "memopad",
                    HelpDescription = "Allows you to write anywhere on the screen.",
                    AppCode_Delegate = misc.Memopad
                },
                new App {
                    CommandInvokers = new string[] { "init_vfs", "!"},
                    HelpTitle = "init_vfs",
                    HelpDescription = "Initialises the Virtual Filesystem Manager.",
                    AppCode_Delegate = fileUtils.InitVFS
                },
                new App {
                    CommandInvokers = new string[] { "list", "ls"},
                    HelpTitle = "list",
                    HelpDescription = "Lists the files in the current directory.",
                    AppCode_Delegate = fileUtils.List
                },
                new App {
                    CommandInvokers = new string[] { "move", "mv"},
                    HelpTitle = "move <file> <new>",
                    HelpDescription = "Renames a file to a new path.",
                    AppCode_Delegate = fileUtils.Move
                },
                new App {
                    CommandInvokers = new string[] { "rename"},
                    HelpTitle = "rename <file> <new>",
                    HelpDescription = "Renames a file to a new filename.",
                    AppCode_Delegate = fileUtils.Move //rename is just 'move' in disguise :D
                },
                new App {
                    CommandInvokers = new string[] { "delete", "rm"},
                    HelpTitle = "delete <file>",
                    HelpDescription = "Deletes a specfied file.",
                    AppCode_Delegate = fileUtils.Delete
                },
                new App {
                    CommandInvokers = new string[] { "copy" ,"cp"},
                    HelpTitle = "copy <file> <new>",
                    HelpDescription = "Copies a file to a new filename.",
                    AppCode_Delegate = fileUtils.Copy
                },
                new App {
                    CommandInvokers = new string[] { "edit"},
                    HelpTitle = "edit [file]",
                    HelpDescription = "Allows rudimentary file editing.",
                    AppCode_Delegate = fileUtils.Edit
                },
                new App {
                    CommandInvokers = new string[] { "type", "cat"},
                    HelpTitle = "type <file>",
                    HelpDescription = "Outputs the content of a file.",
                    AppCode_Delegate = fileUtils.Type
                },
                new App {
                    CommandInvokers = new string[] { "size", "du"},
                    HelpTitle = "size <file>",
                    HelpDescription = "Outputs the size of a file.",
                    AppCode_Delegate = fileUtils.Size
                }
            };
        }

        //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/how-to-initialize-a-dictionary-with-a-collection-initializer
        public class App
        {
            public string[] CommandInvokers { get; set; } = new string[] { "" }; //commands that can reference the app (e.g. 'list' would work the same as 'ls')
            public AppRuntime AppCode_Delegate { get; set; } = MissingAppletError; //delegate to the function that runs the app code

            //shown in the 'help' command
            public string HelpTitle { get; set; } = "";
            public string HelpDescription { get; set; } = "";

            //called by the kernel to initiate that applet's code
            public void Run(string[] argv) { AppCode_Delegate(argv); }
        }
        public delegate void AppRuntime(string[] argv);
        private static void MissingAppletError(string[] argv) { func.Error("Undefined Applet!"); }
    }
}
