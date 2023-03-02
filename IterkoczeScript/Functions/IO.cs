using IterkoczeScript.Errors;
using IterkoczeScript.Interpreter;

namespace IterkoczeScript.Functions;

public static class IO {
    public static object? FileWrite(object?[] args) {
        if (args.Length != 2)
            _ = new RuntimeError("Function \"WriteToFile\" expects 2 arguments.");
        File.WriteAllText(args[0].ToString(), args[1].ToString());

        return args[1];
    }
    public static object? FileRead(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError("Function \"ReadFromFile\" expects 1 argument.");

        string content = "";

        try {
            File.ReadAllText(args[0].ToString());
        } catch {
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
    public static object? DirectoryCreate(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError("Function \"DirectoryCreate\" expects 1 argument. The directory path");

        try {
            Directory.CreateDirectory(args[0].ToString());
        }
        catch (Exception e) {
            _ = new RuntimeError($"Can't create directory. " + e.Message);
        }

        return null;
    }
    public static object? AllFilesIn(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError("Function \"AllFilesIn\" expects 1 argument. The directory path");

        var arr = Directory.GetFiles(args[0].ToString());

        return arr;
    }
    public static object? FileMove(object?[] args) {
        if (args.Length != 2)
            _ = new RuntimeError("Function \"FileMove\" expects 2 arguments. The file to move and destination");

        try {
            File.Move(args[0].ToString(), args[1].ToString());
        } catch (Exception e) {
            _ = new RuntimeError($"Can't move file {args[0].ToString()} to {args[1].ToString()} " + e.Message);
        }

        return null;
    }
    public static object? FileGetName(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError("Function \"FileGetName\" expects1 argument. The file name");

        return Path.GetFileName(args[0].ToString());
    }
    public static object? FileGetExtention(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError($"Function \"FileGetExtention\" takes 1 arguments. The string target");

        return Path.GetExtension(args[0].ToString());
    }
    public static object? GetConfigDirectory(object?[] args) {
        if (args.Length > 0)
            _ = new RuntimeError($"Function \"GetConfigDirectory\" takes 0 arguments, but got {args.Length}");
        if (OperatingSystem.IsLinux())
            return "~/.config/";
        return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    }
}
