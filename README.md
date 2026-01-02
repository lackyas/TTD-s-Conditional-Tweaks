# Conditional Tweaks

**Conditional Tweaks** is a plugin for FFXIV (via Dalamud) that automatically reacts to in-game conditions to change your settings on the fly. It lets you change game settings based on your character's state.

Do you want to keep your weapon out in dungeons? Do you want BGM to always be on for cutscenes? **Do you want the world to turn purple when you jump?**

Then this is the plugin for you.

## 🧩 The Problem
FFXIV has a massive amount of settings, but you often want them to behave differently depending on what you are doing. You might want your weapon sheathed in the overworld but drawn in raids, or background music on during cutscenes but off while grinding. Manually toggling these every time is tedious.

## 🛠️ The Solution
**Conditional Tweaks** acts as an automation layer for your game settings. It allows you to:
* **Automate Settings:** Create rules that trigger setting changes based on conditions.
* **Restore Functionality:** Fully emulates "The Great Controller HUD Switcher" for dynamic HUD management.
* **Share Configurations:** Easily export and import complex rule sets via your clipboard.

## 📋 Default Features
By default, the plugin comes with three active rules to get you started:
* **Weapon Sheathing:** Prevents your character from auto-sheathing their weapon when you are in instanced content.
* **Smart BGM:** Automatically turns Background Music **ON** during instanced content or cutscenes, and turns it **OFF** the rest of the time.
* **Auto-Run Movement:** Automatically switches your movement mode (Standard/Legacy) when the character is auto-running.

## 🧪 Fun Examples
To demonstrate the power (and absurdity) of the rule system, here is how you achieve that "Purple World" effect mentioned earlier.

Copy these lines one at a time and open the Conditional Tweaks settings, click new rule, and then press the clipboard icon:
```
Rule(Miounne's standard directive | AccessibilityColorBlindFilterEnable 1/0 conditions({Jumping, True} ))
```
```
Rule(Kan-E-Senna's shoddy golden rule | AccessibilityColorBlindFilterType 3/0 conditions({Jumping, True} ))
```

## 📥 Installation
1.  Open the **Dalamud Settings** in-game.
2.  Navigate to the **Experimental** tab.
3.  Check the box to **Get plugin testing builds**.
4.  Open the **Plugin Installer** and search for **Conditional Tweaks**.
5.  Click Install.

## 🚀 Usage
Managing Rules
Once installed, open the plugin settings (/conditweaks) to view the active rules.
* **Edit:** Click on any existing rule to tweak the conditions (Where/When) and the result (What setting changes).
* **New:** Create new rules from scratch to automate other parts of your configuration.

Import / Export
You don't have to build every rule yourself.
* **Copy to Clipboard:** Share your favorite rules with others by clicking the copy button.
* **Import from Clipboard:** If a friend shares a rule string (like the examples above), simply copy it and click the clipboard button in the rule creation window to add it to your list.

## ❤️ Support
If you found this tool useful, consider giving the repo a star!
If you want to support my "Project a Week" challenge or buy me a coffee, you can do so here: [Become a Patron](https://www.patreon.com/cw/ccgreen)

