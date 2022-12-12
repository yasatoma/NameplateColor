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
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Logging;


namespace NameplateColor.Nameplates
{
    public class NamePlateManager : IDisposable
    {

        private Nameplate? m_Nameplate;

        public NamePlateManager()
        {

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
                    PluginLog.LogDebug("NameplateColor: Hook.");
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
                PluginLog.LogDebug("NameplateColor: Unhook.");
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

            if (!PluginServices.Configuration.Enabled) return;

            AddTagsToNameplate(args.PlayerCharacter, args.Name);

            if (PluginServices.Configuration.fcNameHide == true)
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

                // SpecialColor1 Listのチェック
                string playerName = playerCharacter1.Name + "@" + playerCharacter1.HomeWorld.GameData!.Name;
                if (PluginServices.Configuration.SpecialColor1List.Contains(playerName))
                {
                    string defaultName;
                    if (name.Payloads[0] is TextPayload textPayload)
                    {
                        defaultName = textPayload.Text!;
                    }
                    else
                    {
                        defaultName = playerCharacter1.Name.ToString();   
                    }

                    string convertedName = ConvertNameFormat(
                            PluginServices.Configuration.SpecialColor1NameFormat,
                            playerCharacter1.Name.ToString(),
                            defaultName,
                            Common.EmphasisMark[PluginServices.Configuration.SpecialColor1EmphasisMarkNo]
                    );

                    name.Payloads.Clear();
                    name.Payloads.Insert(0, new TextPayload(convertedName));
                    name.Payloads.Insert(0, new UIForegroundPayload(PluginServices.Configuration.colorSpecialColor1));
                    name.Payloads.Add(new UIForegroundPayload(0));
                }

                // SpecialColor2 Listのチェック
                else if (PluginServices.Configuration.SpecialColor2List.Contains(playerName))
                {
                    string defaultName;
                    if (name.Payloads[0] is TextPayload textPayload)
                    {
                        defaultName = textPayload.Text!;
                    }
                    else
                    {
                        defaultName = playerCharacter1.Name.ToString();
                    }

                    string convertedName = ConvertNameFormat(
                            PluginServices.Configuration.SpecialColor2NameFormat, 
                            playerCharacter1.Name.ToString(),
                            defaultName,
                            Common.EmphasisMark[PluginServices.Configuration.SpecialColor2EmphasisMarkNo]
                    );

                    name.Payloads.Clear();
                    name.Payloads.Insert(0, new TextPayload(convertedName));
                    name.Payloads.Insert(0, new UIForegroundPayload(PluginServices.Configuration.colorSpecialColor2));
                    name.Payloads.Add(new UIForegroundPayload(0));
                }

                // フレンドリストのチェック
                else if (PluginServices.Configuration.useFriendColor && (playerCharacter1.StatusFlags & StatusFlags.IsCasting) == StatusFlags.IsCasting)
                {
                    // need implements
                    name.Payloads.Insert(0, new UIForegroundPayload(PluginServices.Configuration.colorFriend));
                    name.Payloads.Add(new UIForegroundPayload(0));
                }

                // Listに存在しない場合
                else if (playerCharacter1.ClassJob.GameData != null)
                {

                    switch (playerCharacter1.ClassJob.GameData?.Role)
                    {
                        case 1: // Tank

                            name.Payloads.Insert(0, new UIForegroundPayload(PluginServices.Configuration.colorTank));
                            name.Payloads.Add(new UIForegroundPayload(0));
                            break;
                        case 2: // Melee

                            name.Payloads.Insert(0, new UIForegroundPayload(PluginServices.Configuration.colorDPS));
                            name.Payloads.Add(new UIForegroundPayload(0));
                            break;
                        case 3: // Range

                            name.Payloads.Insert(0, new UIForegroundPayload(PluginServices.Configuration.colorDPS));
                            name.Payloads.Add(new UIForegroundPayload(0));
                            break;
                        case 4: // Healer

                            name.Payloads.Insert(0, new UIForegroundPayload(PluginServices.Configuration.colorHealer));
                            name.Payloads.Add(new UIForegroundPayload(0));
                            break;
                        default:

                            name.Payloads.Insert(0, new UIForegroundPayload(PluginServices.Configuration.colorHandLand));
                            name.Payloads.Add(new UIForegroundPayload(0));
                            break;
                    }
                }
            }
        }

        private string ConvertNameFormat(int nameFormatNo, string fullName, string defaultName, string emphasisMark)
        {
            string[] nameParts = fullName.Split(" ");

            switch (nameFormatNo)
            {
                case 0: // Default
                    //return defaultName + emphasisMark;
                    return emphasisMark + defaultName;

                case 1: // FullName
                    //return fullName + emphasisMark;
                    return emphasisMark + fullName;

                case 2: // FirstNameInitial
                    //return nameParts[0].Substring(0, 1) + "." + nameParts[1] + emphasisMark;
                    return emphasisMark + nameParts[0].Substring(0, 1) + "." + nameParts[1];

                case 3: // FamilyNameInitial
                    //return nameParts[0] + "." + nameParts[1].Substring(0, 1) + emphasisMark;
                    return emphasisMark + nameParts[0] + "." + nameParts[1].Substring(0, 1);

                case 4: // AllInitial
                    //return nameParts[0].Substring(0, 1) + "." + nameParts[1].Substring(0, 1) + emphasisMark;
                    return emphasisMark + nameParts[0].Substring(0, 1) + "." + nameParts[1].Substring(0, 1);

                default:
                    throw new Exception("ConvertNameFormatException: UnexpectedNameFormat");
            }
        }

    }

}
