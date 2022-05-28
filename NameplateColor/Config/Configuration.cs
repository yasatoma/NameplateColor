using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Numerics;
using System.Collections.Generic;

namespace NameplateColor.Config
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {

        public int Version { get; set; } = 0;

        public bool Enabled { get; set; } = false;

        public ushort colorTank = 37;
        public ushort colorHealer = 42;
        public ushort colorDPS = 508;
        public ushort colorHandLand = 22;

        public ushort colorFriend = 62;

        public ushort colorSpecialColor1 = 1;
        public ushort colorSpecialColor2 = 5;

        public bool fcNameHide = true;
        public bool useFriendColor = false;

        public List<string> SpecialColor1List = new();
        public List<string> SpecialColor2List = new();

        // the below exist just to make saving less cumbersome

        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            pluginInterface!.SavePluginConfig(this);
        }
    }
}
