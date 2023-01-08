using Antlr4.Runtime;

namespace IterkoczeScript;

public class Error {
    public Error(string message, ParserRuleContext ctx = null) {
        var oldColour = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("[RUNTIME ERROR] " + message);
        if (ctx != null) Console.Write(" At line " + ctx.start.Line + "\n" + ctx.start);
        Console.ForegroundColor = oldColour;
        Environment.Exit(-1);
    }
}