using Antlr4.Runtime.Sharpen;
using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;

namespace IterkoczeScript.Interpreter;

class ErrorStrategy : BaseErrorListener {
    public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
    {
        var oldColour = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("[SYNTAX ERROR] " + msg + " line " + line + " character " + charPositionInLine);
        Console.ForegroundColor = oldColour;

        Environment.Exit(-1);
        base.SyntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e);
    }
}
