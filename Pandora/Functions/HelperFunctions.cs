using System;

namespace Pandora.Functions
{
    class HelperFunctions
    {
        private  MissingFunctions missingfunctions = new MissingFunctions();
        
        //colour-coded message printing
        public void Error(string mesg, bool newline = true)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (newline) Console.WriteLine(mesg);
            else Console.Write(mesg);
            Console.ResetColor();
        }
        public void Warning(string mesg, bool newline = true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (newline) Console.WriteLine(mesg);
            else Console.Write(mesg);

            Console.ResetColor();
        }
        public void Success(string mesg, bool newline = true)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            if (newline) Console.WriteLine(mesg);
            else Console.Write(mesg);
            Console.ResetColor();
        }

        //equivalent of 'less' in unix
        public void ScrollWithPauses(string[] scrolltext, string messageprefix = "", ConsoleColor textcolour = ConsoleColor.White)
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
        public void OutputHelpText(string[] commandinfo)
        {
            Console.Write(commandinfo[0]); //output command name

            //output optional command description and spacing
            if (commandinfo.Length > 1)
            {
                Console.Write(missingfunctions.EnumerableRepeat(" ", 20 - commandinfo[0].Length)); //20 is the length of the padding.
                Console.Write(commandinfo[1]);
            }

            Console.WriteLine(); //newline regardless of if there was a command description or not
        }
    }
}
