using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using NameplateColor.Data;

using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Text.SeStringHandling.Payloads;


namespace NameplateColor.Nameplates
{
    public class NamePlateManager : IDisposable
    {

        private readonly Plugin plugin;
        private Nameplate? m_Nameplate;


        public NamePlateManager(Plugin plugin)
        {
            this.plugin = plugin;

            PluginServices.ClientState.Login += ClientState_Login;
            PluginServices.ClientState.Logout += ClientState_Logout;
            Hook();
        }

        public void Dispose()
        {
            Unhook();
            PluginServices.ClientState.Logout -= ClientState_Logout;
            PluginServices.ClientState.Login -= ClientState_Login;
        }

        private void Hook()
        {
            if (m_Nameplate == null)
            {
                m_Nameplate = new Nameplate();
                if (!m_Nameplate.IsValid)
                {
                    m_Nameplate = null;
                }

                if (m_Nameplate != null)
                {
                    m_Nameplate.PlayerNameplateUpdated += Nameplate_PlayerNameplateUpdated;
                }
            }
        }

        private void Unhook()
        {
            if (m_Nameplate != null)
            {
                m_Nameplate.PlayerNameplateUpdated -= Nameplate_PlayerNameplateUpdated;
                m_Nameplate.Dispose();
                m_Nameplate = null;
            }
        }

        private void ClientState_Login(object? sender, EventArgs e)
        {
            Hook();
        }

        private void ClientState_Logout(object? sender, EventArgs e)
        {
            Unhook();
        }

        private void Nameplate_PlayerNameplateUpdated(PlayerNameplateUpdatedArgs args)
        {

            if (!plugin.Configuration.Enabled) return;

            AddTagsToNameplate(args.PlayerCharacter, args.Name);

            if (this.plugin.Configuration.fcNameHide == true)
            {
                args.FreeCompany.Payloads.Clear();
            }


        }

        private void AddTagsToNameplate(GameObject gameObject, SeString name)
        {


            if (gameObject is PlayerCharacter playerCharacter1)
            {

                // 自分自身の色は変更しない
                if (playerCharacter1.ObjectId == PluginServices.ClientState.LocalPlayer?.ObjectId) return;

                if (playerCharacter1.ClassJob.GameData != null)
                {

                    switch (playerCharacter1.ClassJob.GameData?.Role)
                    {
                        case 1: // Tank

                            name.Payloads.Insert(0, new UIForegroundPayload(this.plugin.Configuration.colorTank));
                            name.Payloads.Add(new UIForegroundPayload(0));
                            break;
                        case 2: // Melee

                            name.Payloads.Insert(0, new UIForegroundPayload(this.plugin.Configuration.colorDPS));
                            name.Payloads.Add(new UIForegroundPayload(0));
                            break;
                        case 3: // Range

                            name.Payloads.Insert(0, new UIForegroundPayload(this.plugin.Configuration.colorDPS));
                            name.Payloads.Add(new UIForegroundPayload(0));
                            break;
                        case 4: // Healer

                            name.Payloads.Insert(0, new UIForegroundPayload(this.plugin.Configuration.colorHealer));
                            name.Payloads.Add(new UIForegroundPayload(0));
                            break;
                        default:

                            name.Payloads.Insert(0, new UIForegroundPayload(this.plugin.Configuration.colorHandLand));
                            name.Payloads.Add(new UIForegroundPayload(0));
                            break;
                    }
                }
            }
        }


    }

}
