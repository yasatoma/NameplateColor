﻿using Dalamud.ContextMenu;
using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using NameplateColor.Config;

namespace NameplateColor.Data
{
    public class PluginServices
    {
        [PluginService] public static ChatGui ChatGui { get; set; } = null!;
        [PluginService] public static ClientState ClientState { get; set; } = null!;
        [PluginService] public static CommandManager CommandManager { get; set; } = null!;
        [PluginService] public static DalamudPluginInterface DalamudPluginInterface { get; set; } = null!;
        [PluginService] public static DataManager DataManager { get; set; } = null!;
        [PluginService] public static Framework Framework { get; set; } = null!;
        [PluginService] public static GameGui GameGui { get; set; } = null!;
        [PluginService] public static ObjectTable ObjectTable { get; set; } = null!;
        [PluginService] public static PartyList PartyList { get; set; } = null!;

        internal static Configuration Configuration { get; set; } = null!;
        internal static DalamudContextMenu ContextMenu { get; set; } = null!;
        internal static ConfigWindow ConfigWindow { get; set; } = null!;
        internal static PopupWindow PlayerDelPopup { get; set; } = null!;

        public static void Initialize(DalamudPluginInterface pluginInterface)
        {
            pluginInterface.Create<PluginServices>();
        }
    }
}
