using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using FFXIVClientStructs.FFXIV.Client.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using VK = Dalamud.Game.ClientState.Keys.VirtualKey;


namespace ConditionalTweaks.Managers {
    public class ConditionManager : IDisposable {
        private static ConditionManager? Instance;
        internal static Dictionary<string, bool> conditions = new Dictionary<string, bool>();
        private static IPlayerState LocalPlayer = Plugin.PlayerState;

        private ConditionManager() { }
        public static ConditionManager GetConditionManager() {
            {
                if (Instance == null) {
                    Instance = new ConditionManager();
                    conditions["UsingController"] = false;
                    conditions["ActualAutorun"] = false;

                }
                return Instance;
            }
        }

        public void Dispose() {
        }

        public string getConditionList() {
            var retVal = "";
            foreach (var condition in conditions) {
                retVal += condition.Key + ": " + condition.Value + "\n";
            }
            return retVal;
        }

        public void OnConditionChange(ConditionFlag flag, bool value) {
            var flagName = "" + Enum.GetName(flag);
            doRuleUpdate(flagName, value);
        }

        public void CheckAutorunChange(bool value) {
            var curVal = false;
            conditions.TryGetValue("ActualAutorun", out curVal);
            if (value != curVal) {
                doRuleUpdate("ActualAutorun", value);
            }
        }

        private unsafe bool IsInputActive()
        {
            // 1. Check ImGui (Dalamud Plugins)
            if (ImGui.GetIO().WantCaptureKeyboard) return true;

            // 2. Check Game Chat / Search Bars (The "Native" way)
            var uiModule = UIModule.Instance();
            if (uiModule == null) return false;

            var raptureAtkModule = uiModule->GetRaptureAtkModule();
            if (raptureAtkModule == null) return false;

            // This native property checks the flags on the InputManager for you.
            // It covers Chat, Party Finder search, Market Board search, etc.
            return raptureAtkModule->AtkModule.IsTextInputActive();
        }

        public unsafe void CheckIfUsingController()
        {
            //Do nothing if the user has a subwindow highlighted.
            if (IsInputActive()) return;

            //Check Controller movement
            var gs = Plugin.gamepadState;
            var ls = Math.Max(Math.Abs(gs.LeftStick.X), Math.Abs(gs.LeftStick.Y));
            var rs = Math.Max(Math.Abs(gs.RightStick.X), Math.Abs(gs.RightStick.Y));

            //If either stick is moved more than a set deadzone.
            if (Math.Max(ls, rs) >= 25)
            {
                if (conditions["UsingController"] == true) return;
                conditions["UsingController"] = true;
                doRuleUpdate("UsingController", true);
                return;
            }

            //return if already in controller mode
            if (conditions["UsingController"] == false) return;

            // 2. Define the Input IDs for movement (found in FFXIVClientStructs...InputId)
            // 321 = MoveForward, 322 = MoveBack, 323 = MoveLeft, 324 = MoveRight
            InputId[] movementIds = { 
                InputId.MOVE_FORE,
                InputId.MOVE_BACK,
                InputId.MOVE_LEFT,
                InputId.MOVE_RIGHT,
                InputId.MOVE_STRIFE_L,
                InputId.MOVE_STRIFE_R
            };

            bool isMovementKeyPressed = false;
            var ks = Plugin.keyState;
            var uiInput = UIInputData.Instance();

            // 3. Loop through the 4 movement commands

            foreach (var moveId in movementIds)
            {
                Span<KeySetting> binds = uiInput->GetKeybind(moveId)->KeySettings;

                foreach(var bind in binds)
                {
                    if (!Plugin.keyState.IsVirtualKeyValid((VK)(int)bind.Key)){
                        continue;
                    }
                    if((bind.Key != 0) && ks[(VK)(int)bind.Key] && (bind.KeyModifier == 0 || ks[(VK)(int)bind.KeyModifier]))
                    {
                        isMovementKeyPressed = true;
                        break;
                    }
                }
            }

            if (isMovementKeyPressed)
            {
                doRuleUpdate("UsingController", false);
            }
        }

        // -1 unset, 0 heals, 1 tank, 2 dps melee, 3 dps range, 4 dps magic, 5 hand, 6 land
        private int lastRole = -1;
        /* 0  None?
         * 1  GLA PGL MRD LNC ARC 5
         * 6  CNJ THM CRP BSM ARM 10
         * 11 GSM LTW WVR ALC CUL 15
         * 16 MIN BTN FSH PAL MNK 20 
         * 21 WAR DRG BRD WHM BLM 25
         * 26 ACN SMN SCH ROG NIN 30
         * 31 MCH DRK AST SAM RDM 35
         * 36 BLU GNB DNC RPR SGE 40
         * 41 VPR PCT
         */
        private int[] classToRoleMap = { -1,
        1, 2, 1, 2, 4,
        0, 4, 5, 5, 5,
        5, 5, 5, 5, 5,
        6, 6, 6, 1, 2,
        1, 2, 3, 0, 4,
        4, 4, 0, 2, 2,
        3, 1, 0, 2, 4,
        4, 1, 3, 2, 1,
        2, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1};
        //padded with -1s so this doesn't break on a new class being added.
        private string[] roleKeyNames = { "JobRoleHealer", "JobRoleTank", "JobRoleDPSMelee", "JobRoleDPSRange", "JobRoleDPSMage", "JobRoleCrafter", "JobRoleGatherer" };
        public void CheckRoleChange() {
            int role = classToRoleMap[LocalPlayer.ClassJob.RowId];
            if (role == lastRole) return;
            if (lastRole != -1) doRuleUpdate(roleKeyNames[lastRole], false);
            if (role != -1) doRuleUpdate(roleKeyNames[role], true);
            lastRole = role;
        }

        private void doRuleUpdate(string flagName, bool value) {
            conditions[flagName] = value;
            if (!Plugin.ClientState.IsPvP)
            {
                Plugin.RuleManager.checkRules(flagName);
            }
        }

        internal bool? checkCondition(string flagName) {
            return conditions[flagName]; 
        }
    }
}
