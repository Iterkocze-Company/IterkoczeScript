﻿using IterkoczeScript.Interpreter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;
using System.Text;

namespace IterkoczeScript.Functions;
public static class Json {
    public static object? Json1(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError("Function \"Json\" expects 1 argument. Path to the .json file");
        if (!File.Exists(args[0].ToString()))
            File.Create(args[0].ToString());
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
        catch {
            _ = new RuntimeError($"{args[0]} is not a falid Json data variable");
        }

        var res = json[args[1].ToString()];

        if (res == null) {
            _ = new RuntimeError($"{args[1]} does not exist in the provided json variable");
        }

        return res;
    }
    public static object? JsonWrite(object?[] args) {
        if (args.Length != 2)
            _ = new RuntimeError("Function \"JsonWrite\" expects 2 arguments. The json file name and a dictionary containing the data");

        StringBuilder sb = new StringBuilder();
        StringWriter sw = new StringWriter(sb);

        var dic = (Dictionary<string, object?>)args[1];


        using (JsonWriter writer = new JsonTextWriter(sw)) {
            writer.Formatting = Formatting.Indented;

            writer.WriteStartObject();
            foreach (var element in dic) {
                writer.WritePropertyName(element.Key.Replace("\"", ""));
                writer.WriteValue(element.Value);
            }
            writer.WriteEndObject();
        }

        try {
            File.WriteAllText(args[0].ToString(), sw.ToString());
        }
        catch {
            _ = new RuntimeError($"Can't write to {args[0]}");
        }


        return null;
    }
    public static object? JsonArrayToArray(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError("Function \"JsonArrayToArray\" expects 1 argument. The Json array");

        
        var v = ((JArray)args[0]).ToArray();
        var ret = new string[v.Length];
        int i = 0;
        foreach (var vv in v) {
            ret[i] = vv.Value<string>();
            i++;
        }

        return ret;
    }

}

