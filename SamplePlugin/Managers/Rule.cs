using Dalamud.Game.Config;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTDConditionalTweaks.Managers.RuleUpdaters;

namespace TTDConditionalTweaks.Managers {
    public class Rule {
        public string setting { get; private set; }
        public string description;
        public uint value;
        public uint valueOff;
        private RuleUpdater updater;
        public Dictionary<string, bool> conditions;

        public Rule(string description, string setting, uint value, uint valueOff, Dictionary<string, bool> conditions) {
            this.description = description;
            this.setting = setting;
            this.value = value;
            this.valueOff = valueOff;
            this.conditions = conditions;

            updater = RuleUpdater.GetRuleUpdater(setting);

            foreach (var condition in conditions)
            {
                if (!ConditionManager.conditions.ContainsKey(condition.Key))
                {
                    ConditionManager.conditions[condition.Key] = false;
                }
            }

        }

        public void setRule(string setting)
        {
            this.setting = setting;
            updater = RuleUpdater.GetRuleUpdater(setting);
        }

        public void checkRule(string updatedCondition) {
            //return if update is irrelivant
            if (!conditions.ContainsKey(updatedCondition)) return;
            //if enabled check if it's time to disable
            if(updater.getValue() == value)
            {
                foreach (var condition in conditions)
                {
                    if (ConditionManager.conditions[condition.Key] == condition.Value)
                    {
                        return;
                    }
                }
                updater.setValue(valueOff);
            }

            //otherwise check if time to enable
            else {
                foreach (var condition in conditions)
                {
                    if (ConditionManager.conditions[condition.Key] == condition.Value)
                    {
                        updater.setValue(value);
                        return;
                    }
                }
            }
        }

        public override string ToString()
        {
            string retVal = "Rule(" + description + " | "+ setting + " " + value + "/" + valueOff + " conditions(";
            foreach (var condition in conditions)
            {
                retVal += "{" + condition.Key + ", " + condition.Value + "} ";
            }
            retVal += "))";
            return retVal;
        }
    }
}
