using Dalamud.Game.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTDConditionalTweaks.Managers.RuleUpdaters
{
    internal class RuleUpdater
    {
        internal RuleUpdater() { }

        public static RuleUpdater GetRuleUpdater(string setting) {
            if (Enum.IsDefined(typeof(SystemConfigOption), setting))
            {
                return new RuleUpdaterSystemConfig(setting);
            }
            if (Enum.IsDefined(typeof(UiConfigOption), setting))
            {
                return new RuleUpdaterUIConfig(setting);
            }
            if (Enum.IsDefined(typeof(UiControlOption), setting))
            {
                return new RuleUpdaterUIControl(setting);
            }
            return new RuleUpdaterDummy(setting);
        }
        public virtual uint getValue() { return 0; }
        public virtual void setValue(uint value) { }
    }
}
