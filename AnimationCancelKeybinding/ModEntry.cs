using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace AnimationCancelKeybinding;

/// <summary>The mod entry point.</summary>
internal sealed class ModEntry : Mod
{
    private ModConfig config;

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        I18n.Init(Helper.Translation);
        config = helper.ReadConfig<ModConfig>();
        helper.Events.Input.ButtonsChanged += Input_ButtonsChanged;
        helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
    }

    private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
            return;
        
        // register mod
        configMenu.Register(
            mod: ModManifest,
            reset: () => config = new ModConfig(),
            save: () => Helper.WriteConfig(config)
        );
        
        configMenu.AddKeybindList(
            ModManifest,
            name: () => I18n.Gmcm_Keybind_Name(),
            tooltip: () => I18n.Gmcm_Keybind_Tooltip(),
            getValue: () => config.CancelKey,
            setValue: value => config.CancelKey = value);
        configMenu.AddTextOption(
            ModManifest,
            name: () => I18n.Gmcm_Suppression_Name(),
            tooltip: () => I18n.Gmcm_Suppression_Tooltip(I18n.Gmcm_Keybind_Name()),
            getValue: () => config.Suppression.ToString(),
            setValue: value => config.Suppression = Enum.Parse<KeySuppression>(value),
            allowedValues: Enum.GetValues<KeySuppression>().Select(v => v.ToString()).ToArray(),
            formatAllowedValue: value => Helper.Translation.Get($"Gmcm.Suppression.Option.{value}"));
    }

    private void Input_ButtonsChanged(object sender, ButtonsChangedEventArgs e)
    {
        if (Game1.freezeControls)
        {
            return;
        }
        var keybind = config.CancelKey.GetKeybindCurrentlyDown();
        if (keybind is null)
        {
            return;
        }
        var player = Game1.player;
        if (!player.UsingTool)
        {
            if (config.Suppression == KeySuppression.Always)
            {
                SuppressKeybind(keybind);
            }
            return;
        }
        player.forceCanMove();
        player.completelyStopAnimatingOrDoingAction();
        player.UsingTool = false;
        SuppressKeybind(keybind);
    }

    private void SuppressKeybind(Keybind keybind)
    {
        foreach (var button in keybind.Buttons)
        {
            Helper.Input.Suppress(button);
        }
    }
}

enum KeySuppression { OnCancel, Always }

class ModConfig
{
    public KeybindList CancelKey { get; set; } = KeybindList.Parse("Space");
    public KeySuppression Suppression { get; set; } = KeySuppression.OnCancel;
}
