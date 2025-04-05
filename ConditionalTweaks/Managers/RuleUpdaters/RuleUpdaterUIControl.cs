using Dalamud.Game.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConditionalTweaks.Managers.RuleUpdaters {
    internal class RuleUpdaterUIControl : RuleUpdater {
        UiControlOption setting;
        public RuleUpdaterUIControl(string setting) {
            this.setting = (UiControlOption)Enum.Parse(typeof(UiControlOption), setting);
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
