using Dalamud.Game.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTDConditionalTweaks.Managers.RuleUpdaters
{
    internal class RuleUpdaterDummy : RuleUpdater
    {
        string setting;
        public RuleUpdaterDummy(string setting)
        {
            this.setting = setting;
        }
        public override uint getValue()
        {
            Plugin.Log.Error(setting + " is not a valid game setting, please remove it from your rules (returning 0 as default value)");
            return 0;
        }
        public override void setValue(uint value)
        {
            Plugin.Log.Error(setting + " is not a valid game setting, please remove it from your rules");
        }
    }
}
