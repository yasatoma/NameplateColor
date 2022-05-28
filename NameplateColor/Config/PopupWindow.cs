using System;
using System.Collections.Generic;
using System.Numerics;

using Dalamud.Interface;
using Dalamud.Interface.Colors;
using ImGuiNET;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;

namespace NameplateColor.Config
{
    public class PopupWindow : Window
    {
        /// <summary>
        /// Current player.
        /// </summary>
        public string player = String.Empty;

        private readonly Plugin plugin;
        private readonly Configuration configuration;


        private ModalType currentModalType = ModalType.None;

        public enum ModalType
        {
            None,
            ConfirmSpecialColor1ListDelete,
            ConfirmSpecialColor2ListDelete,
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ModalWindow"/> class.
        /// </summary>
        /// <param name="plugin">PlayerTrack plugin.</param>
        public PopupWindow(Plugin plugin)
            : base("ModalWindow", ImGuiWindowFlags.NoResize)
        {
            try
            {
                this.plugin = plugin;
                this.configuration = this.plugin.Configuration;

                this.Size = new Vector2(320, 130);

            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "NameplateColor: Failed to PopupWindow Constructor.");
            }

        }

        public void Open(ModalType modalType, string player)
        {
            this.currentModalType = modalType;
            this.player = player;
            this.IsOpen = true;
        }

        /// <inheritdoc/>
        public override void Draw()
        {
            this.WindowName = "Delete" + "###DeleteConfirmationModal_Window";
            ImGui.TextColored(ImGuiColors.DalamudRed,"Are you sure you want to delete?");
            ImGui.Spacing();
            ImGui.Text("Delete Player Name : ");
            ImGui.SameLine();
            ImGui.TextColored(ImGuiColors.DalamudYellow, player);
            ImGui.Spacing();
            if (ImGui.Button("OK" + "###DeleteConfirmationModalOK_Button"))
            {
                this.IsOpen = false;

                List<string> list;

                switch (this.currentModalType)
                { 
                    case ModalType.ConfirmSpecialColor1ListDelete:
                        list = this.configuration.SpecialColor1List;
                        break;

                    case ModalType.ConfirmSpecialColor2ListDelete:
                        list = this.configuration.SpecialColor2List;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                list.Remove(player);

                configuration.Save();
            }

            ImGui.SameLine();
            if (ImGui.Button("Cancel" + "###DeleteConfirmationModalCancel_Button"))
            {
                this.IsOpen = false;
            }

        }
    }
}
