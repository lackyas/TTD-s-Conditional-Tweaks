using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConditionalTweaks.Managers {
    internal class RuleManager {
        public RuleManager() { }

        public void init() {
            if (Plugin.Configuration.Rules.Count == 0) {
                Plugin.Configuration.Rules.Add(new Rule("Disable Autosheathe in instances", "WeaponAutoPutAway", 0, 1, new Dictionary<string, bool> { { "BoundByDuty", true }, { "InDeepDungeon", true } }));
                Plugin.Configuration.Rules.Add(new Rule("Enable BGM in instances and cutscenes", "IsSndBgm", 0, 1, new Dictionary<string, bool> { { "BoundByDuty", true }, { "InDeepDungeon", true }, { "OccupiedInCutSceneEvent", true } }));
                Plugin.Configuration.Rules.Add(new Rule("Legacy movement with standard autorun", "MoveMode", 0, 1, new Dictionary<string, bool> { { "ActualAutorun", true } }));
                Plugin.Configuration.Save();
            }
            foreach (var rule in Plugin.Configuration.Rules) {
                Plugin.Log.Debug("Loaded rule: " + rule.ToString());
            }
            Plugin.Log.Debug("There are " + Plugin.Configuration.Rules.Count + " rules.");
        }

        public void checkRules(string condition) {
            foreach (Rule rule in Plugin.Configuration.Rules) {
                rule.checkRule(condition);
            }
        }

        private readonly string[] npcName = ["Tataru", "Alpa", "Emet Selch", "Kan-E-Senna", "Sultana Nanamo Ul Namo",
                                            "Admiral Merlwyb Bloefhiswyn", "Mistbeards", "Ryne", "Baderon", "Momodi",
                                            "Miounne", "Riol", "Edda", "Zhloe", "Khloe",
                                            "Dreamingway", "Matsya", "Ruby the emerald Carbuncle", "Puddingway", "Lalai Lai"];
        private readonly string[] quality = ["impeccable", "exceptional", "outstanding", "unparalleled", "high-caliber", "elite",
                                            "okay", "mediocre", "ordinary", "average", "passable", "standard",
                                            "shoddy", "inferior", "cheap", "abysmal", "defective", "unsatisfactory"];
        private readonly string[] noun = ["rule", "law", "statute", "regulation", "decree", "principle", "edict", "mandate", "code", "dictate",
                                          "injunction", "commandmemnt", "maxim", "axiom", "bylaw", "golden rule", "directive", "prescript", "tenet", "order"];
        internal Rule getGenericRule() {
            Random rnd = new Random();
            return new Rule(npcName[rnd.Next(npcName.Length)] + "'s " + quality[rnd.Next(quality.Length)] + " " + noun[rnd.Next(noun.Length)], "None set", 0, 1, new Dictionary<string, bool> { });
        }

        internal static Rule? getRuleFromString(string ruleString) {
            //Rule(Enable BGM in instances and cutscenes | IsSndBgm 0/1 conditions({BoundByDuty, True} {InDeepDungeon, True} {OccupiedInCutSceneEvent, True} {ActualAutorun, False} ))
            Plugin.Log.Debug("Recieved clipboard text: " + ruleString);
            if (!(ruleString.StartsWith("Rule(")) && ruleString.EndsWith("} ))")) return null;

            ruleString = ruleString.Remove(ruleString.Length - 4, 4).Remove(0, 5);

            string[] parts = ruleString.Split(" | ");
            string description = parts[0];
            parts = parts[1].Split(" ", 3);

            string setting = parts[0];

            string[] values = parts[1].Split("/");
            if (!uint.TryParse(values[0], out uint value)) return null;
            if (!uint.TryParse(values[1], out uint valueOff)) return null;

            if (!parts[2].StartsWith("conditions({")) return null;
            parts[2] = parts[2].Remove(0, 12);

            parts = parts[2].Split("} {");

            Dictionary<string, bool> conditions = new Dictionary<string, bool>();

            foreach (string part in parts) {
                string[] subparts = part.Split(", ");
                if (subparts[1] == "True") {
                    conditions[subparts[0]] = true;
                } else if (subparts[1] == "False") {
                    conditions[subparts[0]] = false;
                } else {
                    return null;
                }
            }

            return new Rule(description, setting, value, valueOff, conditions);
        }
    }
}
