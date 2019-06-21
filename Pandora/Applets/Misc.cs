using System;
using Pandora.Functions;

namespace Pandora.Applets
{
    class Misc
    {
        HelperFunctions func = new HelperFunctions();

        public void Help(string[] argv)
        {
            foreach (AppletIndex.App app in Kernel.appletIndex.GenAppletIndex())
            {
                func.OutputHelpText(new string[] { app.HelpTitle, app.HelpDescription });
            }
        }
        public void Memopad(string[] argv)
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
                //bool SHIFT = false;
                if ((key.Modifiers & ConsoleModifiers.Alt) != 0) ALT = true;
                //if ((key.Modifiers & ConsoleModifiers.Shift) != 0) SHIFT = true;

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
    }
}
