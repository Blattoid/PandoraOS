using System;
using System.Collections.Generic;
using System.Text;

namespace Pandora
{
    //I've noticed that some things have not been implemented in Cosmos that I need, so these are workarounds for the problem.
    class MissingFunctions
    {
        //Convert.ToInt32 doesn't work, so I have to implement it myself in such a way that it does work. It will not be efficient.
        public int ConvertToInt(string input)
        {
            Int32 output = 0;
            int multiplier = 1;
            for (int i = input.Length - 1; i >= 0; i -= 1)
            {
                int x = 0;
                string y = input[i].ToString();
                switch (y)
                {
                    case "0":
                        x = 0;
                        break;
                    case "1":
                        x = 1;
                        break;
                    case "2":
                        x = 2;
                        break;
                    case "3":
                        x = 3;
                        break;
                    case "4":
                        x = 4;
                        break;
                    case "5":
                        x = 5;
                        break;
                    case "6":
                        x = 6;
                        break;
                    case "7":
                        x = 7;
                        break;
                    case "8":
                        x = 8;
                        break;
                    case "9":
                        x = 9;
                        break;
                }
                output += x * multiplier;
                multiplier *= 10;
            }
            return output;
        }
    }
}
