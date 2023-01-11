using IterkoczeScript.Interpreter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IterkoczeScript.Functions;
public static class Json {
    public static object? Json1(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError("Function \"Json\" expects 1 argument. Path to the .json file");
        if (!File.Exists(args[0].ToString())) {
            _ = new RuntimeError($"Can't open the json file {args[0]}");
        }
        try {
            return JObject.Parse(File.ReadAllText(args[0].ToString()));
        }
        catch (Exception e) {
            _ = new RuntimeError($"Json file {args[0]} was not formatted properly");
        }
        return null;
    }
    public static object? JsonRead(object?[] args) {
        if (args.Length != 2)
            _ = new RuntimeError("Function \"JsonRead\" expects 2 arguments. The json data and the key to read");

        JObject? json = null;
        try {
            json = (JObject)args[0];
        }
        catch (Exception e) {
            _ = new RuntimeError($"{args[0]} is not a falid Json data variable");
        }

        var res = json[args[1].ToString()];

        if (res == null) {
            _ = new RuntimeError($"{args[1]} does not exist in the provided json variable");
        }

        return res;
    }
}

