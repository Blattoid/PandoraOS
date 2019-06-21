using System;
using System.Collections.Generic;

namespace Pandora.Functions
{
    class HelperFunctions
    {
        private  MissingFunctions missingfunctions = new MissingFunctions();
        
        //colour-coded message printing
        /// <summary>
        /// Outputs a given string in red.
        /// </summary>
        /// <param name="mesg">Text to output</param>
        /// <param name="newline"></param>
        public void Error(string mesg, bool newline = true)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (newline) Console.WriteLine(mesg);
            else Console.Write(mesg);
            Console.ResetColor();
        }
        /// <summary>
        /// Outputs a given string in yellow.
        /// </summary>
        /// <param name="mesg">Text to output</param>
        /// <param name="newline"></param>
        public void Warning(string mesg, bool newline = true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (newline) Console.WriteLine(mesg);
            else Console.Write(mesg);

            Console.ResetColor();
        }
        /// <summary>
        /// Outputs a given string in green.
        /// </summary>
        /// <param name="mesg">Text to output</param>
        /// <param name="newline"></param>
        public void Success(string mesg, bool newline = true)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            if (newline) Console.WriteLine(mesg);
            else Console.Write(mesg);
            Console.ResetColor();
        }

        /// <summary>
        /// Given a string, separate it into an array of arguments.
        /// </summary>
        /// <param name="text">The input text to parse.</param>
        /// <param name="delimiter">The character to split the string by, usually whitespace.</param>
        public string[] SeparateStringIntoArguments(string text, char delimiter = ' ')
        {
            List<string> arguments = new List<string>();
            int cursor_pos = 0; //stores where we are in the string
            string current_arg = ""; //hold the current argument being worked on
            bool escape_char = false;

            if (text.Length == 0) return new string[] { "" };
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
            return args;
        }

        /// <summary>
        /// Given an array of strings (one per line of text), prints a screenful of text and then waits for input before displaying the next screenful.
        /// </summary>
        /// <param name="scrolltext">Array of strings, one for each line of text to output</param>
        /// <param name="messageprefix">Optional prefix for each line in the array</param>
        /// <param name="textcolour">Optional foreground colour for the text</param>
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

        /// <summary>
        /// Custom function to output help documentation with padding.
        /// </summary>
        /// <param name="documentation">Array with two elements. Element 0 is the left text, element 1 is the right text.</param>
        /// <param name="padding">Length of padding. Default is 20</param>
        public void OutputHelpText(string[] documentation, int padding = 20)
        {
            Console.Write(documentation[0]); //output command name

            //output optional command description and spacing
            if (documentation.Length > 1)
            {
                Console.Write(missingfunctions.EnumerableRepeat(" ", 20 - documentation[0].Length)); //20 is the length of the padding.
                Console.Write(documentation[1]);
            }

            Console.WriteLine(); //newline regardless of if there was a command description or not
        }

        /// <summary>
        /// Searches a string array for a string. 
        /// </summary>
        /// <param name="array">The array to search</param>
        /// <param name="searchelement">The string to match against the array</param>
        /// <param name="case_sensitive">Optional argument for case sensitivity. Default is false</param>
        /// <param name="appears_anywhere">Optional argument for if the string is allowed to match anywhere in an array element</param>
        /// <returns></returns>
        public bool DoesStringArrayHaveElement(string[] array, string searchelement, bool case_sensitive = false, bool appears_anywhere = false)
        {
            if (!case_sensitive) searchelement = searchelement.ToLower();
            foreach (string x in array)
            {
                string element = x;
                if (!case_sensitive) element = element.ToLower(); //case insensitivity

                if (element == searchelement) return true; //match if element and search term are exactly equal
                else if (element.Contains(searchelement) && appears_anywhere) return true; //okay, does it appear anywhere?
            }
            return false;
        }
    }
}
