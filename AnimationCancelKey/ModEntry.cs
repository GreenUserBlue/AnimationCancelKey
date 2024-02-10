using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;


/// <summary>The mod entry point.</summary>
internal sealed class ModEntry : Mod
{
    private static int counter = 0;
    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        helper.Events.Input.ButtonPressed += testing;
    }

    private void testing(object sender, ButtonPressedEventArgs e)
    {
        if (e.Button != SButton.Space) return;
        var f = Game1.player;
        if (!f.UsingTool) return;
        f.completelyStopAnimatingOrDoingAction();
    }
}