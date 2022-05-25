using Dalamud.Game.ClientState;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState.Party;
using Dalamud.Data;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game.ClientState.Objects;

//using Dalamud.Game.Text.SeStringHandling;
using Dalamud.DrunkenToad;

using System;
using System.IO;
using System.Reflection;
using System.Numerics;
using NameplateColor.Nameplates;
using NameplateColor.Config;
using NameplateColor.Data;

namespace NameplateColor
{
    public sealed class Plugin : IDalamudPlugin
    {

        private readonly ConfigurationUI ConfigurationUI;


        [PluginService]
        [RequiredVersion("1.0")]
        public static ObjectTable ObjectTable { get; private set; } = null!;

        public NamePlateManager namePlateManager;
        public Configuration Configuration { get; init; } = null!;

        public string Name => "Sample Plugin";
        private const string commandName = "/pmycommand";

        public Plugin(DalamudPluginInterface pluginInterface)
        {
            try
            {

                PluginServices.Initialize(pluginInterface);

                PluginServices.CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
                {
                    HelpMessage = "A useful message to display in /xlhelp"
                });

                Configuration = PluginServices.DalamudPluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
                Configuration.Initialize(PluginServices.DalamudPluginInterface);
                ConfigurationUI = new ConfigurationUI(this, Configuration);

                namePlateManager = new NamePlateManager(this);

                var world = PluginServices.ClientState.LocalPlayer?.CurrentWorld.GameData;
                PluginServices.ChatGui.Print($"Hello, {world?.Name}!");

                PluginServices.DalamudPluginInterface.UiBuilder.Draw += DrawUI;
                PluginServices.DalamudPluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

                Logger.LogInfo("Start My Plugin");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to Plugin Constructor.");
            }

        }

        public void Dispose()
        {

            if (namePlateManager != null)
            {
                namePlateManager.Dispose();
                namePlateManager = null;
            }
            ConfigurationUI.Dispose();
            PluginServices.CommandManager.RemoveHandler(commandName);
            PluginServices.DalamudPluginInterface.UiBuilder.Draw -= DrawUI;
            PluginServices.DalamudPluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;

            Logger.LogInfo("Quit My Plugin");
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            ConfigurationUI.Visible = true;
        }

        private void DrawUI()
        {
            ConfigurationUI.Draw();
        }
        private void DrawConfigUI()
        {
            ConfigurationUI.Visible = true;
        }
    }
}
