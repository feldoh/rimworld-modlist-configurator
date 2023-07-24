using System.Collections.Generic;
using System.IO;
using System.Xml;
using Verse;

namespace ModlistConfigurator;

public readonly struct Preset
{
    public Preset(string name, string label, string version, DirectoryInfo settingsDir)
    {
        Name = name;
        Label = label;
        Version = version;
        SettingsDirectory = settingsDir;
    }
    
    public string Name { get; }
    public string Label { get; }
    public string Version { get; }
    public DirectoryInfo SettingsDirectory { get; }
}

public class LoadedPreset: IExposable
{
    public LoadedPreset()
    {
    }

    public LoadedPreset(string name, string version)
    {
        Name = name;
        Version = version;
        Configs = new();
    }

    public string Name;
    public string Version;
    public List<StoredConfig> Configs = new();

    public void AddLoadedConfig(string modId, XmlNode config)
    {
        Configs.Add(new StoredConfig(modId, config));
    }

    public void ExposeData()
    {
        Scribe_Values.Look(ref Name, "Name");
        Scribe_Values.Look(ref Version, "Version");
        Scribe_Collections.Look(ref Configs, "Configs", LookMode.Deep);
    }
}

public class StoredConfig: IExposable
{
    public StoredConfig()
    {
    }

    public StoredConfig(string modId, XmlNode settings)
    {
        ModId = modId;
        settingsString = settings.OuterXml;
    }

    private string settingsString;

    public string ModId;

    public XmlNode Settings
    {
        get
        {
            if (settingsString == null) return null;
            
            var document = new XmlDocument();
            document.LoadXml(settingsString);
            return document.ChildNodes[0];
        }
    }

    public void ExposeData()
    {
        Scribe_Values.Look(ref ModId, "ModId");
        Scribe_Values.Look(ref settingsString, "Settings");
    }
}