namespace IterkoczeScript.Functions;

public static class IO {
    public static object? WriteToFile(object?[] args) {
        if (args.Length != 2)
            _ = new RuntimeError("Function \"WriteToFile\" expects 2 arguments.");
        File.WriteAllText(args[0].ToString(), args[1].ToString());

        return args[1];
    }
    public static object? ReadFromFile(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError("Function \"ReadFromFile\" expects 1 argument.");

        return File.ReadAllText(args[0].ToString());
    }
}
