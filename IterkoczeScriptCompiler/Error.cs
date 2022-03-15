namespace IterkoczeScriptCompiler;

public class Error
{
    public Error(string message)
    {
        var oldColour = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ForegroundColor = oldColour;
        Environment.Exit(1);
    }
}