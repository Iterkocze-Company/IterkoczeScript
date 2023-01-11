using IterkoczeScript.Errors;
using IterkoczeScript.Interpreter;
using System.Diagnostics;

namespace IterkoczeScript.Functions; 

public static class Utility {
    private static Stopwatch watch = new();


    public static object? OK(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError("Function \"OK\" expects 1 argument.");
        return !(args[0] is IError);
    }
    public static object? Argument(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError("Function \"Argument\" expects 1 argument.");

        if (CLI.CLI.ProgramArgs.Length <= (int)args[0]) {
            IError err = new ErrorOutOfBounds();
            err.SetError();
            return err;
        }
        return CLI.CLI.ProgramArgs[(int)args[0]];
    }
    public static object? ArgumentCount(object?[] args) {
        if (args.Length != 0)
            _ = new RuntimeError("Function \"ArgumentCount\" expects 0 arguments.");

        return CLI.CLI.ProgramArgs.Length;
    }
    public static object? Exit(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError("Function \"Exit\" expects 1 argument.");

        Environment.Exit((int)args[0]);
        return null;
    }
    public static object? StartRuntimeTimer(object?[] args) {
        watch.Start();
        return null;
    }
    public static object? StopRuntimeTimer(object?[] args) {
        watch.Stop();
        return null;
    }
    public static object? ClearRuntimeTimer(object?[] args) {
        watch.Reset();
        return null;
    }
    public static object? GetRuntime(object?[] args) {
        return watch.ElapsedMilliseconds;
    }
    //Seems to not work with Windows a lot. Haven't tested on Linux
    public static object? Execute(object?[] args) { 
        if (args.Length != 1)
            _ = new RuntimeError("Function \"Execute\" expects 1 argument.");
        Process p = new Process();
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.FileName = args[0].ToString();
        try {
            p.Start();
        }
        catch (Exception ex) {
            IError err = new ErrorFileNotFound();
            err.SetError();
            return err;
        }
        string output = p.StandardOutput.ReadToEnd();
        p.WaitForExit();
        return output;
    }
    public static object? Linux(object?[] args) {
        return OperatingSystem.IsLinux();
    }
}
