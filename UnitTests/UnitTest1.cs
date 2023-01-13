namespace UnitTests;

public class UnitTests {
    [Fact]
    public void TestPackageManagerCache() {
        string[] args = {"packsge"};
        IterkoczeScript.CLI.PackageManager.Download(args);
        Assert.False(File.Exists("packages.json.tmp"));
    }
}