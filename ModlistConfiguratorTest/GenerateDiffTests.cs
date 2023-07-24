namespace ModlistConfiguratorTest;

[TestClass]
public class GenerateDiffTests
{
    [TestMethod]
    public void TestSimpleStructureDiff()
    {
        var expectedDocument =
            XmlUtils.GetChildNodeByName(GetSettings("DiffNodesExpected"), "SimpleStructure");
        var comparisonDocument =
            XmlUtils.GetChildNodeByName(GetSettings("DiffNodesComparison"), "SimpleStructure");

        var outputDocument =
            XmlUtils.GetChildNodeByName(GetSettings("DiffNodesOutput"), "SimpleStructure");

        var diff = XmlUtils.GenerateDiff(expectedDocument, comparisonDocument);
        Assert.IsTrue(XmlUtils.NodesAreEqual(diff, outputDocument));
    }

    [TestMethod]
    public void TestSimpleStructureNoDiff()
    {
        var expectedDocument =
            XmlUtils.GetChildNodeByName(GetSettings("DiffNodesExpected"), "SimpleStructure");
        
        Assert.IsNull(XmlUtils.GenerateDiff(expectedDocument, expectedDocument));
    }

    [TestMethod]
    public void TestItemsRemovedDiff()
    {
        var expectedDocument =
            XmlUtils.GetChildNodeByName(GetSettings("DiffNodesExpected"), "RemovedItems");
        var comparisonDocument =
            XmlUtils.GetChildNodeByName(GetSettings("DiffNodesComparison"), "RemovedItems");

        var outputDocument =
            XmlUtils.GetChildNodeByName(GetSettings("DiffNodesOutput"), "RemovedItems");

        var diff = XmlUtils.GenerateDiff(expectedDocument, comparisonDocument);
        Assert.IsTrue(XmlUtils.NodesAreEqual(diff, outputDocument));
    }

    [TestMethod]
    public void TestItemsAddedDiff()
    {
        var expectedDocument =
            XmlUtils.GetChildNodeByName(GetSettings("DiffNodesExpected"), "AddedItems");
        var comparisonDocument =
            XmlUtils.GetChildNodeByName(GetSettings("DiffNodesComparison"), "AddedItems");

        var outputDocument =
            XmlUtils.GetChildNodeByName(GetSettings("DiffNodesOutput"), "AddedItems");

        var diff = XmlUtils.GenerateDiff(expectedDocument, comparisonDocument);
        Assert.IsTrue(XmlUtils.NodesAreEqual(diff, outputDocument));
    }

    [TestMethod]
    public void TestNestedDiff()
    {
        var expectedDocument =
            XmlUtils.GetChildNodeByName(GetSettings("DiffNodesExpected"), "NestedSimpleStructure");
        var comparisonDocument =
            XmlUtils.GetChildNodeByName(GetSettings("DiffNodesComparison"), "NestedSimpleStructure");

        var outputDocument =
            XmlUtils.GetChildNodeByName(GetSettings("DiffNodesOutput"), "NestedSimpleStructure");

        var diff = XmlUtils.GenerateDiff(expectedDocument, comparisonDocument);
        Assert.IsTrue(XmlUtils.NodesAreEqual(diff, outputDocument));
    }

    [TestMethod]
    public void TestNestedNoDiff()
    {
        var expectedDocument =
            XmlUtils.GetChildNodeByName(GetSettings("DiffNodesExpected"), "AddedItems");

        Assert.IsNull(XmlUtils.GenerateDiff(expectedDocument, expectedDocument));
    }

    [TestMethod]
    public void TestNestedAddedItems()
    {
        var expectedDocument =
            XmlUtils.GetChildNodeByName(GetSettings("DiffNodesExpected"), "NestedAdd");
        var comparisonDocument =
            XmlUtils.GetChildNodeByName(GetSettings("DiffNodesComparison"), "NestedAdd");

        var outputDocument =
            XmlUtils.GetChildNodeByName(GetSettings("DiffNodesOutput"), "NestedAdd");

        var diff = XmlUtils.GenerateDiff(expectedDocument, comparisonDocument);
        Assert.IsTrue(XmlUtils.NodesAreEqual(diff, outputDocument));
    }

    [TestMethod]
    public void TestNestedRemovedItems()
    {
        var expectedDocument =
            XmlUtils.GetChildNodeByName(GetSettings("DiffNodesExpected"), "NestedRemove");
        var comparisonDocument =
            XmlUtils.GetChildNodeByName(GetSettings("DiffNodesComparison"), "NestedRemove");

        var outputDocument =
            XmlUtils.GetChildNodeByName(GetSettings("DiffNodesOutput"), "NestedRemove");

        var diff = XmlUtils.GenerateDiff(expectedDocument, comparisonDocument);
        Assert.IsTrue(XmlUtils.NodesAreEqual(diff, outputDocument));
    }

    [TestMethod]
    public void TestModManager()
    {
        var expectedDocument =
            GetSettings("ModManagerCurrent");
        var comparisonDocument =
            GetSettings("ModManagerNew");

        var outputDocument =
            GetSettings("ModManagerDiff");

        var diff = XmlUtils.GenerateDiff(expectedDocument, comparisonDocument);
        Assert.IsTrue(XmlUtils.NodesAreEqual(diff, outputDocument));
    }
    
    [TestMethod]
    public void TestCollectionNoDiff()
    {
        var expectedDocument =
            GetSettings("DubsHygeine");

        var diff = XmlUtils.GenerateDiff(expectedDocument, expectedDocument);
        Assert.IsNull(diff);
    }
}