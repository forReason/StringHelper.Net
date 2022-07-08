using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private static Regex Regex = new Regex("^(0.0*\\d{3})");
        public static string TruncateSmallerZeroCryptoAmt(double amount)
        {
            if (amount >= 1)
            { // in a range to show 2 decimals
                return amount.ToString("0.##"); // eg 5.93
            }
            else {
                var match = Regex.Match(ToReadableString(amount));
                return match.Value.TrimEnd('0');
            }
            
        }
    }
}
