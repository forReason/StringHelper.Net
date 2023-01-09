using System.Text.RegularExpressions;

namespace StringHelper.Net
{
    /// <summary>
    /// Class for formatting numbers as strings. 
    /// Includes methods for converting doubles to strings and rounding numbers to specific precisions for currency display.
    /// </summary>
    public class NumberFormatting
    {
        /// <summary>
        /// convert double to string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToReadableString(double input)
        {
            return input.ToString(DoubleFixedPoint);
        }
        public const string DoubleFixedPoint = "0.###################################################################################################################################################################################################################################################################################################################################################";
        private static Regex Regex = new Regex("^(0.0*\\d{3})");
        /// <summary>
        /// this function rounds to either 1.00 (currency format) or more precise upto 1.0001, useful for cryptoCurrencies
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string PreciseCurrency(double amount)
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
