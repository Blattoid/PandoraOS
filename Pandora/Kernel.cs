using System;
using System.IO;
using Sys = Cosmos.System;

namespace Pandora
{
    public class Kernel : Sys.Kernel
    {
        protected override void BeforeRun()
        {
            Console.WriteLine("Cosmos booted successfully. Type a line of text to get it echoed back.");
        }

        protected override void Run()
        {
            for (; ; )
            {
                Console.Write("Enter text: ");
                string input = Console.ReadLine();
                Console.WriteLine("You entered: "+input);
            }
        }
    }
}
