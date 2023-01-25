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
        /// <summary>
        /// Extracts the version number from a given string.
        /// </summary>
        /// <param name="versionString">A string that may contain a version number.</param>
        /// <returns>The version number as a string if found, or null if not found.</returns>
        string ExtractVersionNumber(string versionString)
        {
            var versionRegex = new Regex(@"(\d+(\.\d+)*)");
            var match = versionRegex.Match(versionString);
            if (match.Success)
            {
                return match.Value;
            }
            return null;
        }
        /// <summary>
        /// Converts a double to a string with a fixed number of decimal places.
        /// </summary>
        /// <param name="input">The double number to be converted</param>
        /// <returns>The double number as a string with a fixed number of decimal places.</returns>
        public static string ToReadableString(double input)
        {
            return input.ToString(DoubleFixedPoint);
        }
        public const string DoubleFixedPoint = "0.###################################################################################################################################################################################################################################################################################################################################################";
        private static Regex Regex = new Regex("^(0.0*\\d{3})");
        /// <summary>
        /// Truncates a small decimal number to a certain number of decimal places. 
        /// </summary>
        /// <param name="amount">The decimal number to be truncated.</param>
        /// <returns>The truncated decimal number as a string.</returns>
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
