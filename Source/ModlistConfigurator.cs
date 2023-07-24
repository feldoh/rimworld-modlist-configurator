using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;

namespace ModlistConfigurator;

public class ModlistConfigurator : Mod
{
    public static DirectoryInfo SettingsDir;

    public ModlistConfigurator(ModContentPack content) : base(content)
    {
        SettingsDir = Content.ModMetaData.RootDir.GetDirectories().FirstOrDefault(dir => dir.Name == "Settings");
        GetSettings<Settings>().AutomaticSettingsImport();
    }

    public override void DoSettingsWindowContents(Rect canvas)
    {
        base.DoSettingsWindowContents(canvas);
        GetSettings<Settings>().DoWindowContents(canvas);
    }

    public override string SettingsCategory()
    {
        return "Auto Mod Config";
    }
}