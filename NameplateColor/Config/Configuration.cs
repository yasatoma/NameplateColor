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

        public ushort colorWhiteList = 1;
        public ushort colorBlackList = 5;

        public bool fcNameHide = true;

        public List<string> whiteList = new();
        public List<string> blackList = new();

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
