using Dalamud.Game.ClientState.GamePad;
using Dalamud.Game.Config;
using Dalamud.IoC;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            controllerProcessDelay = DateTime.Now.AddMilliseconds(250);
        }

        public void configEvent(object? sender, Dalamud.Game.Config.ConfigChangeEvent e) {
            Plugin.Log.Debug("Setting Changed: " + e.Option.ToString());
            RuleUpdater temp = RuleUpdater.GetRuleUpdater(e.Option.ToString());
            Plugin.Data.lastSetting = e.Option.ToString();
            Plugin.Data.lastSettingVal = "" + (int)temp.getValue();
        }
    }
}
