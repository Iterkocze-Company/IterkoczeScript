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
}
