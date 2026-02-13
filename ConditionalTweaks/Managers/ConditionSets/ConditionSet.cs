using System;
using System.Collections.Generic;
using System.Text;

namespace ConditionalTweaks.Managers.ConditionSets
{
    internal class ConditionSet
    {
        ConditionSetType type = ConditionSetType.OR;
        Dictionary<string, bool> conditions = new Dictionary<string, bool>();
        List<ConditionSet> conditionSets = new List<ConditionSet>();
        List<string> externalConditionSets = new List<string>();
        private bool firstRun = true;
        private bool lastVal = false;

        enum ConditionSetType { AND, OR }
        ConditionSet(ConditionSetType type) { 
            this.type = type;
        }

        bool hasCondition(string condition) {
            if(conditions.ContainsKey(condition)) return true;

            foreach (ConditionSet cs in conditionSets) {
                if (cs.hasCondition(condition)) return true;
            }
            //TODO external condition sets

            return false;
        }

        private bool checkAnd(string c) {
            foreach (KeyValuePair<string, bool> condition in conditions) {
                bool? conditionVal = Plugin.conditionManager.checkCondition(condition.Key);
                if (conditionVal == null) {
                    Plugin.Log.Error("Checking for non-existant condition: " + condition.Key);
                    continue;
                }
                
                if (condition.Value != conditionVal) return false;
            }
            foreach (ConditionSet cs in conditionSets) {
                if (cs.check(c) == false) return false;
            }
            //TODO external condition sets
            return true;
        }

        private bool checkOr(string c) {
            foreach (KeyValuePair<string, bool> condition in conditions) {
                bool? conditionVal = Plugin.conditionManager.checkCondition(condition.Key);
                if (conditionVal == null) {
                    Plugin.Log.Error("Checking for non-existant condition: " + condition.Key);
                    continue;
                }

                if (condition.Value == conditionVal) return true;
            }
            foreach (ConditionSet cs in conditionSets) {
                if (cs.check(c) == true) return true;
            }
            //TODO external condition sets
            return false;
        }

        bool check(string condition) {
            if (!firstRun && !hasCondition(condition)) {
                return lastVal;
            }
            firstRun = false;
            if (type == ConditionSetType.AND) lastVal = checkAnd(condition);
            else lastVal = checkOr(condition);
            return lastVal;
        }
    }
}
