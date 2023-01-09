using StringHelper.Net;
using System.Text;

namespace StringHelper.Net.XUnitText
{
    public class InvariantString
    {
        [Fact]
        public void InvaryString_Test()
        {
            string sample = " Alf�ns Fr�nke ";
            string result = StringHelper.Net.InvariantString.InvaryString(sample, true);
            if (result != "alfoens frenke")
            {
                throw new Exception($"{result} should be alfoens frenke");
            }
        }
        [Fact]
        public void RemoveCharacters()
        {
            string sample = " Alf�ns Fr�nke. �pper Dr�pper";
            string result = StringHelper.Net.InvariantString.RemoveCharacters(sample, new[] {'�','p'} );
            if (result != " Alfns Fr�nke. �er Dr�er")
            {
                throw new Exception($"'{result}' should be ' Alfns Fr�nke. �er Dr�er'");
            }
        }
    }
}