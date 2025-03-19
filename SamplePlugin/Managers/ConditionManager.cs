using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Config;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Common.Lua;
using Lumina.Excel.Sheets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTDConditionalTweaks.Managers
{
    public class ConditionManager : IDisposable
    {
        private static ConditionManager? Instance;
        internal static Dictionary<string, bool> conditions = new Dictionary<string, bool>();

        private ConditionManager() { }
        public static ConditionManager GetConditionManager()
        {
            {
                if (Instance == null)
                {
                    Instance = new ConditionManager();
                }
                return Instance;
            }
        }

        public void Dispose()
        {
        }

        public string getConditionList()
        {
            var retVal = "";
            foreach (var condition in conditions)
            {
                retVal += condition.Key + ": " + condition.Value + "\n";
            }
            return retVal;
        }

        public void OnConditionChange(ConditionFlag flag, bool value)
        {
            var flagName = "" + Enum.GetName(flag);
            conditions[flagName] = value;
            doRuleUpdate(flagName);
        }

        public void CheckAutorunChange(bool value)
        {
            var curVal = false;
            conditions.TryGetValue("ActualAutorun", out curVal);
            if (value != curVal)
            {
                conditions["ActualAutorun"] = value;
                doRuleUpdate("ActualAutorun");
            }
        }

        private void doRuleUpdate(string flagName)
        {
            Plugin.RuleManager.checkRules(flagName);
        }
    }
}
