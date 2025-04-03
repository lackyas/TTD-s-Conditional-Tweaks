using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using TTDConditionalTweaks.Windows;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using System;
using TTDConditionalTweaks.Managers;
using FFXIVClientStructs.FFXIV.Common.Configuration;

namespace TTDConditionalTweaks;

public sealed class Plugin : IDalamudPlugin {
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    [PluginService] internal static ICondition Condition { get; private set; } = null!;
    [PluginService] internal static IFramework Framework { get; private set; } = null!;
    [PluginService] internal static IGameConfig GameConfig { get; private set; } = null!;


    private const string CommandName = "/CondiTweaks";

    public static Configuration Configuration { get; private set; }
    internal static RuleManager RuleManager { get; private set; }
    internal static Data Data { get; set; }

    public readonly WindowSystem WindowSystem = new("Conditional Tweaks");
    internal static ConfigWindow ConfigWindow { get; set; }
    private MainWindow MainWindow { get; init; }
    private ConditionManager conditionManger { get; init; }
    private KeyManager keyManager { get; init; }
    private EventHandler<Dalamud.Game.Config.ConfigChangeEvent> configEvent { get; init; }

    public Plugin() {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        // you might normally want to embed resources and load them from the manifest stream
        var goatImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");

        RuleManager = new RuleManager();
        ConfigWindow = new ConfigWindow();
        MainWindow = new MainWindow(this, goatImagePath);
        conditionManger = ConditionManager.GetConditionManager();
        keyManager = new KeyManager(conditionManger);
        configEvent = new EventHandler<Dalamud.Game.Config.ConfigChangeEvent>(keyManager.configEvent);

        Data = new Data();

        RuleManager.init();

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
            HelpMessage = "Opens the settings menu."
        });

        PluginInterface.UiBuilder.Draw += DrawUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        // Add a simple message to the log with level set to information
        // Use /xllog to open the log window in-game
        // Example Output: 00:57:54.959 | INF | [SamplePlugin] ===A cool log message from Sample Plugin===
        Log.Information($"===A cool log message from {PluginInterface.Manifest.Name}===");

        Condition.ConditionChange += conditionManger.OnConditionChange;
        Framework.Update += keyManager.update;
        GameConfig.UiControlChanged += configEvent;
        GameConfig.UiConfigChanged += configEvent;
        GameConfig.SystemChanged += configEvent;

    }

    public void Dispose() {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);

        Condition.ConditionChange -= conditionManger.OnConditionChange;
        Framework.Update -= keyManager.update;
        GameConfig.UiControlChanged -= configEvent;
        GameConfig.UiConfigChanged -= configEvent;
        GameConfig.SystemChanged -= configEvent;

        conditionManger.Dispose();
    }

    private void OnCommand(string command, string args) {
        // in response to the slash command, just toggle the display status of our main ui
        ToggleMainUI();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleMainUI() => MainWindow.Toggle();
}
