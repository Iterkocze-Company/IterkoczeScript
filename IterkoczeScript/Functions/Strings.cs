using IterkoczeScript.Interpreter;

namespace IterkoczeScript.Functions; 

public static class Strings {
    public static object? GetChar(object?[] args) {
        if (args.Length != 2)
            _ = new RuntimeError($"Function \"GetChar\" takes 2 arguments but got {args.Length}.");

        return args[0].ToString()[(int)args[1]].ToString();
    }
    public static object? ToLower(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError($"Function \"ToLower\" takes 1 argument.");

        return args[0].ToString().ToLower();
    }
    public static object? ToUpper(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError($"Function \"ToUpper\" takes 1 argument.");

        return args[0].ToString().ToUpper();
    }
    public static object? StringRemove(object?[] args) {
        if (args.Length != 2)
            _ = new RuntimeError($"Function \"StringRemove\" takes 2 arguments. The string target and char to remove");

        return args[0].ToString().Replace(args[1].ToString(), "");
    }
    public static object? StringRemoveFirst(object?[] args) {
        if (args.Length != 2)
            _ = new RuntimeError($"Function \"StringRemoveFirst\" takes 1 argument. The string target");

        return args[0].ToString().Substring(1);
    }
    public static object? StringEndsWith(object?[] args) {
        if (args.Length != 2)
            _ = new RuntimeError($"Function \"StringEndsWith\" takes 2 arguments. The string target and string to check against");

        return args[0].ToString().EndsWith(args[1].ToString());
    }
}
