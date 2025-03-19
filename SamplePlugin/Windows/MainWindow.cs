using System;
using System.Data;
using System.Numerics;
using Dalamud.Game.Config;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using ImGuiNET;
using Lumina.Excel.Sheets;
using TTDConditionalTweaks.Managers;
using static FFXIVClientStructs.FFXIV.Client.UI.AddonJobHudRDM0.BalanceGauge;

namespace TTDConditionalTweaks.Windows;

public class MainWindow : Window, IDisposable
{
    private ConditionManager conditionManager;
    private string goatImagePath;
    private Plugin plugin;

    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(Plugin plugin, string goatImagePath)
        : base("Conditional Tweaks##TTDConditionalTweaks-main")
    {

        Flags = ImGuiWindowFlags.None;
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 250),
            MaximumSize = new Vector2(375, float.MaxValue)
        };

        this.goatImagePath = goatImagePath;
        this.plugin = plugin;
        conditionManager = ConditionManager.GetConditionManager();
    }

    public void Dispose() { }

    public override void Draw()
    {
        if (ImGui.BeginTabBar("Config Tabs"))
        {
            if (ImGui.BeginTabItem("Rules"))
            {
                if(ImGui.BeginChild("RulesChild", new Vector2(375, ImGui.GetContentRegionAvail().Y)))
                {
                    DrawRules();
                    ImGui.EndChild();
                }
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Current Conditions"))
            {
                DrawCurrentConditions();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Other data"))
            {
                if (ImGui.BeginChild("RulesChild", new Vector2(375, ImGui.GetContentRegionAvail().Y)))
                {
                    DrawOtherData();
                    ImGui.EndChild();
                }
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Goat"))
            {
                DrawGoat();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
   }

    private void HeadingButton(string text, Vector2 size){
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, Dalamud.Interface.Colors.ImGuiColors.ParsedGrey);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Dalamud.Interface.Colors.ImGuiColors.ParsedGrey);
        ImGui.PushStyleColor(ImGuiCol.Button, Dalamud.Interface.Colors.ImGuiColors.ParsedGrey);
        ImGui.Button(text, size);
        ImGui.PopStyleColor(3);
    }

    private void DrawRules()
    {
        int num = 0;
        int width = ImGui.GetScrollMaxY() > 0.0f ? 355 : 360;
        Vector2 headingSize = new Vector2(width - 15, 20);
        foreach (var rule in Plugin.Configuration.Rules)
        {

            ImGui.BeginChild("Rule" + rule.setting + num++, new Vector2(width, 150 + rule.conditions.Count * 20), true);
            Plugin.Log.Info("Rule" + rule.setting + num);
            HeadingButton(rule.description, headingSize);
            if(ImGui.BeginTable("RuleTable" + num, 2))
            {
                ImGui.TableSetupColumn("Attribute", ImGuiTableColumnFlags.WidthFixed, 80f);
                ImGui.TableSetupColumn("Value");
                ImGui.TableNextColumn();
                ImGui.Text("Setting");
                ImGui.TableNextColumn();
                ImGui.Text(rule.setting);
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("True value");
                ImGui.TableNextColumn();
                ImGui.Text("" + rule.value);
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("False value");
                ImGui.TableNextColumn();
                ImGui.Text("" + rule.valueOff);
                ImGui.EndTable();
            } 

            if (ImGui.BeginTable("ConditionTable" + num, 2))
            {
                ImGui.TableSetupColumn("Condition",ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("Value", ImGuiTableColumnFlags.WidthFixed, 80f);
                ImGui.TableHeadersRow();
                foreach (var condition in rule.conditions)
                {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text(condition.Key);
                    ImGui.TableNextColumn();
                    ImGui.Text("" + condition.Value);
                }
                ImGui.EndTable();
            }
            using (ImRaii.PushFont(UiBuilder.IconFont))
            {
                if (ImGuiComponents.IconButton(FontAwesomeIcon.Copy.ToIconString())) 
                    ImGui.SetClipboardText(rule.ToString());
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Copy");
            }
            ImGui.SameLine();
            ImGui.SetCursorPosX(width - 88);
            if (ImGui.Button("Edit", Plugin.Data.saveSize))
            {
                Plugin.ConfigWindow.setRuleAndShow(rule);
            }
            ImGui.EndChild();
        }
        ImGui.SetCursorPosX(width - 88);
        if (ImGui.Button("New", Plugin.Data.saveSize))
        {
            Plugin.ConfigWindow.setRuleAndShow(rule: null);
        }
    }

    private void DrawCurrentConditions()
    {
        ImGui.TextUnformatted(conditionManager.getConditionList());
    }

    private void DrawOtherData()
    {
        var localPlayer = Plugin.ClientState.LocalPlayer;
        if (localPlayer == null)
        {
            ImGui.TextUnformatted("Our local player is currently not loaded.");
            return;
        }

        if (!localPlayer.ClassJob.IsValid)
        {
            ImGui.TextUnformatted("Our current job is currently not valid.");
            return;
        }

        // ExtractText() should be the preferred method to read Lumina SeStrings,
        // as ToString does not provide the actual text values, instead gives an encoded macro string.
        ImGui.TextUnformatted($"Our current job is ({localPlayer.ClassJob.RowId}) \"{localPlayer.ClassJob.Value.Abbreviation.ExtractText()}\"");

        // Example for quarrying Lumina directly, getting the name of our current area.
        var territoryId = Plugin.ClientState.TerritoryType;
        if (Plugin.DataManager.GetExcelSheet<TerritoryType>().TryGetRow(territoryId, out var territoryRow))
        {
            ImGui.TextUnformatted($"We are currently in ({territoryId}) \"{territoryRow.PlaceName.Value.Name.ExtractText()}\"");
        }
        else
        {
            ImGui.TextUnformatted("Invalid territory.");
        }
        ImGui.TextWrapped("Last changed setting was " + Plugin.Data.lastSetting + " and it was set to " + Plugin.Data.lastSettingVal);
    }

    private void DrawGoat()
    {
        ImGui.TextUnformatted("Have a goat:");
        var goatImage = Plugin.TextureProvider.GetFromFile(goatImagePath).GetWrapOrDefault();
        if (goatImage != null)
        {
            using (ImRaii.PushIndent(55f))
            {
                ImGui.Image(goatImage.ImGuiHandle, new Vector2(goatImage.Width, goatImage.Height));
            }
        }
        else
        {
            ImGui.TextUnformatted("Image not found.");
        }
    }
}
