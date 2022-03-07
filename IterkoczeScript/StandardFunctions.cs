namespace IterkoczeScript;

public class StandardFunctions
{
    public static object? Write(object?[] args)
    {
        if (args.Length != 1)
            new Error("Function \"Write\" expects 1 argument.");
        foreach (var arg in args)
        {
            Console.WriteLine(arg);
        }

        return null;
    }
    public static object? WriteToFile(object?[] args)
    {
        if (args.Length != 2)
            new Error("Function \"WriteToFile\" expects 2 arguments.");
        File.WriteAllText(args[0].ToString(), args[1].ToString());

        return args[1];
    }
    public static object? Read(object?[] args)
    {
        if (args.Length > 1)
            new Error($"Function \"Read\" takes 1 optional argument but got {args.Length}.");

        if (args.Length == 1)
            Console.Write(args[0]);
        return Console.ReadLine();
    }
    public static object? ReadAsInt(object?[] args)
    {
        if (args.Length > 1)
            new Error($"Function \"ReadAsInt\" takes 1 optional argument but got {args.Length}.");

        if (args.Length == 1)
            Console.Write(args[0]);
        return Convert.ToInt32(Console.ReadLine());
    }
}