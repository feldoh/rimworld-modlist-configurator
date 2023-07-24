using System.Xml;

namespace ModlistConfiguratorTest;

public class Utils
{
    public static XmlNode GetSettings(string fixtureName)
    {
        return SettingsImporter.GetSettingsFromFile(Path.GetFullPath($"../../../Fixtures/{fixtureName}.xml"))
            .DocumentElement
            .ChildNodes[0];
    }

    private Report DoSomething()
    {
        var report = new Report();
        report.Ok = false;
        report.Message = "Test";

        return report;
    }

    private void Test()
    {
        if (DoSomething() is var report and not { Ok: true })
        {
            throw new Exception($"Test failed. Reason: {report.Message}");
        }
    }
}

public class Report
{
    public Boolean Ok;
    public String Message;
}