using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Numerics;
using XivCommon.Functions.NamePlates;

namespace NameplateColor.Config
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {

        public int Version { get; set; } = 0;

        public bool Enabled { get; set; } = false;

        public ushort colorTank = 37;
        public ushort colorHealer = 508;
        public ushort colorDPS = 42;
        public ushort colorHandLand = 22;

        public bool fcNameHide = false;

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
