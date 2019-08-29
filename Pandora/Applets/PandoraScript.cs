using System;
using System.Collections.Generic;
using Pandora.Functions;

namespace Pandora.Applets
{
    class PandoraScript
    {
        HelperFunctions func = new HelperFunctions();

        bool proceed = true; //should we continue execution of the script?

        public void RunFromArray(List<string> script)
        {
            for (int ProgramCounter = 0; ProgramCounter < script.Count; ProgramCounter++) //use a 'for' loop for the program counter. this enables altering the program counter
            {
                if (!proceed) break;

                string Line = script[ProgramCounter]; //read the line from the array
                if (Line.Length != 0) //ignore blank lines
                {
                    if (Line.StartsWith("#")) continue; //ignore comments

                    string[] Tokens = func.SeparateStringIntoArguments(Line,';'); //semicolons separate arguments in commands.
                    Tokens[0] = Tokens[0].ToUpper();

                    if (Tokens[0] == "PRINTLN")
                    {
                        if (Tokens.Length < 1) { RuntimeError(ProgramCounter, "Insufficient parameters to PRINTLN"); }
                        else Console.WriteLine(Tokens[1]);
                    }
                    else if (Tokens[0] == "PRINT")
                    {
                        if (Tokens.Length < 1) { RuntimeError(ProgramCounter, "Insufficient parameters to PRINT"); }
                    }
                    else { RuntimeError(ProgramCounter, string.Format("Unknown command '{0}'", Tokens[0])); }
                }
            }
            Console.WriteLine("Program end.");
        }
        private void RuntimeError(int ProgramCounter, string Reason)
        {
            func.Error(string.Format("RuntimeError at line {0}.\n{1}", ProgramCounter, Reason));
            proceed = false; //terminate execution
        }
    }
}
