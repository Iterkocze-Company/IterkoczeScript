using System.Text;
using System.Security.Cryptography;

namespace IterkoczeScript.Functions;

public static class Security {
    public static object? SHA1(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError($"Function \"SHA1\" takes 1 argument.");

        return ToSHA1(args[0].ToString());
    }

    public static object? IterkoczeUUID(object?[] args) {
        return GenerateHaphazardUUID();
    }

    private static string ToSHA1(string input) {
        using (SHA1Managed sha1 = new SHA1Managed()) {
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder(hash.Length * 2);

            foreach (byte b in hash){
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }
    }

    // Very secure. I promise
    private static string GenerateHaphazardUUID() {
        var timestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        var random = new Random();
        var rand = random.Next();
        return timestamp.ToString() + rand.ToString();
    }

}
