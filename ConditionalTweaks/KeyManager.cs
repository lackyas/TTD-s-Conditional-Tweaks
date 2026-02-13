using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using System;
using ConditionalTweaks.Managers;
using ConditionalTweaks.Managers.RuleUpdaters;

namespace ConditionalTweaks {
    internal class KeyManager {
        public void configEvent(object? sender, Dalamud.Game.Config.ConfigChangeEvent e) {
            RuleUpdater temp = RuleUpdater.GetRuleUpdater(e.Option.ToString());
            Plugin.Data.lastSetting = e.Option.ToString();
            Plugin.Data.lastSettingVal = "" + (int)temp.getValue();
        }
    }
}
