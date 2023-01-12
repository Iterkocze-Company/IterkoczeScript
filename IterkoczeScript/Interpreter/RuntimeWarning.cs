using Antlr4.Runtime;

namespace IterkoczeScript.Interpreter; 

public class RuntimeWarning {
    public RuntimeWarning(string message, ParserRuleContext? ctx = null) {
        if (IterkoczeScriptVisitor.IsSilent) return;
        var oldColour = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("[WARNING] " + message);
        if (ctx != null) Console.Write(" At line " + ctx.start.Line + "\n" + ctx.start + "\n");
        Console.ForegroundColor = oldColour;
    }
}
