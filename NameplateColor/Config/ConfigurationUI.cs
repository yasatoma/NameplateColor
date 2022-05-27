using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

using NameplateColor.Data;

using ImGuiNET;
using Dalamud.Interface.Colors;
using Dalamud.Interface;
using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;

namespace NameplateColor.Config
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    class ConfigurationUI : IDisposable
    {
        private readonly Configuration configuration;
        private readonly Plugin plugin;


        // this extra bool exists for ImGui, since you can't ref a property
        private bool visible = false;

        private int selectedWorldNo1;
        private string addPlayerInput1 = string.Empty;
        private bool showInvalidNameError1;
        private bool showDuplicatePlayerError1;

        private int selectedWorldNo2;
        private string addPlayerInput2 = string.Empty;
        private bool showInvalidNameError2;
        private bool showDuplicatePlayerError2;

        private string[] worlds;

        public ConfigurationUI(Plugin plugin, Configuration configuration)
        {
            try
            {

            this.plugin = plugin;
            this.configuration = configuration;

            worlds = PluginServices.DataManager.GetExcelSheet<World>()
                .Where(world => world.IsPublic)
                .OrderBy(world => world.Name.ToString())
                .Select(world => world.Name.ToString())
                .ToArray();

            string localWorld = PluginServices.ClientState.LocalPlayer.CurrentWorld.GameData.Name;

            for (int i = 0; i < worlds.Length; i++)
            {
                if (worlds[i] == localWorld)
                {
                    selectedWorldNo1 = i;
                    selectedWorldNo2 = i;
                    break;
                }
            }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "NameplateColor: Failed to ConfigurationUI Constructor.");
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

        public void Draw()
        {
            DrawConfigWindow();
        }

        public void DrawConfigWindow()
        {
            if (!Visible)
            {
                return;
            }

            // 初期サイズ設定
            //ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Vector2(300, 200));
            ImGui.SetNextWindowSize(new Vector2(300, 200), ImGuiCond.FirstUseEver);


            // 最小サイズ設定
            //ImGui.SetNextWindowSizeConstraints(new Vector2(300, 200), new Vector2(300, 200));
            ImGui.Separator();

            if (ImGui.Begin("NameplateColorChange Congfig", ref visible, /*ImGuiWindowFlags.NoResize |*/ ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {

                bool _enabled = configuration.Enabled;


                if (ImGui.Checkbox("Enabled", ref _enabled))
                {
                    configuration.Enabled = _enabled;
                    configuration.Save();
                }

                ImGui.Separator();

                if (_enabled)
                {
                    // Role Color Setting Panel
                    DrawRoleColorPanel();

                    // WhiteList Setting Panel
                    DrawWhiteListSettingPanel();
                    ImGui.SameLine();

                    // WhiteList Setting Panel
                    DrawBlackListSettingPanel();
                    ImGui.SameLine();

                    // WhiteList Setting Panel
                    DrawPlayerAddPanel();

                }
                ImGui.Separator();
            }
            ImGui.End();
        }

        private void DrawRoleColorPanel()
        {

            // FCName Visible
            bool _fcNameHide = configuration.fcNameHide;
            if (ImGui.Checkbox("FCName Hide", ref _fcNameHide))
            {
                configuration.fcNameHide = _fcNameHide;
                configuration.Save();
            }

            ImGui.BeginGroup();
            ushort value;
            // TANK用色設定
            value = configuration.colorTank;
            ColorPickerButton("TANK", ref value);
            if (configuration.colorTank != value)
            {
                configuration.colorTank = value;
                configuration.Save();
            }
            ImGui.SameLine();
            ImGui.Text("TANK");

            // HEALER用色設定
            value = configuration.colorHealer;
            ColorPickerButton("HEALER", ref value);
            if (configuration.colorHealer != value)
            {
                configuration.colorHealer = value;
                configuration.Save();
            }
            ImGui.SameLine();
            ImGui.Text("HEALER");

            // DPS用色設定
            value = configuration.colorDPS;
            ColorPickerButton("DPS", ref value);
            if (configuration.colorDPS != value)
            {
                configuration.colorDPS = value;
                configuration.Save();
            }
            ImGui.SameLine();
            ImGui.Text("DPS");

            // HandLand用色設定
            value = configuration.colorHandLand;
            ColorPickerButton("HandLand", ref value);
            if (configuration.colorHandLand != value)
            {
                configuration.colorHandLand = value;
                configuration.Save();
            }
            ImGui.SameLine();
            ImGui.Text("HandLand");

            ImGui.EndGroup();
            ImGui.SameLine();


            ImGui.SetNextItemWidth(200f * ImGuiHelpers.GlobalScale);
            ImGui.BeginGroup();

            // WhiteList用色設定
            value = configuration.colorWhiteList;
            ColorPickerButton("WhiteList", ref value);
            if (configuration.colorWhiteList != value)
            {
                configuration.colorWhiteList = value;
                configuration.Save();
            }
            ImGui.SameLine();
            ImGui.Text("WhiteList");

            // BlackList用色設定
            value = configuration.colorBlackList;
            ColorPickerButton("BlackList", ref value);
            if (configuration.colorBlackList != value)
            {
                configuration.colorBlackList = value;
                configuration.Save();
            }
            ImGui.SameLine();
            ImGui.Text("BlackList");

            ImGui.EndGroup();

        }

        private void DrawWhiteListSettingPanel()
        {

            // Left: PlayerList Panel
            ImGui.BeginGroup();

            ImGui.TextColored(ImGuiColors.DalamudViolet, "White List.");
            ImGui.BeginChild(
                  "WhitePlayerList",
                  new Vector2(205 , 0) * ImGuiHelpers.GlobalScale,
                  true);

            // player = name@worldName
            foreach (string player in this.configuration.whiteList)
            {
                ImGui.Selectable(player);
            }

            ImGui.EndChild();
            ImGui.EndGroup();

            ImGui.SameLine();

        }

        private void DrawBlackListSettingPanel()
        {

            // Center: PlayerList Panel
            ImGui.BeginGroup();

            ImGui.TextColored(ImGuiColors.DalamudViolet, "Balck List.");
            ImGui.BeginChild(
                  "BlackPlayerList",
                  new Vector2(205, 0) * ImGuiHelpers.GlobalScale,
                  true);

            // player = name@worldName
            foreach (string player in this.configuration.blackList)
            {
                ImGui.Selectable(player);
            }

            ImGui.EndChild();
            ImGui.EndGroup();
        }

        private void DrawPlayerAddPanel()
        {

            // Right: PlayerAdd Panel
            ImGui.BeginGroup();

            // Black List
            ImGui.TextColored(ImGuiColors.DalamudViolet, "Add player to White List.");
            ImGui.Spacing();

            ImGui.SetNextItemWidth(150f * ImGuiHelpers.GlobalScale);
            ImGui.Combo(
                "###AddWhitePlayer",
                ref selectedWorldNo1,
                worlds,
                worlds.Length);
            ImGui.SetNextItemWidth(150f * ImGuiHelpers.GlobalScale);
            ImGui.InputTextWithHint(
                "###WhitePlayerNameAdd_Input",
                "player name",
                ref this.addPlayerInput1,
                30);

            ImGui.Spacing();
            if (ImGui.Button("Add" + "###AddWhitePlayer_OK_Button"))
            {
                this.showInvalidNameError1 = false;
                this.showDuplicatePlayerError1 = false;
                if (IsValidCharacterName(this.addPlayerInput1))
                {
                    string player = this.addPlayerInput1 + "@" + this.worlds[this.selectedWorldNo1];

                    //存在チェック
                    if (this.configuration.whiteList.Contains(player))
                    {
                        this.showDuplicatePlayerError1 = true;
                    }
                    else
                    {
                        this.addPlayerInput1 = string.Empty;
                        this.configuration.whiteList.Add(player);
                        configuration.Save();
                    }
                }
                else
                {
                    this.showInvalidNameError1 = true;
                }
            }

            ImGui.SameLine();
            if (ImGui.Button("Cancel" + "###AddWhitePlayer_Cancel_Button"))
            {
                this.addPlayerInput1 = string.Empty;
                this.showInvalidNameError1 = false;
                this.showDuplicatePlayerError1 = false;
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


            // Black List
            ImGui.TextColored(ImGuiColors.DalamudViolet, "Add player to Black List.");
            ImGui.Spacing();

            ImGui.SetNextItemWidth(150f * ImGuiHelpers.GlobalScale);
            ImGui.Combo(
                "###AddBlackPlayer",
                ref selectedWorldNo2,
                worlds,
                worlds.Length);
            ImGui.SetNextItemWidth(150f * ImGuiHelpers.GlobalScale);
            ImGui.InputTextWithHint(
                "###BlackPlayerNameAdd_Input",
                "player name",
                ref this.addPlayerInput2,
                30);

            ImGui.Spacing();
            if (ImGui.Button("Add" + "###AddBlackPlayer_OK_Button"))
            {
                this.showInvalidNameError2 = false;
                this.showDuplicatePlayerError2 = false;
                if (IsValidCharacterName(this.addPlayerInput2))
                {
                    string player = this.addPlayerInput2 + "@" + this.worlds[this.selectedWorldNo2];

                    //存在チェック
                    if (this.configuration.blackList.Contains(player))
                    {
                        this.showDuplicatePlayerError2 = true;
                    }
                    else
                    {
                        this.addPlayerInput2 = string.Empty;
                        this.configuration.blackList.Add(player);
                        configuration.Save();
                    }
                }
                else
                {
                    this.showInvalidNameError2 = true;
                }
            }

            ImGui.SameLine();
            if (ImGui.Button("Cancel" + "###AddBlackPlayer_Cancel_Button"))
            {
                this.addPlayerInput2 = string.Empty;
                this.showInvalidNameError2 = false;
                this.showDuplicatePlayerError1 = false;
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


            ImGui.EndGroup();



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

    }
}


