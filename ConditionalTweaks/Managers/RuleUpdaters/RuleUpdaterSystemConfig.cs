using Dalamud.Game.Config;
using System;

namespace ConditionalTweaks.Managers.RuleUpdaters {
    internal class RuleUpdaterSystemConfig : RuleUpdater {
        SystemConfigOption setting;

        public RuleUpdaterSystemConfig(string setting) {
            this.setting = (SystemConfigOption)Enum.Parse(typeof(SystemConfigOption), setting);
            this.type = SettingTypes.SYSTEMCONFIG;
        }
        public override uint getValue() {
            uint retVal;
            Plugin.GameConfig.TryGet(setting, out retVal);
            return retVal;
        }
        public override void setValue(uint value) {
            Plugin.GameConfig.Set(setting, value);
        }
    }
}
