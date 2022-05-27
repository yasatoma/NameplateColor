using Dalamud.Game.ClientState;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState.Party;
using Dalamud.Data;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Logging;

using System;
using System.IO;
using System.Reflection;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

using NameplateColor.Nameplates;
using NameplateColor.Config;
using NameplateColor.Data;
using Lumina.Excel.GeneratedSheets;

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

        public string Name => "NameplateColor";
        private const string commandName = "/np";

        public Plugin(DalamudPluginInterface pluginInterface)
        {
            try
            {

                PluginLog.LogDebug(String.Format("NameplateColor ver {0} : Start. ", Assembly.GetExecutingAssembly().GetName().Version));
                PluginServices.Initialize(pluginInterface);

                PluginServices.CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
                {
                    HelpMessage = "Open NamepleteColor ConfigWindow."
                });

                Configuration = PluginServices.DalamudPluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
                Configuration.Initialize(PluginServices.DalamudPluginInterface);
                ConfigurationUI = new ConfigurationUI(this, Configuration);

                namePlateManager = new NamePlateManager(this);

                var world = PluginServices.ClientState.LocalPlayer?.CurrentWorld.GameData;

                //PluginServices.ChatGui.Print($"Hello, {world?.RowId}:{world?.Name}!");


                PluginServices.DalamudPluginInterface.UiBuilder.Draw += DrawUI;
                PluginServices.DalamudPluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "NameplateColor: Failed to initialize plugin.");
            }

        }

        public void Dispose()
        {
            try
            {
                if (namePlateManager != null)
                {
                    namePlateManager.Dispose();
                    namePlateManager = null;
                }
                if (ConfigurationUI != null)
                {
                    ConfigurationUI.Dispose();
                }

                PluginServices.CommandManager.RemoveHandler(commandName);
                PluginServices.DalamudPluginInterface.UiBuilder.Draw -= DrawUI;
                PluginServices.DalamudPluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;
                PluginLog.LogDebug("NameplateColor: Dispose.");
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "NameplateColor: Failed to Dispose plugin.");
            }

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
