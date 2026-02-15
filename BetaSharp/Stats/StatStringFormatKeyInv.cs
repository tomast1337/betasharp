using BetaSharp.Client.Input;
using BetaSharp.Client.Resource.Language;
using BetaSharp.Stats.Achievements;

namespace BetaSharp.Stats;

public class StatStringFormatKeyInv : AchievementStatFormatter
{
    readonly Minecraft theGame;
    private static readonly TranslationStorage localizedName = TranslationStorage.Instance;


    public StatStringFormatKeyInv(Minecraft game)
    {
        theGame = game;
    }

    public String formatString(String key)
    {
        return localizedName.TranslateKeyFormat(key, Keyboard.getKeyName(theGame.options.keyBindInventory.keyCode));
    }
}