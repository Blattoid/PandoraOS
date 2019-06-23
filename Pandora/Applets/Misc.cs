using System;
using System.Collections.Generic;
using Pandora.Functions;

namespace Pandora.Applets
{
    class Misc
    {
        HelperFunctions func = new HelperFunctions();
        MissingFunctions missingfunctions = new MissingFunctions();

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

            //initialise buffer
            List<List<char>> buffer = new List<List<char>>(); //y, x
            buffer.Capacity = Console.WindowHeight;
            for (int i = 1; i <= Console.WindowHeight; i++)
            {
                List<char> line = new List<char>();
                line.Capacity = Console.WindowWidth;
                for (int j = 0; j <= Console.WindowWidth; j++)
                {
                    if (i == 1 && j < 19) line.Add("Press ALT-C to exit."[j]);
                    else line.Add(' ');
                }
                buffer.Add(line);
            }

            int[] cursor_pos = new int[2] { 1, 0 }; //y, x

            //Console.WriteLine("buffer initialised.");
            //Console.WriteLine("y:"+buffer.Count+" x:"+buffer[0].Count);

            for (; ; )
            {
                //copy buffer to the screen
                for (int y = 0; y < buffer.Count - 1; y++)
                {
                    for (int x = 0; x < buffer[y].Count - 1; x++)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(buffer[y][x]);
                    }
                }

                Console.SetCursorPosition(cursor_pos[1], cursor_pos[0]); //update cursor position
                key = Console.ReadKey(true); //read and intercept user input

                //update modifier states
                bool ALT = false;
                if ((key.Modifiers & ConsoleModifiers.Alt) != 0) ALT = true;
                //bool SHIFT = false;
                //if ((key.Modifiers & ConsoleModifiers.Shift) != 0) SHIFT = true;

                if (ALT && key.KeyChar == 'c') break; //exit program

                else if (key.Key == ConsoleKey.Backspace) //backspace implementation
                {
                    cursor_pos[1]--; //decrement x axis
                    if (cursor_pos[1] < 0) //check it doesn't go below 0.
                    {
                        //if it does, loop around to the end of the previous line
                        cursor_pos[1] = Console.WindowWidth - 1;
                        cursor_pos[0]--; //decrement y axis
                        if (cursor_pos[0] < 0) cursor_pos[0] = Console.WindowHeight - 1; //if the cursor goes above the buffer, loop around to the bottom.
                    }
                    buffer[cursor_pos[0]][cursor_pos[1]] = ' '; //overwrite that character with empty space.
                }
                else if (key.Key == ConsoleKey.LeftArrow) //left arrow implementation
                {
                    cursor_pos[1]--; //decrement x axis
                    if (cursor_pos[1] < 0) //check it doesn't go below 0.
                    {
                        //if it does, loop around to the end of the previous line
                        cursor_pos[1] = Console.WindowWidth - 1;
                        cursor_pos[0]--; //decrement y axis
                        if (cursor_pos[0] < 0) cursor_pos[0] = Console.WindowHeight - 1; //if the cursor goes above the buffer, loop around to the bottom.
                    }
                }
                else if (key.Key == ConsoleKey.RightArrow) //right arrow implementation
                {
                    cursor_pos[1]++; //increment x axis
                    if (cursor_pos[1] >= Console.WindowWidth) //check it doesn't exceed window width
                    {
                        //if it does, loop around to the start of the next line
                        cursor_pos[1] = 0;
                        cursor_pos[0]++; //increment y axis
                        if (cursor_pos[0] < 0) cursor_pos[0] = Console.WindowHeight - 1; //if the cursor goes above the buffer, loop around to the bottom.
                    }
                }
                else if (key.Key == ConsoleKey.UpArrow) //up arrow implementation
                {
                    cursor_pos[0]--; //decrement y axis
                    if (cursor_pos[0] < 0) cursor_pos[0] = Console.WindowHeight - 1; //if the cursor goes above the buffer, loop around to the bottom.
                }
                else if (key.Key == ConsoleKey.DownArrow) //down arrow implementation
                {
                    cursor_pos[0]++; //increment y axis
                    if (cursor_pos[0] >= Console.WindowHeight) cursor_pos[0] = 0; //if the cursor goes below the buffer, loop around to the top.
                }
                else if (key.Key == ConsoleKey.Home) cursor_pos[1] = 0;
                else if (key.Key == ConsoleKey.End) cursor_pos[1] = Console.WindowWidth - 1;
                else if (key.Key == ConsoleKey.PageUp) cursor_pos[0] = 0;
                else if (key.Key == ConsoleKey.PageDown) cursor_pos[0] = Console.WindowHeight - 1;
                else if (key.Key == ConsoleKey.Enter)
                {
                    cursor_pos[1] = 0; //go to the start of the line
                    cursor_pos[0]++; //increment y axis
                    if (cursor_pos[0] >= Console.WindowHeight) cursor_pos[0] = 0; //if the cursor goes below the buffer, loop around to the top.
                }

                else
                {
                    //it wasn't an arrow key, so write the character to the buffer.
                    buffer[cursor_pos[0]][cursor_pos[1]] = key.KeyChar;

                    cursor_pos[1]++; //increment x axis
                    if (cursor_pos[1] >= Console.WindowWidth) //check it doesn't exceed window width
                    {
                        //if it does, loop around to the start of the next line
                        cursor_pos[1] = 0;
                        cursor_pos[0]++; //increment y axis
                        if (cursor_pos[0] < 0) cursor_pos[0] = Console.WindowHeight - 1; //if the cursor goes above the buffer, loop around to the bottom.
                    }
                }
            }
            Console.WriteLine(); //write a new line on applet exit.
        }
    }
}
