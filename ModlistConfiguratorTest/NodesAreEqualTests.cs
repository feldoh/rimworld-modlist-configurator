using System.Xml;

namespace ModlistConfiguratorTest;

[TestClass]
public class NodesAreEqualTests
{
    [TestMethod]
    public void TestNodeValuesAreEqual()
    {
        var expectedDocument =
            XmlUtils.GetChildNodeByName(GetSettings("SimpleNodeExpected"), "AddHugsLibToNewModLists");
        var comparisonDocument =
            XmlUtils.GetChildNodeByName(GetSettings("SimpleNodeComparison"), "AddHugsLibToNewModLists");

        Assert.IsTrue(XmlUtils.NodesAreEqual(expectedDocument, comparisonDocument));
    }

    [TestMethod]
    public void TestNodeValuesAreNotEqual()
    {
        var expectedDocument =
            XmlUtils.GetChildNodeByName(GetSettings("SimpleNodeExpected"), "ShowSatisfiedRequirements");
        var comparisonDocument =
            XmlUtils.GetChildNodeByName(GetSettings("SimpleNodeComparison"), "ShowSatisfiedRequirements");

        Assert.IsFalse(XmlUtils.NodesAreEqual(expectedDocument, comparisonDocument));
    }

    [TestMethod]
    public void TestChildNodeValuesAreEqual()
    {
        var expectedDocument =
            XmlUtils.GetChildNodeByName(GetSettings("NestedNodesExpected"), "Compatibilities");

        Assert.IsTrue(XmlUtils.NodesAreEqual(expectedDocument, expectedDocument));
    }

    [TestMethod]
    public void TestChildNodeValuesAreNotEqual()
    {
        var expectedDocument =
            XmlUtils.GetChildNodeByName(GetSettings("NestedNodesExpected"), "Compatibilities");
        var comparisonDocument =
            XmlUtils.GetChildNodeByName(GetSettings("NestedNodesComparison"), "Compatibilities");

        Assert.IsFalse(XmlUtils.NodesAreEqual(expectedDocument, comparisonDocument));
    }

    [TestMethod]
    public void TestExpectedNodeMissingChildren()
    {
        Assert.IsTrue(false);
    }

    [TestMethod]
    public void TestComparisonNodeMissingChildren()
    {
        Assert.IsTrue(false);
    }

    [TestMethod]
    public void TestHasChildMismatch()
    {
        Assert.IsTrue(false);
    }

    [TestMethod]
    public void TestHydrateAndRestore()
    {
        var expectedDocument =
            XmlUtils.GetChildNodeByName(GetSettings("NestedNodesExpected"), "Compatibilities");

        var stored = expectedDocument.OuterXml;
        var restored = new XmlDocument();
        restored.LoadXml(stored);
        var restored2 = restored.ChildNodes[0];
        
        Assert.IsTrue(XmlUtils.NodesAreEqual(expectedDocument, restored2));
    }
}