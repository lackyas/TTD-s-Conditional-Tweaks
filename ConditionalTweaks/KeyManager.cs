using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using System;
using ConditionalTweaks.Managers;
using ConditionalTweaks.Managers.RuleUpdaters;

namespace ConditionalTweaks {
    internal class KeyManager {
        private ConditionManager conditionManager;

        public KeyManager(ConditionManager conditionManager) {
            this.conditionManager = conditionManager;
        }

        public void update(IFramework framework) {
            updateKeyPresses();
        }
        private DateTime controllerProcessDelay = DateTime.Now;
        private void updateKeyPresses() {
            // Run every 250ms
            if (DateTime.Now < controllerProcessDelay) return;
            conditionManager.CheckAutorunChange(InputManager.IsAutoRunning());
            conditionManager.CheckIfUsingController();
            controllerProcessDelay = DateTime.Now.AddMilliseconds(250);
        }

        public void configEvent(object? sender, Dalamud.Game.Config.ConfigChangeEvent e) {
            RuleUpdater temp = RuleUpdater.GetRuleUpdater(e.Option.ToString());
            Plugin.Data.lastSetting = e.Option.ToString();
            Plugin.Data.lastSettingVal = "" + (int)temp.getValue();
        }
    }
}
