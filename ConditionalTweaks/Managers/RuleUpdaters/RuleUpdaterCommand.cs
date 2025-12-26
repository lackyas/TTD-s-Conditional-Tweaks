using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using System;
using System.Collections.Generic;

namespace ConditionalTweaks.Managers.RuleUpdaters {
    internal class RuleUpdaterCommand : RuleUpdater
    {

        static internal Dictionary<string, (uint Min, uint Max, uint current)> validCommandRules = new()
        {
            { 
                "HudLayout", (1, 4, 0) 
            }
        };
        string setting;
        bool lastReturnedMax = true;
        public RuleUpdaterCommand(string setting) {
            this.setting = setting;
            this.type = SettingTypes.COMMAND;
        }
        public override uint getValue() {
            return validCommandRules[setting].current;
        }
        public override unsafe void setValue(uint value) {
            if (validCommandRules.TryGetValue(setting, out var entry))
            {
                if (value < entry.Min || value > entry.Max)
                {
                    uint temp = value;
                    value = Math.Clamp(value, entry.Min, entry.Max);
                    Plugin.Log.Warning($"Value {temp} for '{setting}' is out of valid range ({entry.Min}-{entry.Max}). Has been set to {value}");
                }
                validCommandRules[setting] = (entry.Min, entry.Max, entry.current);
                AddonConfig.Instance()->ChangeHudLayout(value - 1);
                Plugin.Log.Info($"Setting {setting} to {value}");
                return;
            }
            Plugin.Log.Warning($"Automation for {setting} has not been implmented yet");
        }
    }
}
