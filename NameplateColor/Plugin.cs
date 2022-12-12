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
using Dalamud.Interface.Windowing;

using NameplateColor.Nameplates;
using NameplateColor.Config;
using NameplateColor.Data;
using Lumina.Excel.GeneratedSheets;
using Dalamud.ContextMenu;

namespace NameplateColor
{
    public sealed class Plugin : IDalamudPlugin
    {


        [PluginService]
        [RequiredVersion("1.0")]
        public static ObjectTable ObjectTable { get; private set; } = null!;

        public NamePlateManager namePlateManager;

        private WindowSystem WindowSystem { get; }
        private readonly ContextMenu contextMenu;

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

                PluginServices.Configuration = PluginServices.DalamudPluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
                PluginServices.Configuration.Initialize(PluginServices.DalamudPluginInterface);
                PluginServices.ConfigWindow = new ConfigWindow();

                PluginServices.PlayerDelPopup = new PopupWindow();
                this.WindowSystem = new WindowSystem("NameplateColorWindowSystem");
                this.WindowSystem.AddWindow(PluginServices.PlayerDelPopup);

                PluginServices.ContextMenu = new DalamudContextMenu();
                this.contextMenu = new ContextMenu();


                namePlateManager = new NamePlateManager();

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

                PluginServices.ContextMenu.Dispose();
                this.contextMenu.Dispose();

                PluginServices.DalamudPluginInterface.UiBuilder.Draw -= DrawUI;
                PluginServices.DalamudPluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;
                if (namePlateManager != null)
                {
                    namePlateManager.Dispose();
                }
                if (PluginServices.ConfigWindow != null)
                {
                    PluginServices.ConfigWindow.Dispose();
                }

                PluginServices.CommandManager.RemoveHandler(commandName);
                this.WindowSystem.RemoveAllWindows();
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
            PluginServices.ConfigWindow.Visible = true;
        }

        private void DrawUI()
        {
            PluginServices.ConfigWindow.DrawConfigWindow();
            this.WindowSystem.Draw();
        }
        private void DrawConfigUI()
        {
            PluginServices.ConfigWindow.Visible = true;
        }
    }
}
