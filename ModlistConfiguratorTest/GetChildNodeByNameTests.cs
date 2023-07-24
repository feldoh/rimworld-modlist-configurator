namespace ModlistConfiguratorTest;

[TestClass]
public class GetChildNodeByNameTests
{
    [TestMethod]
    public void TestGetChildNodeByName()
    {
        var settings = GetSettings("SimpleNodeComparison");
        Assert.AreEqual("AddHugsLibToNewModLists", XmlUtils.GetChildNodeByName(settings, "AddHugsLibToNewModLists").Name);
        Assert.AreEqual("ShowSatisfiedRequirements", XmlUtils.GetChildNodeByName(settings, "ShowSatisfiedRequirements").Name);
    }
}