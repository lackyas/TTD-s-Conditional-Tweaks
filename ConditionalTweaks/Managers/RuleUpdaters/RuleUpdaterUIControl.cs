using Dalamud.Game.Config;
using System;

namespace ConditionalTweaks.Managers.RuleUpdaters {
    internal class RuleUpdaterUIControl : RuleUpdater {
        UiControlOption setting;
        public RuleUpdaterUIControl(string setting) {
            this.setting = (UiControlOption)Enum.Parse(typeof(UiControlOption), setting);
            this.type = SettingTypes.UICONTROL;
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
