
using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;


/// <summary>The mod entry point.</summary>
internal sealed class ModEntry : Mod
{
    private ModConfig config;
    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        config = helper.ReadConfig<ModConfig>();
        helper.Events.Input.ButtonsChanged += cancelOnChange;

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
        var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
            return;

        config = Helper.ReadConfig<ModConfig>();
        
        // register mod
        configMenu.Register(
            mod: this.ModManifest,
            reset: () => this.config = new ModConfig(),
            save: () => this.Helper.WriteConfig(this.config)
        );
        
        configMenu.AddKeybindList( mod: this.ModManifest,
            name: () => "Animation-Cancel key",
            tooltip: () => "Which key to press to cancel the current animation",
            getValue: () => this.config.CancelKey,
            setValue: value => this.config.CancelKey= value);
    }

    private void cancelOnChange(object sender, ButtonsChangedEventArgs e)
    {
        if (!config.CancelKey.JustPressed()) return;
        var f = Game1.player;
        if (!f.UsingTool) return;
        f.completelyStopAnimatingOrDoingAction();
    }
}

class ModConfig
{
    public KeybindList CancelKey { get; set; } = KeybindList.Parse("Space");
}
