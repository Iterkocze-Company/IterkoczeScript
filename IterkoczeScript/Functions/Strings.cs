namespace IterkoczeScript.Functions; 

public static class Strings {
    public static object? GetChar(object?[] args) {
        if (args.Length != 2)
            _ = new RuntimeError($"Function \"GetChar\" takes 2 arguments but got {args.Length}.");

        return args[0].ToString()[(int)args[1]].ToString();
    }
}
