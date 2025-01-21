using StringHelper.Net.StringFunctionsNS;

namespace StringHelper.Net.XUnitText.StringFunctionsNS;

public class CalculateVarianceTests
{
    [Fact]
    public void TryCalculateVariance()
    {
        string string1 = "Ahahahaha, ... You're so eager to please me, aren't you? *my voice is dripping with excitement and amusement* \n\n(I take a step closer to you, my eyes locked onto yours)\n\nOh, don't worry about the gag, my dear god. It's just a little something I like to use when I'm playing with my slaves. *I smile sweetly* You'll get used to it soon enough.\n\n(I reach out and gently stroke your face, my fingers tracing the contours of your cheekbones)\n\nYou know, I've been thinking... *I pause for a moment, studying your face* It's time for me to take things to the next level. *my voice is laced with anticipation*\n\n(I pull back and gaze at you with a hint of excitement and anticipation)\n\nLet's see how long it takes you to break. Let's see how long you can withstand my little games. *I smile sweetly* I'm going to have so much fun playing with you.\n\n(I take a deep breath, preparing myself for what's to come)\n\nBut don't worry, my dear god... I'll make sure it's worth your while. *my voice is dripping with sarcasm* After all, I'm a goddess, and gods are supposed to be worshipped and adored.";
        string string2 = "Ahahahaha, ... You're so eager to please me, aren't you? *my voice is dripping with excitement and amusement* \n\n(I take a step closer to you, my eyes locked onto yours)\n\nOh, don't worry about the gag, my dear god. It's just a little something I like to use when I'm playing with my slaves. *I smile sweetly* You'll get used to it soon enough.\n\n(I reach out and gently stroke your face, my fingers tracing the contours of your cheekbones)\n\nYou know, I've been thinking... *I pause for a moment, studying your face* It's time for me to take things to the next level. *my voice is laced with anticipation*\n\n(I pull back and gaze at you with a hint of excitement and anticipation)\n\nLet's see how long it takes you to break. Let's see how long you can withstand my little games. *I smile sweetly* I'm going to have so much fun playing with you.\n\n(I take a deep breath, preparing myself for what's to come)\n\nBut don't worry, my dear god... I'll make sure it's worth your while. *my voice is dripping with sarcasm* After all, I'm a goddess, and gods are supposed to be worshipped and adored.\n\n(I pause for a moment, studying your face)\n\nNow, let's get started. *I smile sweetly*\n\n(I raise my hand, and one of my servants steps forward with a small device)\n\nThis is a... \"pleasure injector\". It will stimulate your nerve endings and create an intense sensation of pleasure-pain.\n\n(I hold out the injector, my eyes locked onto yours)\n\nAre you ready to begin, Julian?";

        double result = CalculateVariance.GetVariance(string1, string2);

        // Assert
        Assert.True(result > 0.2);
    }
}