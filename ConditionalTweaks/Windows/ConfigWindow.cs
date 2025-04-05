using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ConditionalTweaks.Managers;
using ConditionalTweaks.Managers.RuleUpdaters;
namespace ConditionalTweaks.Windows;

public class ConfigWindow : Window, IDisposable {
    private Rule? rule = null;
    private const int Width = 360;
    private Vector2 headingSize = new Vector2(Width - 15, 20);
    private bool newRule = true;

    private string description = "";
    private int settingNum = -1;
    private int value = 0;
    private int valueOff = 1;

    private Dictionary<string, bool> conditions = new Dictionary<string, bool>();
    private int newConditionNum = 0;
    private bool newConditionVal = false;

    private bool delete = false;

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow() : base("Conditional Tweaks Editor###ConditionalTweaks-settings") {
        Flags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.AlwaysAutoResize;

        SizeCondition = ImGuiCond.Always;
    }

    public void setRuleAndShow(Rule? rule) {
        if (this.IsOpen && this.rule == rule) {
            this.Toggle();
            return;
        }

        if (rule == null) {
            newRule = true;
            rule = Plugin.RuleManager.getGenericRule();
        } else {
            newRule = false;
        }

        this.WindowName = "Conditional Tweaks Editor - " + (newRule ? "New Rule" : "Editing rule") + "###ConditionalTweaks-settings";

        if (rule != this.rule) {
            this.rule = rule;
            description = rule.description;
            settingNum = Array.IndexOf(Plugin.Data.settings, rule.setting);
            value = (int)rule.value;
            valueOff = (int)rule.valueOff;
            conditions = new Dictionary<string, bool>(rule.conditions);

            newConditionNum = 0;
            newConditionVal = false;
        }

        if (!this.IsOpen) {
            this.Toggle();
        }
    }

    public void Dispose() { }

    public override void PreDraw() {
        // Flags must be added or removed before Draw() is being called, or they won't apply
        Size = new Vector2(375, 252 + conditions.Count * 24);

        if (delete) {
            Plugin.Configuration.Rules.Remove(rule);

            rule = null;
            newRule = false;

            description = "";
            settingNum = -1;
            value = 0;
            valueOff = 1;

            conditions = new Dictionary<string, bool>();
            newConditionNum = 0;
            newConditionVal = false;

            delete = false;

            Plugin.Configuration.Save();

            if (this.IsOpen) { this.Toggle(); }
        }
    }

    private void HeadingButton(string text, Vector2 size) {
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, Dalamud.Interface.Colors.ImGuiColors.ParsedGrey);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Dalamud.Interface.Colors.ImGuiColors.ParsedGrey);
        ImGui.PushStyleColor(ImGuiCol.Button, Dalamud.Interface.Colors.ImGuiColors.ParsedGrey);
        ImGui.Button(text, size);
        ImGui.PopStyleColor(3);
    }

    private void deleteButton() {
        using (ImRaii.PushFont(UiBuilder.IconFont)) {
            if (ImGuiComponents.IconButton(FontAwesomeIcon.TrashAlt.ToIconString()))
                ImGui.OpenPopup("Delete?");
        }
        if (ImGui.IsItemHovered()) {
            ImGui.SetTooltip("Delete");
        }

        // Always center this window when appearing
        Vector2 center = ImGui.GetMainViewport().GetCenter();
        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));

        bool open = true;
        if (ImGui.BeginPopupModal("Delete?", ref open, ImGuiWindowFlags.AlwaysAutoResize)) {
            ImGui.Text("This rule will be deleted.\nThis operation cannot be undone!");
            ImGui.Separator();

            //static int unused_i = 0;
            //ImGui::Combo("Combo", &unused_i, "Delete\0Delete harder\0");

            if (ImGui.Button("OK", new Vector2(120, 0))) { ImGui.CloseCurrentPopup(); delete = true; }
            ImGui.SetItemDefaultFocus();
            ImGui.SameLine();
            if (ImGui.Button("Cancel", new Vector2(120, 0))) { ImGui.CloseCurrentPopup(); }
            ImGui.EndPopup();
        }
    }

    public override void Draw() {
        if (rule == null) {
            ImGui.TextUnformatted("No rule set");
            return;
        }
        if (ImGui.BeginChild(rule.setting, new Vector2(Width, 216 + conditions.Count * 24), true)) {
            HeadingButton(description, headingSize);
            ImGui.InputText("Description", ref description, 100);
            if (ImGui.BeginCombo("Setting", Plugin.Data.settings[settingNum])) {
                for (int n = 0; n < Plugin.Data.settings.Length; n++) {
                    bool is_selected = (settingNum == n);
                    if (ImGui.Selectable(Plugin.Data.settings[n], is_selected)) { settingNum = n; }
                    if (is_selected) { ImGui.SetItemDefaultFocus(); }
                }
                ImGui.EndCombo();
            }
            ImGui.PushItemWidth(Width - 100 - Plugin.Data.saveSize.X);
            ImGui.InputInt("True value", ref value);
            ImGui.SameLine();
            ImGui.SetCursorPosX(Width - 88);
            if (ImGui.Button("Current###CurrentValue", Plugin.Data.saveSize)) {
                if (Plugin.Data.settings[settingNum] == Plugin.Data.noSettingSet) {
                    Random rnd = new Random();
                    value = rnd.Next(500);
                } else {
                    RuleUpdater temp = RuleUpdater.GetRuleUpdater(Plugin.Data.settings[settingNum]);
                    value = (int)temp.getValue();
                }
            }
            ImGui.InputInt("False value", ref valueOff);
            ImGui.SameLine();
            ImGui.SetCursorPosX(Width - 88);
            if (ImGui.Button("Current###CurrentValueOff", Plugin.Data.saveSize)) {
                if (Plugin.Data.settings[settingNum] == Plugin.Data.noSettingSet) {
                    Random rnd = new Random();
                    valueOff = rnd.Next(500);
                } else {
                    RuleUpdater temp = RuleUpdater.GetRuleUpdater(Plugin.Data.settings[settingNum]);
                    valueOff = (int)temp.getValue();
                }
            }
            ImGui.PopItemWidth();
            if (ImGui.BeginTable("ConditionTable", 3)) {
                ImGui.TableSetupColumn("Condition", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("Value", ImGuiTableColumnFlags.WidthFixed, 80f);
                ImGui.TableSetupColumn("##Delete/Save", ImGuiTableColumnFlags.WidthFixed, 20f);
                ImGui.TableHeadersRow();
                foreach (var condition in conditions) {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text(condition.Key);
                    ImGui.TableNextColumn();
                    ImGui.Text("" + condition.Value);
                    ImGui.TableNextColumn();
                    if (ImGui.Button("-###" + condition.Key, Plugin.Data.smallButton)) {
                        conditions.Remove(condition.Key);
                    }
                }
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                if (ImGui.BeginCombo("###Setting", Plugin.Data.conditions[newConditionNum])) {
                    for (int n = 0; n < Plugin.Data.conditions.Length; n++) {
                        bool is_selected = (newConditionNum == n);
                        if (ImGui.Selectable(Plugin.Data.conditions[n], is_selected)) { newConditionNum = n; }
                        if (is_selected) { ImGui.SetItemDefaultFocus(); }
                    }
                    ImGui.EndCombo();
                }
                ImGui.TableNextColumn();
                ImGui.Checkbox("###new condition value", ref newConditionVal);
                ImGui.TableNextColumn();
                if (ImGui.Button("+###new condition", Plugin.Data.smallButton)) {
                    conditions[Plugin.Data.conditions[newConditionNum]] = newConditionVal;
                }
                ImGui.EndTable();
            }
            if (!newRule) {
                deleteButton();
            } else {
                using (ImRaii.PushFont(UiBuilder.IconFont)) {
                    if (ImGuiComponents.IconButton(FontAwesomeIcon.Clipboard.ToIconString())) {
                        Rule? tempRule = RuleManager.getRuleFromString(ImGui.GetClipboardText());
                        if (tempRule != null) {
                            setRuleAndShow(tempRule);
                        }
                    }
                }
                if (ImGui.IsItemHovered()) {
                    ImGui.SetTooltip("Paste");
                }
            }
            ImGui.SameLine();
            ImGui.SetCursorPosX(Width - 88);
            if (ImGui.Button("Save", Plugin.Data.saveSize)) {

                Rule tempRule = new Rule(description, Plugin.Data.settings[settingNum], (uint)value, (uint)valueOff, conditions);
                if (Plugin.Configuration.Rules.IndexOf(rule) >= 0) {
                    Plugin.Configuration.Rules[Plugin.Configuration.Rules.IndexOf(rule)] = tempRule;
                } else {
                    Plugin.Configuration.Rules.Add(tempRule);
                }
                rule = tempRule;
                conditions = new Dictionary<string, bool>(rule.conditions);
                newRule = false;

                Plugin.Configuration.Save();
            }
            ImGui.EndChild();
        }
    }
}
