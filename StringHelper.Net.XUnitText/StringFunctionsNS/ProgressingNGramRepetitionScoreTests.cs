using StringHelper.Net.StringFunctionsNS;
using Xunit.Abstractions;

namespace StringHelper.Net.XUnitText.StringFunctionsNS;

public class ProgressingNGramRepetitionScoreTests
{
    private readonly ITestOutputHelper _TestOutputHelper;
    public ProgressingNGramRepetitionScoreTests(ITestOutputHelper testOutputHelper)
    {
        _TestOutputHelper = testOutputHelper;
    }
    [Fact]
    public void AddTokens_NoRepetition_ScoreRemainsLow()
    {
        var scorer = new ProgressingNGramRepetitionScore(ngramLength: 2);
        scorer.AddTokens("The quick brown fox jumps over the lazy dog.");

        var scores = scorer.GetAllScores();
        Assert.All(scores, s => Assert.True(s.Value <= 1.0));
    }

    [Fact]
    public void AddTokens_WithRepetition_ScoreIncreases()
    {
        var scorer = new ProgressingNGramRepetitionScore(ngramLength: 2);
        scorer.AddTokens("the cat chased the dog. the cat chased the dog.");

        var flagged = scorer.GetAllScores(onlyReturnFlagged: true);

        Assert.NotEmpty(flagged);
        Assert.Contains(flagged, kv => kv.Key == "the cat" || kv.Key == "cat chased");
        Assert.All(flagged, kv => Assert.True(kv.Value > 1.5));
    }

    [Fact]
    public void AddTokens_PartialToken_NoCrash()
    {
        var scorer = new ProgressingNGramRepetitionScore();
        scorer.AddTokens("incomplete token");
        scorer.AddTokens(" continued and finished.");

        var scores = scorer.GetAllScores();
        Assert.NotNull(scores);
    }

    [Fact]
    public void GetAllScores_ReturnsSortedDescending()
    {
        var scorer = new ProgressingNGramRepetitionScore(ngramLength: 2);
        scorer.AddTokens("repeat this phrase. repeat this phrase again. repeat this phrase once more.");

        var scores = scorer.GetAllScores();
        for (int i = 1; i < scores.Count; i++)
        {
            Assert.True(scores[i - 1].Value >= scores[i].Value);
        }
    }

    [Fact]
    public void Clear_ResetsState()
    {
        var scorer = new ProgressingNGramRepetitionScore(ngramLength: 2);
        scorer.AddTokens("repeat me repeat me");
        Assert.NotEmpty(scorer.GetAllScores());

        scorer.Clear();
        Assert.Empty(scorer.GetAllScores());
        Assert.Equal(0, scorer.WordIndex);
    }

    [Fact]
    public void ShortNGram_ScoringStillWorks()
    {
        var scorer = new ProgressingNGramRepetitionScore(ngramLength: 1);
        scorer.AddTokens("echo echo echo");

        var flagged = scorer.GetAllScores(onlyReturnFlagged: true);
        Assert.NotEmpty(flagged);
    }
    [Fact]
    public void ShortNGram_ScoringWorks()
    {
        var scorer = new ProgressingNGramRepetitionScore(ngramLength: 4);
        List<string> tokens = new List<string>()
{
    "#", " ", "Eins", "atz", "\n", "\n", "* ", "  Fernw", "artung", ", ", "direkte", " Verb", "indung", " zum",
    " Service", "-", "Experten", ":", " Störun", "gen", " direkt", " lös", "en ", "bzw.", " Verkü", "rzung", " der",
    " Dauer", " zur", " Behebung", "\n", "*", "  ", " Monit", "oring ", "/", " Dash", "board", ":", " Sämt", "liche",
    " Anlagen", " auf", " einen", " Blick", "\n", "*  ", " Möglich", "keit ", "Na", "chrichten ", "zu", " versenden",
    " beim", " Erreic", "hen ", "vo", "n Grenz", "werten", "\n", "*  ", " Mit ", "LT", "E ", "oder ", "Kundenn", "etzwerk",
    " möglich", "\n", "*", "   Kein", " Zugriff ", "ohne", " Einwi", "lligung", " des", " Anwenders", "\n", "\n",
    "[", "Image", " of", " two", " people", " in", " lab", " coats", " and", " hair", "nets", " working", " at", " a",
    " laboratory", " bench", "]", "\n", "\n", "#", " SKAN", " connect", "\n", "\n",
    "*   Fernw", "artung", ", ", "direkte", " Verb", "indung", " zum", " Service", "-", "Experten", ":",
    " Störun", "gen", " direkt", " lös", "en ", "bzw.", " Verkü", "rzung", " der", " Dauer", " zur", " Behebung", "\n",
    "*   Monit", "oring ", "/", " Dash", "board", ":", " Sämt", "liche", " Anlagen", " auf", " einen", " Blick", "\n",
    "*   Möglich", "keit ", "Na", "chrichten ", "zu", " versenden", " beim", " Erreic", "hen ", "vo", "n Grenz", "werten", "\n",
    "*   Mit ", "LT", "E ", "oder ", "Kundenn", "etzwerk", " möglich", "\n",
    "*   Kein", " Zugriff ", "ohne", " Einwi", "lligung", " des", " Anwenders", "\n", "\n",
    "[", "Image", " of", " a", " person", " in", " a", " lab", " coat", " and", " hair", "net", " working", " at", " a",
    " laboratory", " bench", "]", "\n", "\n", "#", " SKAN", " connect", "\n", "\n",
    "*   Fernw", "artung", ", ", "direkte", " Verb", "indung", " zum", " Service", "-", "Experten", ":",
    " Störun", "gen", " direkt", " lös", "en ", "bzw.", " Verkü", "rzung", " der", " Dauer", " zur", " Behebung", "\n",
    "*   Monit", "oring ", "/", " Dash", "board", ":", " Sämt", "liche", " Anlagen", " auf", " einen", " Blick", "\n",
    "*   Möglich", "keit ", "Na", "chrichten ", "zu", " versenden", " beim", " Erreic", "hen ", "vo", "n Grenz", "werten", "\n",
    "*   Mit ", "LT", "E ", "oder ", "Kundenn", "etzwerk", " möglich", "\n",
    "*   Kein", " Zugriff ", "ohne", " Einwi", "lligung", " des", " Anwenders", "\n", "\n",
    "[", "Image", " of", " a", " person", " in", " a", " lab", " coat", " and", " hair", "net", " working", " at", " a",
    " laboratory", " bench", "]", "\n", "\n", "#", " SKAN", " connect", "\n", "\n",
    "*   Fernw", "artung", ", ", "direkte", " Verb", "indung", " zum", " Service", "-", "Experten", ":",
    " Störun", "gen", " direkt", " lös", "en ", "bzw.", " Verkü", "rzung", " der", " Dauer", " zur", " Behebung", "\n",
    "*   Monit", "oring ", "/", " Dash", "board", ":", " Sämt", "liche", " Anlagen", " auf", " einen", " Blick", "\n",
    "*   Möglich", "keit ", "Na", "chrichten ", "zu", " versenden", " beim", " Erreic", "hen ", "vo", "n Grenz", "werten", "\n",
    "*   Mit ", "LT", "E ", "oder ", "Kundenn", "etzwerk", " möglich", "\n",
    "*   Kein", " Zugriff ", "ohne", " Einwi", "lligung", " des", " Anwenders", "\n", "\n",
    "[", "Image", " of", " a", " person", " in", " a", " lab", " coat", " and", " hair", "net", " working", " at", " a",
    " laboratory", " bench", "]"
};

        List<KeyValuePair<string, double>> flagged = new List<KeyValuePair<string, double>>();
        foreach (string token in tokens)
        {
            scorer.AddTokens(token);
            flagged = scorer.GetAllScores(onlyReturnFlagged: false);
            if (flagged.Any())
                _TestOutputHelper.WriteLine(flagged[0].Value.ToString());
        }

        Assert.NotEmpty(flagged);
    }

    [Fact]
    public void AddTokens_HandlesBoundaryCorrectly()
    {
        var scorer = new ProgressingNGramRepetitionScore(ngramLength: 2);
        scorer.AddTokens("word1 word2 ");
        scorer.AddTokens("word3 word4");

        var scores = scorer.GetAllScores();
        Assert.Contains(scores, kv => kv.Key == "word1 word2");
    }
}