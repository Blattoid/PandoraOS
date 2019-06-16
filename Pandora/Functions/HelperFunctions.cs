using System;
using System.Collections.Generic;

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

        /// <summary>Given a string, separate it into an array of arguments.</summary>
        /// <param name="text">The input text to parse.</param>
        /// <param name="delimiter">The character to split the string by, usually whitespace.</param>
        public string[] SeparateStringIntoArguments(string text, char delimiter = ' ')
        {
            List<string> arguments = new List<string>();
            int cursor_pos = 0; //stores where we are in the string
            string current_arg = ""; //hold the current argument being worked on
            bool escape_char = false;

            foreach (char character in text) //iterate over every character in text
            {
                if (character == '\\' && !escape_char) //do we need to escape the next character AND we aren't supposed to escape this one?
                {
                    //yes, set the flag and move onto the next character.
                    escape_char = true;
                    continue;
                }

                if (character == delimiter && !escape_char) //is this character a match against our delimiter AND we aren't supposed to escape this one?
                {
                    if (current_arg.Length == 0) continue; //if we do not have an argument to add, do not proceed.
                    arguments.Add(current_arg);
                    cursor_pos += current_arg.Length; //put the 'cursor' at where our argument ends in the text
                    current_arg = "";
                }
                else current_arg += character; //add the character to the current argument.

                escape_char = false; //we no longer need to escape a character.
            }
            arguments.Add(current_arg); //add the last argument to the list

            //copy the arguments to a string array, since those are more stable in COSMOS.
            string[] args = new string[arguments.Count];
            for (int i = 0; i < arguments.Count; i++) args[i] = arguments[i];
            foreach (string arg in args) { Console.WriteLine("\t" + arg); } //debug output
            return args;
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
