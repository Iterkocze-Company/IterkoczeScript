using IterkoczeScript.Errors;
using IterkoczeScript.Interpreter;

namespace IterkoczeScript.Functions;

public static class Basic {
    public static object? Write(object?[] args) {
        if (args.Length == 0 || args.Length > 2)
            _ = new RuntimeError("Function \"Write\" expects 1 argument. And 1 optional argument");

        if (args.Length == 2) {
            var oldColour = Console.ForegroundColor;
            ConsoleColor colour = (ConsoleColor)(args[1]);
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

    public static object? Read(object?[] args) {
        if (args.Length > 1)
            _ = new RuntimeError($"Function \"Read\" takes 1 optional argument but got {args.Length}.");

        if (args.Length == 1)
            Console.Write(args[0]);
        return Console.ReadLine();
    }


    public static object? ReadAsInt(object?[] args) {
        if (args.Length > 1)
            _ = new RuntimeError($"Function \"ReadAsInt\" takes 1 optional argument but got {args.Length}.");

        if (args.Length == 1)
            Console.Write(args[0]);

        try { return Convert.ToInt32(Console.ReadLine()); }
        catch {
            IError err = new ErrorConversionFailed();
            err.SetError();
            return err;
        }
    }
}

