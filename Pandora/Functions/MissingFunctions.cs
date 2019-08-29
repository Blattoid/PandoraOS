using System;
using System.Text;

namespace Pandora.Functions
{
    //I've noticed that some things have not been implemented in Cosmos that I need, so these are some workarounds for those problems.
    class MissingFunctions
    {
        //Convert.ToInt32 doesn't work, so I have to implement it myself.
        public int ConvertToInt(string input)
        {
            var throwerror = new Action(() => { throw new FormatException("Input string was not in the correct format."); });

            int multiplier = 1;
            int result = 0;
            bool is_negative = false; //is the number negative?

            byte[] data = Encoding.ASCII.GetBytes(input);
            for (int i = data.Length - 1; i >= 0; i--) //loop over the digits in reverse so we process the 1's digit first.
            {
                int code = (int)data[i]; //convert the byte to an integer.

                if (code == 45) //hyphon check
                {
                    if (i == 0) //if the input starts with a hyphon, the number is negative.
                    {
                        is_negative = true; //set the flag.
                    }
                    //but if the hyphon appears later in the string, the string is malformed.
                    else throwerror();
                }

                //matches if the character is not between 0 and 9 and isn't a hyphon.
                // 48 is decimal in ascii for 0, and similar is true for 57 meaning 9.
                else if (!(code >= 48 && code <= 57)) throwerror();  //character code exceeds range - throw an error.

                if (is_negative)
                {
                    result -= (result * 2); //make the result a negative integer by subtracting itself twice.
                }
                else
                {
                    //the code has been validated to be a digit - convert it to an integer.
                    //the conversion from ascii decimal --> integer is to just subtract 48.
                    code -= 48;
                    //then we multiply it by our multiplier to it is the correct place.
                    code *= multiplier;
                    //finally, add it to the result.
                    result += code;
                }
                multiplier *= 10; //multiply the multiplier by 10 since we are about to process the next digit.
            }
            return result; //done!
        }
        //There is no existing function to convert a string to a byte array.
        public byte[] ConvertToByteArray(string input)
        {
            byte[] result = new byte[] { }; //initialise the array
            int i = 0;
            //iterate over every character, cast it to a byte and add it to the array
            foreach (char character in input)
            {
                result[i] = (byte)character;
            }
            return result; //done!
        }

        //Because System.Linq isn't supported, Enumerable.Repeat also doesn't exist.
        public string EnumerableRepeat(string repeatthis, int repeatcount)
        {
            string output = "";
            int i = 0;
            while (i < repeatcount)
            {
                output += repeatthis;
                i++;
            }
            return output;
        }
    }
}
