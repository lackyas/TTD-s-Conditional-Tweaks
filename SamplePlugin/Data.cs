using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Config;
using FFXIVClientStructs.FFXIV.Common.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTDConditionalTweaks.Managers.RuleUpdaters;

namespace TTDConditionalTweaks {
    internal class Data {
        internal string lastSetting = "None";
        internal string lastSettingVal = "Nothing";
        internal string[] settings;
        internal string noSettingSet = "None set";
        internal string[] conditions;


        internal Vector2 saveSize = new Vector2(80, 20);
        internal Vector2 smallButton = new Vector2(20, 20);

        internal Data() {
            List<string> temp = new List<string>();
            temp.Add(noSettingSet);
            temp.AddRange(Enum.GetNames<SystemConfigOption>());
            temp.AddRange(Enum.GetNames<UiConfigOption>());
            temp.AddRange(Enum.GetNames<UiControlOption>());
            temp.Sort();
            settings = temp.ToArray();
            temp = new List<string>();
            temp.AddRange(Enum.GetNames<ConditionFlag>());
            temp.Add("ActualAutorun");
            temp.Sort();
            conditions = temp.ToArray();
        }
    }
}
