using Antlr4.Runtime;
using IterkoczeScript;
using IterkoczeScript.Content;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            new Error("You need to provide a valid path for a script file.");
        }
        string fileName = args[0];
        string fileContent = File.ReadAllText(fileName);
        
        AntlrInputStream inputStream = new(fileContent);
        IterkoczeScriptLexer IterkoczeLexer = new(inputStream);
        CommonTokenStream commonTokenStream = new(IterkoczeLexer);
        IterkoczeScriptParser IterkoczeParser = new(commonTokenStream);
        IterkoczeScriptParser.ProgramContext programContent = IterkoczeParser.program();
        IterkoczeScriptVisitor IterkoczeVisitor = new IterkoczeScriptVisitor();
        IterkoczeVisitor.Visit(programContent);
    }
}