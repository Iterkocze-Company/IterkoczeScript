using Newtonsoft.Json;

namespace IterkoczeScript.CLI;

public static class PackageManager {
    public static void Download(string[] args) {
        if (args.Length != 2) {
            Console.WriteLine("You didn't provided a valid package name");
            Environment.Exit(-1);
        }
        string whatPackageToDownload = args[1];
        const string url = "https://iterkoczescriptpackages.xlx.pl/packages.json";
        Console.WriteLine("Downloading database file...");
        try {
            using var client = new HttpClient();
            using var s = client.GetStreamAsync(url);
            using var fs = new FileStream("packages.json.tmp", FileMode.OpenOrCreate);
            s.Result.CopyTo(fs);
        }
        catch (Exception e) {
            Console.WriteLine("Can't download package. Fck you");
            Environment.Exit(-1);
        }
        Console.WriteLine("Dwonloaded database file");
        Console.WriteLine("Reading the database file...");
        using (StreamReader r = new StreamReader("packages.json.tmp")) {
            string json = r.ReadToEnd();
            List<PackageItemJSON> items = JsonConvert.DeserializeObject<List<PackageItemJSON>>(json);
            foreach (var item in items) {
                if (item.Package.Name == whatPackageToDownload) {
                    File.WriteAllText($"Lib/{whatPackageToDownload}.is", item.Package.Src);
                    Console.WriteLine("Package downloaded");
                    goto cleanup;
                }
            }
            Console.WriteLine($"Package {whatPackageToDownload} wasn't found in the database");
        }
        cleanup:
        File.Delete("packages.json.tmp");
        Environment.Exit(0);
    }
}
