using System;
using System.Linq;
using Dalamud.ContextMenu;
//using FFLogsViewer.Manager;
using Lumina.Excel.GeneratedSheets;
using NameplateColor.Config;
using NameplateColor.Data;

namespace NameplateColor.Nameplates
{
    public class ContextMenu : IDisposable
    {
        public ContextMenu()
        {
            if (PluginServices.Configuration.ContextMenu)
            {
                Enable();
            }
        }

        public static void Enable()
        {
            PluginServices.ContextMenu.OnOpenGameObjectContextMenu -= OnOpenContextMenu;
            PluginServices.ContextMenu.OnOpenGameObjectContextMenu += OnOpenContextMenu;
        }

        public static void Disable()
        {
            PluginServices.ContextMenu.OnOpenGameObjectContextMenu -= OnOpenContextMenu;
        }

        public void Dispose()
        {
            Disable();
            GC.SuppressFinalize(this);
        }

        private static bool IsMenuValid(BaseContextMenuArgs args)
        {
            switch (args.ParentAddonName)
            {
                case null: // Nameplate/Model menu
                case "LookingForGroup":
                case "PartyMemberList":
                case "FriendList":
                case "FreeCompany":
                case "SocialList":
                case "ContactList":
                case "ChatLog":
                case "_PartyList":
                case "LinkShell":
                case "CrossWorldLinkshell":
                case "ContentMemberList": // Eureka/Bozja/...
                case "BeginnerChatList":
                    return args.Text != null && args.ObjectWorld != 0 && args.ObjectWorld != 65535;

                default:
                    return false;
            }
        }

        private static void SearchPlayerFromMenu(BaseContextMenuArgs args)
        {
            var world = PluginServices.DataManager.GetExcelSheet<World>()
                               ?.FirstOrDefault(x => x.RowId == args.ObjectWorld);

            if (world == null)
                return;

            if (args.Text == null)
                return;

            PluginServices.ConfigWindow.OpenConfig(new CharaData(args.Text.ToString(), world.Name));

        }

        private static void OnOpenContextMenu(GameObjectContextMenuOpenArgs args)
        {
            if (!PluginServices.DalamudPluginInterface.UiBuilder.ShouldModifyUi || !IsMenuValid(args))
                return;

            args.AddCustomItem(new GameObjectContextMenuItem("Add NameplateColor List", Search));
        }

        private static void Search(GameObjectContextMenuItemSelectedArgs args)
        {
            if (!IsMenuValid(args))
                return;

            SearchPlayerFromMenu(args);
        }
    }

}