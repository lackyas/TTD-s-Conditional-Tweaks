using Dalamud.Game.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTDConditionalTweaks.Managers.RuleUpdaters
{
    internal class RuleUpdaterUIConfig : RuleUpdater
    {
        UiConfigOption setting;
        public RuleUpdaterUIConfig(string setting)
        {
            this.setting = (UiConfigOption)Enum.Parse(typeof(UiConfigOption), setting);
        }
        public override uint getValue()
        {
            uint retVal;
            Plugin.GameConfig.TryGet(setting, out retVal);
            return retVal;
        }
        public override void setValue(uint value)
        {
            Plugin.GameConfig.Set(setting, value);
        }
    }
}
