using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Numerics;
using XivCommon.Functions.NamePlates;
using Lumina.Excel.GeneratedSheets;
using NameplateColor.Data;

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


        public ConfigurationUI(Plugin plugin, Configuration configuration)
        {
            this.plugin = plugin;
            this.configuration = configuration;
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
            // This is our only draw handler attached to UIBuilder, so it needs to be
            // able to draw any windows we might have open.
            // Each method checks its own visibility/state to ensure it only draws when
            // it actually makes sense.
            // There are other ways to do this, but it is generally best to keep the number of
            // draw delegates as low as possible.

            DrawConfigWindow();
            //DrawSettingsWindow();
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
                ushort _colorTank = configuration.colorTank;
                ushort _colorHealer = configuration.colorHealer;
                ushort _colorDPS = configuration.colorDPS;
                ushort _colorHandLand = configuration.colorHandLand;
                bool _fcNameHide = configuration.fcNameHide;


                if (ImGui.Checkbox("Enabled", ref _enabled))
                {
                    configuration.Enabled = _enabled;
                    configuration.Save();
                }

                ImGui.Separator();

                if (_enabled)
                {

                    // FCName Visible
                    if (ImGui.Checkbox("FCName Hide", ref _fcNameHide))
                    {
                        configuration.fcNameHide = _fcNameHide;
                        configuration.Save();
                    }

                    ushort value;
                    // TANK用色設定
                    value = _colorTank;
                    ColorPickerButton("TANK", ref value);
                    if (_colorTank != value)
                    {
                        configuration.colorTank = value;
                        configuration.Save();
                    }
                    ImGui.SameLine();
                    ImGui.Text("TANK");

                    // HEALER用色設定
                    value = _colorHealer;
                    ColorPickerButton("HEALER", ref value);
                    if (_colorHealer != value)
                    {
                        configuration.colorHealer = value;
                        configuration.Save();
                    }
                    ImGui.SameLine();
                    ImGui.Text("HEALER");

                    // DPS用色設定
                    value = _colorDPS;
                    ColorPickerButton("DPS", ref value);
                    if (_colorDPS != value)
                    {
                        configuration.colorDPS = value;
                        configuration.Save();
                    }
                    ImGui.SameLine();
                    ImGui.Text("DPS");

                    // HandLand用色設定
                    value = _colorHandLand;
                    ColorPickerButton("HandLand", ref value);
                    if (_colorHandLand != value)
                    {
                        configuration.colorHandLand = value;
                        configuration.Save();
                    }
                    ImGui.SameLine();
                    ImGui.Text("HandLand");

                }
                ImGui.Separator();
            }
            ImGui.End();
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

    }
}


