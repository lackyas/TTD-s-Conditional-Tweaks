using Dalamud.Configuration;
using System;
using System.Collections.Generic;
using ConditionalTweaks.Managers;

namespace ConditionalTweaks;

[Serializable]
public class Configuration : IPluginConfiguration {
    public int Version { get; set; } = 0;

    public List<Rule> Rules { get; set; } = new List<Rule>();

    // the below exist just to make saving less cumbersome
    public void Save() {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
