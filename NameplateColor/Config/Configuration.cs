using System;
using System.Numerics;
using System.Collections.Generic;

using NameplateColor.Data;

using Dalamud.Configuration;
using Dalamud.Plugin;
using Dalamud.Logging;

namespace NameplateColor.Config
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {

        //public enum NameFormat : int
        //{
        //    Default = 0,
        //    FullName,
        //    FirstNameInitial,
        //    FamilyNameInitial,
        //    AllInitial,
        //}

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

        public int SpecialColor1EmphasisMarkNo = 0;
        public int SpecialColor2EmphasisMarkNo = 0;
        public int SpecialColor1NameFormat = 0;
        public int SpecialColor2NameFormat = 0;
        public List<string> SpecialColor1List = new();
        public List<string> SpecialColor2List = new();

        public bool ContextMenu { get; set; } = true;

        // the below exist just to make saving less cumbersome

        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;

            // Emphasis Mark Out of Range Check
            try
            {
                string v = Common.EmphasisMark[SpecialColor1EmphasisMarkNo];
            }
            catch (Exception ex)
            {
                PluginLog.LogWarning("SpecialColor1EmphasisMarkNo is OutOfRange.{0}", SpecialColor1EmphasisMarkNo);
                SpecialColor1EmphasisMarkNo = 0;
            }

            try
            {
                string v = Common.EmphasisMark[SpecialColor2EmphasisMarkNo];
            }
            catch (Exception ex)
            {
                PluginLog.LogWarning("SpecialColor2EmphasisMarkNo is OutOfRange.{0}", SpecialColor2EmphasisMarkNo);
                SpecialColor2EmphasisMarkNo = 0;
            }

        }

        public void Save()
        {
            pluginInterface!.SavePluginConfig(this);
        }
    }
}
