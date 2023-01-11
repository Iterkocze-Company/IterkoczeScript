using IterkoczeScript.Errors;
using IterkoczeScript.Interpreter;

namespace IterkoczeScript.Functions;

public static class IO {
    public static object? WriteFile(object?[] args) {
        if (args.Length != 2)
            _ = new RuntimeError("Function \"WriteToFile\" expects 2 arguments.");
        File.WriteAllText(args[0].ToString(), args[1].ToString());

        return args[1];
    }
    public static object? ReadFile(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError("Function \"ReadFromFile\" expects 1 argument.");

        string content = "";

        try {
            File.ReadAllText(args[0].ToString());
        } catch(Exception ex) {
            IError err = new ErrorFileNotFound();
            err.SetError();
            return err;
        }

        return content;
    }
    public static object? FileExists(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError("Function \"FileExists\" expects 1 argument.");

        return File.Exists(args[0].ToString());
    }
}
