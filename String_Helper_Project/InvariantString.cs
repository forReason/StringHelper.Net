using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace String_Helper_Project
{
    public class InvariantString
    {
        public string InvaryString(string input, bool toLower = false, bool trim = true, bool cleanUmlaute = true)
        {
            if (trim) input = input.Trim();
            if (toLower) input = input.ToLower().Trim();
            StringBuilder output = new StringBuilder();
            foreach (char c in input)
            {
                if (cleanUmlaute)
                {
                    switch (c)
                    {
                        case 'ä':
                            output.Append("ae");
                            break;
                        case 'ö':
                            output.Append("oe");
                            break;
                        case 'ü':
                            output.Append("ue");
                            break;
                    }
                }
                switch (c)
                {
                    case 'è':
                        output.Append("e");
                        break;
                    case 'ê':
                        output.Append("e");
                        break;
                    case 'é':
                        output.Append("e");
                        break;
                    case 'ì':
                        output.Append("i");
                        break;
                    case 'î':
                        output.Append("i");
                        break;
                    case 'í':
                        output.Append("i");
                        break;
                    case 'â':
                        output.Append("a");
                        break;
                    case 'à':
                        output.Append("a");
                        break;
                    case 'á':
                        output.Append("a");
                        break;
                    default:
                        output.Append(c);
                        break;
                }
            }
            return output.ToString();
        }
        public string CleanCharacters(string input, char[] charsToClean)
        {
            StringBuilder output = new StringBuilder();
            foreach (char c in input)
            {
                if (!charsToClean.Contains(c)) output.Append(c);
            }
            return output.ToString();
        }
    }
}