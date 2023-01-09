using StringHelper.Net;
using System.Text;

namespace StringHelper.Net.XUnitText
{
    public class InvariantString
    {
        [Fact]
        public void InvaryString_Test()
        {
            string sample = " Alföns Frènke ";
            string result = StringHelper.Net.InvariantString.InvaryString(sample, true);
            if (result != "alfoens frenke")
            {
                throw new Exception($"{result} should be alfoens frenke");
            }
        }
        [Fact]
        public void RemoveCharacters()
        {
            string sample = " Alföns Frènke. Öpper Drüpper";
            string result = StringHelper.Net.InvariantString.RemoveCharacters(sample, new[] {'ö','p'} );
            if (result != " Alfns Frènke. Öer Drüer")
            {
                throw new Exception($"'{result}' should be ' Alfns Frènke. Öer Drüer'");
            }
        }
    }
}