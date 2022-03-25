using Antlr4.Runtime;
using IterkoczeScript;
using IterkoczeScript.Content;

public static class Program
{
    public static string[] ProgramArgs = null;
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            new Error("You need to provide a valid path for a script file.");
        }

        ProgramArgs = new string[args.Length - 1];
        Array.Copy(args, 1, ProgramArgs, 0, ProgramArgs.Length);
        string fileName = args[0];
        string fileContent = File.ReadAllText(fileName);
        string finalContent = String.Empty;

        foreach (string line in fileContent.Split('\n'))
        {
            if (line.Contains("@use"))
            {
                finalContent += File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Lib\\"+line.Remove(0, line.IndexOf(' ')).Replace(";", "").Trim() + ".is");
            }
            if (line.Contains("#use"))
            {
                finalContent += File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + line.Remove(0, line.IndexOf(' ')).Replace(";", "").Trim() + ".is");
            }
        }

        finalContent += fileContent;
        
        AntlrInputStream inputStream = new(finalContent);
        IterkoczeScriptLexer IterkoczeLexer = new(inputStream);
        CommonTokenStream commonTokenStream = new(IterkoczeLexer);
        IterkoczeScriptParser IterkoczeParser = new(commonTokenStream);
        IterkoczeScriptParser.ProgramContext programContent = IterkoczeParser.program();
        IterkoczeScriptVisitor IterkoczeVisitor = new IterkoczeScriptVisitor();
        IterkoczeVisitor.Visit(programContent);
    }
}