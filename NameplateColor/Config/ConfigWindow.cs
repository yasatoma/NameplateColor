using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NameplateColor.Data;

using ImGuiNET;
using Dalamud.Interface.Colors;
using Dalamud.Interface;
using Dalamud.Logging;
using Dalamud.Game.ClientState.Objects.SubKinds;

using Lumina.Excel.GeneratedSheets;

namespace NameplateColor.Config
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    class ConfigWindow : IDisposable
    {
        // this extra bool exists for ImGui, since you can't ref a property
        private bool visible = false;

        private int selectedEmphasisMark1;
        private int selectedNameFormat1;
        private int selectedWorldNo1;
        private string addPlayerInput1 = string.Empty;
        private bool showInvalidNameError1;
        private bool showDuplicatePlayerError1;

        private int selectedEmphasisMark2;
        private int selectedNameFormat2;
        private int selectedWorldNo2;
        private string addPlayerInput2 = string.Empty;
        private bool showInvalidNameError2;
        private bool showDuplicatePlayerError2;

        private string[] worlds;
        string localWorld = string.Empty;

        public ConfigWindow()
        {
            try
            {

                worlds = PluginServices.DataManager.GetExcelSheet<World>()!
                    .Where(world => world.IsPublic)
                    .OrderBy(world => world.Name.ToString())
                    .Select(world => world.Name.ToString())
                    .ToArray();

                SetLocalWorldNo();

            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "NameplateColor: Failed to ConfigWindow Constructor.");
            }

        }


        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        // passing in the image here just for simplicity
        public void Dispose()
        {

        }

        public void OpenConfig(CharaData charaData)
        {
            visible = true;

            #region ----- Charadata Apply ----- 

            int? ret = GetLocalWorldNo(charaData.world);
            if (ret != null)
            {
                this.selectedWorldNo1 = (int)ret;
                this.selectedWorldNo2 = (int)ret;
            }
            this.addPlayerInput1 = charaData.name;
            this.addPlayerInput2 = charaData.name;
            #endregion

            PluginLog.LogDebug($"NameplateColor: name={charaData.name}/world={charaData.world}/ret={ret}");

            //DrawConfigWindow();

        }

        public void DrawConfigWindow()
        {
            if (!Visible)
            {
                return;
            }

            SetLocalWorldNo();

            // 初期サイズ設定
            ImGui.SetNextWindowSize(new Vector2(640, 480), ImGuiCond.FirstUseEver);

            // 最小サイズ設定
            ImGui.SetNextWindowSizeConstraints(new Vector2(640, 480), new Vector2(640, 800));
            ImGui.Separator();

            if (ImGui.Begin(
                $"NameplateColor Congfig {Assembly.GetExecutingAssembly().GetName().Version}", 
                ref visible, 
                ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {

                bool _enabled = PluginServices.Configuration.Enabled;

                if (ImGui.Checkbox("Enabled", ref _enabled))
                {
                    PluginServices.Configuration.Enabled = _enabled;
                    PluginServices.Configuration.Save();
                }

                ImGui.Separator();

                if (_enabled)
                {
                    // Role Color Setting Panel
                    DrawColorSettingPanel();

                    #region ----- SpecialColor1 Group Start -----
                    ImGui.BeginGroup();

                    ImGui.TextColored(ImGuiColors.DalamudViolet, "SpecialColor1 List.");

                    #region ----- SpecialColor1 Combo Start -----
                    ImGui.SetNextItemWidth(150f * ImGuiHelpers.GlobalScale);
                    if (ImGui.BeginCombo("###SpecialColor1_NameFormat", Common.NameFormat[PluginServices.Configuration.SpecialColor1NameFormat])) 
                    {
                        selectedNameFormat1 = 0;
                        foreach (string v in Common.NameFormat)
                        {
                            if (ImGui.Selectable(v, (selectedNameFormat1 == PluginServices.Configuration.SpecialColor1NameFormat)))
                            {
                                PluginServices.Configuration.SpecialColor1NameFormat = selectedNameFormat1;
                                PluginServices.Configuration.Save();
                            }
                            selectedNameFormat1++;
                        }
                        ImGui.EndCombo();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("SpecialColor1 Player's Name Format.");
                    }
                    ImGui.SameLine();

                    ImGui.SetNextItemWidth(50f * ImGuiHelpers.GlobalScale);
                    if (ImGui.BeginCombo("###SpecialColor1_EmphasisMark", Common.EmphasisMark[PluginServices.Configuration.SpecialColor1EmphasisMarkNo]))
                    {
                        selectedEmphasisMark1 = 0;
                        foreach (string v in Common.EmphasisMark)
                        {
                            if (ImGui.Selectable(v, (selectedEmphasisMark1 == PluginServices.Configuration.SpecialColor1EmphasisMarkNo)))
                            {
                                PluginServices.Configuration.SpecialColor1EmphasisMarkNo = selectedEmphasisMark1;
                                PluginServices.Configuration.Save();
                            }
                            selectedEmphasisMark1++;
                        }
                        ImGui.EndCombo();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("EmphasisMark add to PlayerName in SpecialColor1 List.");
                    }
                    #endregion ----- SpecialColor1 Combo End -----

                    ImGui.BeginChild(
                        "SpecialColor1_PlayerList",
                        new Vector2(205, 0) * ImGuiHelpers.GlobalScale, true);

                    // SpecialColor1 List
                    DrawSpecialColor1ListSettingPanel();
                    
                    ImGui.EndChild();
                    ImGui.EndGroup();
                    #endregion ----- SpecialColor1 List Group End -----

                    ImGui.SameLine();

                    #region ----- SpecialColor2 Group Start -----
                    ImGui.BeginGroup();

                    ImGui.TextColored(ImGuiColors.DalamudViolet, "SpecialColor2 List.");

                    #region ----- SpecialColor2 Combo Start -----
                    ImGui.SetNextItemWidth(150f * ImGuiHelpers.GlobalScale);
                    if (ImGui.BeginCombo("###SpecialColor2_NameFormat", Common.NameFormat[PluginServices.Configuration.SpecialColor2NameFormat]))
                    {
                        selectedNameFormat2 = 0;
                        foreach (string v in Common.NameFormat)
                        {
                            if (ImGui.Selectable(v, (selectedNameFormat2 == PluginServices.Configuration.SpecialColor2NameFormat)))
                            {
                                PluginServices.Configuration.SpecialColor2NameFormat = selectedNameFormat2;
                                PluginServices.Configuration.Save();
                            }
                            selectedNameFormat2++;
                        }
                        ImGui.EndCombo();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("SpecialColor2 Player's Name Format.");
                    }

                    ImGui.SameLine();

                    ImGui.SetNextItemWidth(50f * ImGuiHelpers.GlobalScale);
                    if (ImGui.BeginCombo("###SpecialColor2_EmphasisMark", Common.EmphasisMark[PluginServices.Configuration.SpecialColor2EmphasisMarkNo]))
                    {
                        selectedEmphasisMark2 = 0;
                        foreach (string v in Common.EmphasisMark)
                        {
                            if (ImGui.Selectable(v, (selectedEmphasisMark2 == PluginServices.Configuration.SpecialColor2EmphasisMarkNo)))
                            {
                                PluginServices.Configuration.SpecialColor2EmphasisMarkNo = selectedEmphasisMark2;
                                PluginServices.Configuration.Save();
                            }
                            selectedEmphasisMark2++;
                        }
                        ImGui.EndCombo();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("EmphasisMark add to PlayerName in SpecialColor2 List.");
                    }
                    #endregion ----- SpecialColor2 Combo End -----

                    ImGui.BeginChild(
                        "SpecialColor2_PlayerList",
                        new Vector2(205, 0) * ImGuiHelpers.GlobalScale, true);

                    // SpecialColor2 List
                    DrawSpecialColor2ListSettingPanel();

                    ImGui.EndChild();
                    ImGui.EndGroup();
                    #endregion ----- SpecialColor2 List Group End -----

                    ImGui.SameLine();

                    #region ----- Player Add Panel Start -----
                    // Right: PlayerAdd Panel
                    ImGui.BeginGroup();

                    // SpecialColor1 Add Panel
                    DrawSpecialColor1PlayerAddPanel();
                    ImGui.Spacing();

                    // SpecialColor2 Add Panel
                    DrawSpecialColor2PlayerAddPanel();

                    ImGui.EndGroup();
                    #endregion ----- Player Add Panel End -----

                }
            }
            ImGui.End();
        }

        private void DrawColorSettingPanel()
        {

            // FCName Visible
            bool _fcNameHide = PluginServices.Configuration.fcNameHide;
            if (ImGui.Checkbox("FCName Hide", ref _fcNameHide))
            {
                PluginServices.Configuration.fcNameHide = _fcNameHide;
                PluginServices.Configuration.Save();
            }
            ImGui.SameLine();

            bool _useFriendColor = PluginServices.Configuration.useFriendColor;
            if (ImGui.Checkbox("Use Friend Color", ref _useFriendColor))
            {
                PluginServices.Configuration.useFriendColor = _useFriendColor;
                PluginServices.Configuration.Save();
            }

            ImGui.BeginGroup();

            ImGui.TextColored(ImGuiColors.DalamudViolet, "Role Color, When no condition.");

            ushort value;
            // TANK用色設定
            value = PluginServices.Configuration.colorTank;
            ColorPickerButton("TANK", ref value);
            if (PluginServices.Configuration.colorTank != value)
            {
                PluginServices.Configuration.colorTank = value;
                PluginServices.Configuration.Save();
            }
            ImGui.SameLine();
            ImGui.Text("TANK");

            // HEALER用色設定
            value = PluginServices.Configuration.colorHealer;
            ColorPickerButton("HEALER", ref value);
            if (PluginServices.Configuration.colorHealer != value)
            {
                PluginServices.Configuration.colorHealer = value;
                PluginServices.Configuration.Save();
            }
            ImGui.SameLine();
            ImGui.Text("HEALER");

            // DPS用色設定
            value = PluginServices.Configuration.colorDPS;
            ColorPickerButton("DPS", ref value);
            if (PluginServices.Configuration.colorDPS != value)
            {
                PluginServices.Configuration.colorDPS = value;
                PluginServices.Configuration.Save();
            }
            ImGui.SameLine();
            ImGui.Text("DPS");

            // HandLand用色設定
            value = PluginServices.Configuration.colorHandLand;
            ColorPickerButton("HandLand", ref value);
            if (PluginServices.Configuration.colorHandLand != value)
            {
                PluginServices.Configuration.colorHandLand = value;
                PluginServices.Configuration.Save();
            }
            ImGui.SameLine();
            ImGui.Text("HandLand");

            ImGui.EndGroup();
            ImGui.SameLine();


            //ImGui.SetNextItemWidth(200f * ImGuiHelpers.GlobalScale);
            ImGui.BeginGroup();

            ImGui.TextColored(ImGuiColors.DalamudViolet, "Conditional Color.");

            // SpecialColor1 List用色設定
            value = PluginServices.Configuration.colorSpecialColor1;
            ColorPickerButton("SpecialColor1List", ref value);
            if (PluginServices.Configuration.colorSpecialColor1 != value)
            {
                PluginServices.Configuration.colorSpecialColor1 = value;
                PluginServices.Configuration.Save();
            }
            ImGui.SameLine();
            ImGui.Text("SpecialColor1");

            // SpecialColor2 List用色設定
            value = PluginServices.Configuration.colorSpecialColor2;
            ColorPickerButton("SpecialColor2List", ref value);
            if (PluginServices.Configuration.colorSpecialColor2 != value)
            {
                PluginServices.Configuration.colorSpecialColor2 = value;
                PluginServices.Configuration.Save();
            }
            ImGui.SameLine();
            ImGui.Text("SpecialColor2");

            // Friend Color用色設定
            value = PluginServices.Configuration.colorFriend;
            ColorPickerButton("FriendColor", ref value);
            if (PluginServices.Configuration.colorFriend != value)
            {
                PluginServices.Configuration.colorFriend = value;
                PluginServices.Configuration.Save();
            }
            ImGui.SameLine();
            ImGui.Text("FriendColor");
            ImGui.SameLine();
            //ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, 30f);
            ImGui.TextColored(ImGuiColors.DalamudGrey2, "When [Use Friend Color] is enabled only.");
            //ImGui.PopStyleVar();
            ImGui.EndGroup();


        }

        private void DrawSpecialColor1ListSettingPanel()
        {
            // player = name@worldName
            int i = 1;
            foreach (string player in PluginServices.Configuration.SpecialColor1List)
            {
                if (ImGui.Selectable(
                    player + "###SpecialColor1_ListPlayer_Selectable_" + i,
                    false,
                    ImGuiSelectableFlags.AllowDoubleClick))
                {
                    if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                    {
                        // Confirm Window
                        PluginServices.PlayerDelPopup.Open(PopupWindow.ModalType.ConfirmSpecialColor1ListDelete, player);
                    }
                }
                i++;
            }
        }

        private void DrawSpecialColor2ListSettingPanel()
        {

            // player = name@worldName
            int i = 1;
            foreach (string player in PluginServices.Configuration.SpecialColor2List)
            {
                if (ImGui.Selectable(
                    player + "###SpecialColor2_ListPlayer_Selectable_" + i,
                    false,
                    ImGuiSelectableFlags.AllowDoubleClick))
                {
                    if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                    {
                        // Confirm Window
                        PluginServices.PlayerDelPopup.Open(PopupWindow.ModalType.ConfirmSpecialColor2ListDelete, player);
                    }

                }
                i++;
            }
        }

        private void DrawSpecialColor1PlayerAddPanel()
        {

            // Get Target Info
            PlayerCharacter? target = null;
            if (PluginServices.ClientState.LocalPlayer!.TargetObject != null && PluginServices.ClientState.LocalPlayer.TargetObject is PlayerCharacter)
            {
                target = (PlayerCharacter)PluginServices.ClientState.LocalPlayer.TargetObject;
            }

            // SpecialColor1 List
            ImGui.TextColored(ImGuiColors.DalamudViolet, "Add player to SpecialColor1 List.");
            ImGui.Spacing();

            ImGui.SetNextItemWidth(150f * ImGuiHelpers.GlobalScale);
            ImGui.Combo(
                "###AddSpecialColor1Player",
                ref selectedWorldNo1,
                worlds,
                worlds.Length);
            ImGui.SetNextItemWidth(150f * ImGuiHelpers.GlobalScale);
            ImGui.InputTextWithHint(
                "###SpecialColor1PlayerNameAdd_Input",
                "player name",
                ref this.addPlayerInput1,
                30);

            ImGui.Spacing();
            if (ImGui.Button("Add" + "###AddSpecialColor1Player_OK_Button"))
            {
                this.showInvalidNameError1 = false;
                this.showDuplicatePlayerError1 = false;
                if (IsValidCharacterName(this.addPlayerInput1))
                {
                    string player = this.addPlayerInput1 + "@" + this.worlds[this.selectedWorldNo1];

                    //存在チェック
                    if (PluginServices.Configuration.SpecialColor1List.Contains(player))
                    {
                        this.showDuplicatePlayerError1 = true;
                    }
                    else
                    {
                        this.addPlayerInput1 = string.Empty;
                        PluginServices.Configuration.SpecialColor1List.Add(player);
                        PluginServices.Configuration.Save();
                    }
                }
                else
                {
                    this.showInvalidNameError1 = true;
                }
            }

            ImGui.SameLine();
            if (ImGui.Button("Cancel" + "###AddSpecialColor1Player_Cancel_Button"))
            {
                int? ret = GetLocalWorldNo(localWorld);
                if (ret != null)
                {
                    this.selectedWorldNo1 = (int)ret;
                }
                this.addPlayerInput1 = string.Empty;
                this.showInvalidNameError1 = false;
                this.showDuplicatePlayerError1 = false;

            }

            if (target != null)
            {
                ImGui.SameLine();
                ImGui.PushFont(UiBuilder.IconFont);
                if (ImGui.Button(FontAwesomeIcon.Search.ToIconString() + "###AddSpecialColor1Player_Search_Button"))
                {
                    int? ret = GetLocalWorldNo(target.HomeWorld.GameData!.Name);
                    if (ret != null)
                    {
                        this.selectedWorldNo1 = (int)ret;
                    }
                    this.addPlayerInput1 = target.Name.ToString();
                }
                ImGui.PopFont();
                ImGui.SameLine();
            }

            ImGui.Spacing();
            if (this.showInvalidNameError1)
            {
                ImGui.TextColored(ImGuiColors.DPSRed, "Not a valid player name - try again.");
            }
            else if (this.showDuplicatePlayerError1)
            {
                ImGui.TextColored(ImGuiColors.DPSRed, "This player already exists in your list!");
            }
        }

        private void DrawSpecialColor2PlayerAddPanel()
        {
            // Get Target Info
            PlayerCharacter? target = null;
            if (PluginServices.ClientState.LocalPlayer!.TargetObject != null && PluginServices.ClientState.LocalPlayer.TargetObject is PlayerCharacter)
            {
                target = (PlayerCharacter)PluginServices.ClientState.LocalPlayer.TargetObject;
            }

            // SpecialColor2 List
            ImGui.TextColored(ImGuiColors.DalamudViolet, "Add player to SpecialColor2 List.");
            ImGui.Spacing();

            ImGui.SetNextItemWidth(150f * ImGuiHelpers.GlobalScale);
            ImGui.Combo(
                "###AddSpecialColor2Player",
                ref selectedWorldNo2,
                worlds,
                worlds.Length);
            ImGui.SetNextItemWidth(150f * ImGuiHelpers.GlobalScale);
            ImGui.InputTextWithHint(
                "###SpecialColor2PlayerNameAdd_Input",
                "player name",
                ref this.addPlayerInput2,
                30);

            ImGui.Spacing();
            if (ImGui.Button("Add" + "###AddSpecialColor2Player_OK_Button"))
            {
                this.showInvalidNameError2 = false;
                this.showDuplicatePlayerError2 = false;
                if (IsValidCharacterName(this.addPlayerInput2))
                {
                    string player = this.addPlayerInput2 + "@" + this.worlds[this.selectedWorldNo2];

                    //存在チェック
                    if (PluginServices.Configuration.SpecialColor2List.Contains(player))
                    {
                        this.showDuplicatePlayerError2 = true;
                    }
                    else
                    {
                        this.addPlayerInput2 = string.Empty;
                        PluginServices.Configuration.SpecialColor2List.Add(player);
                        PluginServices.Configuration.Save();
                    }
                }
                else
                {
                    this.showInvalidNameError2 = true;
                }
            }

            ImGui.SameLine();
            if (ImGui.Button("Cancel" + "###AddSpecialColor2Player_Cancel_Button"))
            {
                int? ret = GetLocalWorldNo(localWorld);
                if (ret != null)
                {
                    this.selectedWorldNo2 = (int)ret;
                }
                this.addPlayerInput2 = string.Empty;
                this.showInvalidNameError2 = false;
                this.showDuplicatePlayerError1 = false;
            }

            if (target != null)
            {
                ImGui.SameLine();
                ImGui.PushFont(UiBuilder.IconFont);
                if (ImGui.Button(FontAwesomeIcon.Search.ToIconString() + "###AddSpecialColor2Player_Search_Button"))
                {
                    int? ret = GetLocalWorldNo(target.HomeWorld.GameData!.Name);
                    if (ret != null)
                    {
                        this.selectedWorldNo2 = (int)ret;
                    }
                    this.addPlayerInput2 = target.Name.ToString();
                }
                ImGui.PopFont();
                ImGui.SameLine();
            }

            ImGui.Spacing();
            if (this.showInvalidNameError2)
            {
                ImGui.TextColored(ImGuiColors.DPSRed, "Not a valid player name - try again.");
            }
            else if (this.showDuplicatePlayerError2)
            {
                ImGui.TextColored(ImGuiColors.DPSRed, "This player already exists in your list!");
            }
        }

        private void ColorPickerButton(string popupID, ref ushort value)
        {
            DrawColorButton(
                popupID + ":" + value.ToString(),
                UIColorHelper.ToColor(value),
                () =>
                {
                    ImGui.OpenPopup(popupID);
                });

            ushort? colorValue = null;
            bool wasStyleConsumed = false;
            ImGui.SetNextWindowPos(ImGui.GetCursorScreenPos() + new Vector2(30, -30));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(10, 10));
            if (ImGui.BeginPopup(popupID))
            {
                wasStyleConsumed = true;
                ImGui.PopStyleVar();

                DrawUIColorPicker(
                    (v) =>
                    {
                        ImGui.CloseCurrentPopup();
                        colorValue = (ushort)v.RowId;
                    });

                value = colorValue is null ? value : (ushort)colorValue;
                ImGui.EndPopup();
            }
            if (!wasStyleConsumed)
            {
                ImGui.PopStyleVar();
            }
        }

        private void DrawColorButton(string colorId, Vector4 color, System.Action clicked)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, color);

            ImGui.PushStyleColor(ImGuiCol.FrameBgActive, Vector4.Zero);
            ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, Vector4.Zero);
            ImGui.PushStyleColor(ImGuiCol.FrameBg, Vector4.Zero);

            //ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 3));
            ////ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(5, 5));
            ////ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, new Vector2(0, 0));
            ////ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3);
            //if (ImGui.Button($"###{colorId}", new Vector2(23, 23)))

            if (ImGui.Button($"###{colorId}", new Vector2(23, 23)))
            {
                clicked();
            }

            ////ImGui.PopStyleVar(3);
            ImGui.PopStyleColor(6);

            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(colorId);
            }
        }

        private void DrawUIColorPicker(Action<UIColor> colorSelected)
        {
            const int columnCount = 12;

            int currentColumn = 0;
            foreach (var uiColor in UIColorHelper.UIColors)
            {
                if (currentColumn % columnCount != 0)
                {
                    ImGui.SameLine(0, 0);
                }

                DrawColorButton(uiColor.RowId.ToString(), UIColorHelper.ToColor(uiColor), () => { colorSelected(uiColor); });
                currentColumn++;
            }
        }

        private bool IsValidCharacterName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            string[] array = value.Split(' ');
            if (array.Length != 2)
            {
                return false;
            }

            int length = array[0].Length;
            if (length < 2 || length > 15)
            {
                return false;
            }

            length = array[1].Length;
            if (length < 2 || length > 15)
            {
                return false;
            }

            if (array[0].Length + array[1].Length > 20)
            {
                return false;
            }

            if (!char.IsLetter(array[0][0]))
            {
                return false;
            }

            if (!char.IsLetter(array[1][0]))
            {
                return false;
            }

            if (!char.IsUpper(array[0][0]))
            {
                return false;
            }

            if (!char.IsUpper(array[1][0]))
            {
                return false;
            }

            if (value.Contains("  "))
            {
                return false;
            }

            if (value.Contains("--"))
            {
                return false;
            }

            if (value.Contains("'-"))
            {
                return false;
            }

            if (value.Contains("-'"))
            {
                return false;
            }

            return !value.Any((char c) => !char.IsLetter(c) && !c.Equals('\'') && !c.Equals('-') && !c.Equals(' '));
        }

        private void SetLocalWorldNo()
        {
            try
            {

                if (localWorld == PluginServices.ClientState.LocalPlayer!.CurrentWorld.GameData!.Name) return;

                localWorld = PluginServices.ClientState.LocalPlayer!.CurrentWorld.GameData!.Name;

                for (int i = 0; i < worlds.Length; i++)
                {
                    if (worlds[i] == localWorld)
                    {
                        PluginLog.LogDebug($"NameplateColor: [SetLocalWorldNo] localWorld={localWorld}/WorldNo={i}");

                        selectedWorldNo1 = i;
                        selectedWorldNo2 = i;
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "NameplateColor: Failed to ConfigWindow SetLocalWorldNo.");
            }

        }

        private int? GetLocalWorldNo(string worldName)
        {
            try
            {

                for (int i = 0; i < worlds.Length; i++)
                {
                    if (worlds[i] == worldName)
                    {
                        PluginLog.LogDebug($"NameplateColor: [SetLocalWorldNo] localWorld={localWorld}/WorldNo={i}");

                        return i;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "NameplateColor: Failed to ConfigWindow SetLocalWorldNo.");
                return null;
            }

        }
    }
}


