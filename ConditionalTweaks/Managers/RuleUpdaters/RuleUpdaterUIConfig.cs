using Dalamud.Game.Config;
using System;

namespace ConditionalTweaks.Managers.RuleUpdaters {
    internal class RuleUpdaterUIConfig : RuleUpdater {
        UiConfigOption setting;
        public RuleUpdaterUIConfig(string setting) {
            this.setting = (UiConfigOption)Enum.Parse(typeof(UiConfigOption), setting);
            this.type = SettingTypes.UICONFIG;
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
