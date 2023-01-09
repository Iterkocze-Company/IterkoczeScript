using IterkoczeScript.Errors;
using System.Media;

namespace IterkoczeScript;

public class StandardFunctions
{
    public static object? Write(object?[] args)
    {
        if (args.Length > 2)
            new Error("Function \"Write\" expects 1 argument. And 1 optional argument");

        if (args.Length == 2)
        {
            var oldColour = Console.ForegroundColor;
            var colour = args[1];
            switch (colour)
            {
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
                    new Error("The colour wasn't defined!");
                    break;
                        
            }
            if (args[0] != null)
                Console.WriteLine(args[0]);
            Console.ForegroundColor = oldColour;
        }
        else
        {
            if (args[0] != null)
                Console.WriteLine(args[0]);
        }

        return null;
    }
    public static object? WriteToFile(object?[] args)
    {
        if (args.Length != 2)
            _ = new Error("Function \"WriteToFile\" expects 2 arguments.");
        File.WriteAllText(args[0].ToString(), args[1].ToString());

        return args[1];
    }
    public static object? ReadFromFile(object?[] args)
    {
        if (args.Length != 1)
            new Error("Function \"ReadFromFile\" expects 1 argument.");
        
        return File.ReadAllText(args[0].ToString());
    }
    public static object? Read(object?[] args)
    {
        if (args.Length > 1)
            new Error($"Function \"Read\" takes 1 optional argument but got {args.Length}.");

        if (args.Length == 1)
            Console.Write(args[0]);
        return Console.ReadLine();
    }
    public static object? GetChar(object?[] args)
    {
        if (args.Length != 2)
            new Error($"Function \"GetChar\" takes 2 arguments but got {args.Length}.");

        return args[0].ToString()[(int)args[1]].ToString();
    }
    public static object? ReadAsInt(object?[] args)
    {
        if (args.Length > 1)
            new Error($"Function \"ReadAsInt\" takes 1 optional argument but got {args.Length}.");

        if (args.Length == 1)
            Console.Write(args[0]);
        return Convert.ToInt32(Console.ReadLine());
    }
    public static object? Exit(object?[] args)
    {
        if (args.Length != 1)
            new Error("Function \"Exit\" expects 1 argument.");

        Environment.Exit((int)args[0]);
        return null;
    }
    // Arguments
    public static object? Argument(object?[] args) {
        if (args.Length != 1)
            new Error("Function \"Argument\" expects 1 argument.");

        if (Program.ProgramArgs.Length <= (int)args[0]) {
            IError err = new ErrorConversionFailed();
            err.SetError();
            return err;
        }
        return Program.ProgramArgs[(int)args[0]];
    }
    public static object? ArgumentCount(object?[] args)
    {
        if (args.Length != 0)
            new Error("Function \"ArgumentCount\" expects 0 arguments.");

        return Program.ProgramArgs.Length;
    }
    //Convert Functions
    public static object? ConvertToInt(object?[] args) {
        if (args.Length != 1)
            new Error("Function \"ConvertToInt\" expects 1 argument.");

        if (!int.TryParse(args[0].ToString(), out int output)) {
            IError err = new ErrorConversionFailed();
            err.SetError();
            return err;
        }
 
        return output;
    }
    public static object? ConvertToString(object?[] args) {
        if (args.Length != 1)
            new Error("Function \"ConvertToString\" expects 1 argument.");
 
        return args[0].ToString();
    }
    // SECTION: Error handling
    public static object? OK(object?[] args) {
        return !(args[0] is Errors.IError);
    }
}