using System.Collections.Generic;
using ConditionalTweaks.Managers.RuleUpdaters;

namespace ConditionalTweaks.Managers {
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

            foreach (var condition in conditions) {
                if (!ConditionManager.conditions.ContainsKey(condition.Key)) {
                    ConditionManager.conditions[condition.Key] = false;
                }
            }

        }

        public void setRule(string setting) {
            this.setting = setting;
            updater = RuleUpdater.GetRuleUpdater(setting);
        }
        
        private bool checkDisable()
        {
            foreach (var condition in conditions)
            {
                if (ConditionManager.conditions[condition.Key] == condition.Value)
                {
                    return false;
                }
            }
            return true;
        }

        private bool checkEnable()
        {
            foreach (var condition in conditions)
            {
                if (ConditionManager.conditions[condition.Key] == condition.Value)
                {
                    return true;
                }
            }
            return false;
        }


        public void checkRule(string updatedCondition) {
            //return if update is irrelivant
            if (!conditions.ContainsKey(updatedCondition)) return;
            if (updater.type == RuleUpdater.SettingTypes.DUMMY) return;

            //if enabled check if it's time to disable
            if (updater.type == RuleUpdater.SettingTypes.COMMAND || updater.getValue() == value) {
                if(checkDisable()) updater.setValue(valueOff);
            }

            //otherwise check if time to enable
            if (updater.type == RuleUpdater.SettingTypes.COMMAND || updater.getValue() != value) {
                if(checkEnable()) updater.setValue(value);
            }
        }

        public override string ToString() {
            string retVal = "Rule(" + description + " | " + setting + " " + value + "/" + valueOff + " conditions(";
            foreach (var condition in conditions) {
                retVal += "{" + condition.Key + ", " + condition.Value + "} ";
            }
            retVal += "))";
            return retVal;
        }
    }
}
