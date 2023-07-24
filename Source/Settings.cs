using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace ModlistConfigurator;

public class Settings : ModSettings
{
    private SettingsImporter Importer = new();

    public static Dictionary<string, LoadedPreset> StoredPresets = new();

    public override void ExposeData()
    {
        Scribe_Collections.Look(ref StoredPresets, "StoredPresets", LookMode.Value, LookMode.Deep);
        StoredPresets ??= new();
    }

    /**
     * This is our entry point for "Safe Updates". We call it when the mod first initializes, before the user has any
     * input on anything. So as a result, we want to make sure that we don't overwrite any user settings.
     *
     * First we remove any presets that used to be in the list but are no longer in the list, for example when a user swaps modlists
     * Then we split the presets into New Presets and Updated Presets
     * For new presets, we just import them and overwrite the settings
     * For updated presets, we pass them through the `AutoUpdate` function which should ideally only merge "safe" updates
     */
    public void AutomaticSettingsImport()
    {
        var presets = Importer.GetPresets();

        foreach (var storedPresetName in StoredPresets.Keys.Where(storedPresetName =>
                     !presets.Exists(preset => preset.Name == storedPresetName)))
        {
            StoredPresets.Remove(storedPresetName);
        }

        var newPresets = presets.Where(preset => !StoredPresets.ContainsKey(preset.Name)).ToList();
        var existingPresets = presets.Where(preset => StoredPresets.ContainsKey(preset.Name)).ToList();
        var updatedPresets = existingPresets.Where(preset => StoredPresets[preset.Name].Version != preset.Version)
            .ToList();

        List<LoadedPreset> loadedPresets = new();

        newPresets.ForEach(preset =>
        {
            var loadedPreset = Importer.OverwriteSettings(preset.Name);
            if (loadedPreset == null) return;

            loadedPresets.Add(loadedPreset);
        });

        updatedPresets.ForEach(preset =>
        {
            var loadedPreset = Importer.AutoUpdate(preset.Name, StoredPresets[preset.Name]);
            if (loadedPreset == null) return;

            loadedPresets.Add(loadedPreset);
        });

        foreach (var loadedPreset in loadedPresets)
        {
            if (StoredPresets.ContainsKey(loadedPreset.Name)) StoredPresets.Remove(loadedPreset.Name);
            StoredPresets.Add(loadedPreset.Name, loadedPreset);
        }

        Write();
    }

    public void DoWindowContents(Rect canvas)
    {
        var listing = new Listing_Standard();
        listing.Begin(canvas);

        if (Importer.GetPresets().Count == 0)
        {
            listing.Label("No presets found. This mod is designed to work in addition to a Modlist Preset downloaded off of the Steam Workshop and has no functionality without one");
            listing.End();
            return;
        }

        var buttonPosition = listing.GetRect(34f);

        if (Widgets.ButtonText(buttonPosition.LeftHalf().ContractedBy(2f), "Reset to Preset"))
        {
            List<FloatMenuOption> presets = new();

            Importer.GetPresets().ForEach(preset =>
            {
                presets.Add(new FloatMenuOption(preset.Label, () => { OverwriteSettings(preset.Name); }));
            });

            Find.WindowStack.Add(new FloatMenu(presets));
        }
        
        if (Widgets.ButtonText(buttonPosition.RightHalf().ContractedBy(2f), "Merge Preset into Current Config"))
        {
            List<FloatMenuOption> presets = new();

            Importer.GetPresets().ForEach(preset =>
            {
                presets.Add(new FloatMenuOption(preset.Label, () => { MergeSettings(preset.Name); }));
            });

            Find.WindowStack.Add(new FloatMenu(presets));
        }

        if (!Prefs.DevMode)
        {
            listing.End();
            return;
        }
        
        listing.GapLine();
        var nextButtonPosition = listing.GetRect(34f);

        if (Widgets.ButtonText(nextButtonPosition.LeftHalf().ContractedBy(2f), "Check Config Status"))
        {
            List<FloatMenuOption> presets = new();

            Importer.GetPresets().ForEach(preset =>
            {
                presets.Add(new FloatMenuOption(preset.Label, () => { CheckSettingSyncStatus(preset.Name); }));
            });

            Find.WindowStack.Add(new FloatMenu(presets));
        }
        
        listing.End();
    }

    private void CheckSettingSyncStatus(string presetName)
    {
        var modsToImport = Importer.ModsToImport(presetName);

        var message = modsToImport.Count == 0
            ? "All mods configs are in sync"
            : $"{modsToImport.Count} mod configs are out of sync";

        Find.WindowStack.Add(new Dialog_MessageBox(
            $"{message}"));
    }

    private void OverwriteSettings(string presetName)
    {
        var modsToImport = Importer.ModsToImport(presetName);

        if (modsToImport.Count == 0)
        {
            Find.WindowStack.Add(new Dialog_MessageBox(
                $"Current config is already set to \"{presetName}\" Preset. There's nothing to import"
            ));
        }
        else
        {
            var s = StoredPresets;

            Find.WindowStack.Add(new Dialog_MessageBox(
                $"This will import the configs for the following mods, and will restart Rimworld to apply the configs. Do you want to continue?\r\n\r\n{string.Join("\r\n", modsToImport)}",
                buttonAAction:
                () =>
                {
                    var loadedConfigs = Importer.OverwriteSettings(presetName);

                    if (loadedConfigs != null)
                    {
                        if (StoredPresets.ContainsKey(loadedConfigs.Name)) StoredPresets.Remove(loadedConfigs.Name);

                        StoredPresets.Add(loadedConfigs.Name, loadedConfigs);
                    }

                    Find.WindowStack.Add(new Dialog_MessageBox(
                        $"Config for all mods have been reset to \"{presetName}\" Preset. Restarting Rimworld to apply the configs",
                        buttonAAction: () =>
                        {
                            Write();
                            ModsConfig.RestartFromChangedMods();
                        }));
                }, buttonBText: "Cancel"));
        }
    }

    private void MergeSettings(string presetName)
    {
        var modsToImport = Importer.ModsToImport(presetName);

        if (modsToImport.Count == 0)
        {
            Find.WindowStack.Add(new Dialog_MessageBox("All mods config is up-to-date, nothing to import"));
        }
        else
        {
            Find.WindowStack.Add(new Dialog_MessageBox(
                $"This will merge the settings for the following mods and will restart Rimworld to apply the configs. Merging allows you to preserve any user-settings that do not conflict with the preset. Do you want to continue?\r\n\r\n{string.Join("\r\n", modsToImport)}",
                buttonAAction:
                () =>
                {
                    Importer.MergeSettings(presetName);
                    Find.WindowStack.Add(new Dialog_MessageBox(
                        $"Config from \"{presetName}\" Preset have been imported. Restarting Rimworld to apply the configs",
                        buttonAAction: ModsConfig.RestartFromChangedMods));
                }, buttonBText: "Cancel"));
        }
    }
}