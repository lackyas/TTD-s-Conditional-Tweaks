using Dalamud.Game.Config;
using System;

namespace ConditionalTweaks.Managers.RuleUpdaters {
    internal class RuleUpdater {
        internal RuleUpdater() { }
        internal enum SettingTypes { DUMMY, SYSTEMCONFIG, UICONFIG, UICONTROL, COMMAND }
        internal SettingTypes type { get; init; }

        public static RuleUpdater GetRuleUpdater(string setting) {
            if (Enum.IsDefined(typeof(UiConfigOption), setting)) {
                return new RuleUpdaterUIConfig(setting);
            }
            if (Enum.IsDefined(typeof(UiControlOption), setting)) {
                return new RuleUpdaterUIControl(setting);
            }
            if (Enum.IsDefined(typeof(SystemConfigOption), setting))
            {
                return new RuleUpdaterSystemConfig(setting);
            }
            if (RuleUpdaterCommand.validCommandRules.ContainsKey(setting)) {
                return new RuleUpdaterCommand(setting);
            }
            return new RuleUpdaterDummy(setting);
        }
        public virtual uint getValue() { return 0; }
        public virtual void setValue(uint value) { }
    }
}
