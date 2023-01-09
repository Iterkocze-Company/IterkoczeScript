using IterkoczeScript.Errors;
using System.Diagnostics;

namespace IterkoczeScript.Functions;

public class StandardFunctions {
    public static object? Write(object?[] args) {
        if (args.Length > 2)
            _ = new RuntimeError("Function \"Write\" expects 1 argument. And 1 optional argument");

        if (args.Length == 2) {
            var oldColour = Console.ForegroundColor;
            var colour = args[1];
            switch (colour) {
                case ConsoleColor.Red:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case ConsoleColor.Green:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case ConsoleColor.Blue:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                default:
                    new RuntimeError("The colour wasn't defined!");
                    break;

            }
            if (args[0] != null)
                Console.WriteLine(args[0]);
            Console.ForegroundColor = oldColour;
        }
        else {
            if (args[0] != null)
                Console.WriteLine(args[0]);
        }
        return null;
    }
    public static object? WriteToFile(object?[] args)
    {
        if (args.Length != 2)
            _ = new RuntimeError("Function \"WriteToFile\" expects 2 arguments.");
        File.WriteAllText(args[0].ToString(), args[1].ToString());

        return args[1];
    }
    public static object? ReadFromFile(object?[] args)
    {
        if (args.Length != 1)
            _ = new RuntimeError("Function \"ReadFromFile\" expects 1 argument.");

        return File.ReadAllText(args[0].ToString());
    }
    public static object? Read(object?[] args)
    {
        if (args.Length > 1)
            _ = new RuntimeError($"Function \"Read\" takes 1 optional argument but got {args.Length}.");

        if (args.Length == 1)
            Console.Write(args[0]);
        return Console.ReadLine();
    }
    public static object? GetChar(object?[] args)
    {
        if (args.Length != 2)
            _ = new RuntimeError($"Function \"GetChar\" takes 2 arguments but got {args.Length}.");

        return args[0].ToString()[(int)args[1]].ToString();
    }
    public static object? ReadAsInt(object?[] args)
    {
        if (args.Length > 1)
            _ = new RuntimeError($"Function \"ReadAsInt\" takes 1 optional argument but got {args.Length}.");

        if (args.Length == 1)
            Console.Write(args[0]);
        return Convert.ToInt32(Console.ReadLine());
    }
    
    //SECTION Convert Functions
    public static object? ConvertToInt(object?[] args)
    {
        if (args.Length != 1)
           _ = new RuntimeError("Function \"ConvertToInt\" expects 1 argument.");

        if (!int.TryParse(args[0].ToString(), out int output)) {
            IError err = new ErrorConversionFailed();
            err.SetError();
            return err;
        }

        return output;
    }
    public static object? ConvertToString(object?[] args)
    {
        if (args.Length != 1)
            _ = new RuntimeError("Function \"ConvertToString\" expects 1 argument.");

        return args[0].ToString();
    }
    // SECTION: Utility
    private static Stopwatch watch = new();
    public static object? OK(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError("Function \"OK\" expects 1 argument.");
        return !(args[0] is IError);
    }
    public static object? Argument(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError("Function \"Argument\" expects 1 argument.");

        if (Program.ProgramArgs.Length <= (int)args[0])
        {
            IError err = new ErrorConversionFailed();
            err.SetError();
            return err;
        }
        return Program.ProgramArgs[(int)args[0]];
    }
    public static object? ArgumentCount(object?[] args) {
        if (args.Length != 0)
            _ = new RuntimeError("Function \"ArgumentCount\" expects 0 arguments.");

        return Program.ProgramArgs.Length;
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
    public static object? GetRuntime(object?[] args) {
        return watch.ElapsedMilliseconds;
    }
    public static object? Execute(object?[] args) { //Seems to not work with Windows a lot. Haven't tested on Linux
        if (args.Length != 1)
            _ = new RuntimeError("Function \"Execute\" expects 1 argument.");
        Process p = new Process();
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.FileName = args[0].ToString();
        try {
            p.Start();
        } catch(Exception ex)  {
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