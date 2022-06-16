using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace String_Helper_Project
{
    public class NumberFormatting
    {
        public static string ToReadableString(double input)
        {
            return input.ToString(DoubleFixedPoint);
        }
        public const string DoubleFixedPoint = "0.###################################################################################################################################################################################################################################################################################################################################################";
    }
}
