using Antlr4.Runtime;
using IterkoczeScript;
using IterkoczeScript.Content;

public static class Program {
    public static string[]? ProgramArgs;
    public static void Main(string[] args) {
        if (args.Length == 0) {
            _ = new RuntimeError("You need to provide a valid path for a script file.");
        }
        if (args.Contains("-v")) {
            Console.WriteLine("Iterkocze IterkoczeScriptInterpreter 1.1.0");
            Environment.Exit(0);
        }
        ProgramArgs = new string[args.Length - 1];
        Array.Copy(args, 1, ProgramArgs, 0, ProgramArgs.Length);
        string fileName = args[0];
        string fileContent = File.ReadAllText(fileName);
        string finalContent = String.Empty;
        string currentlyImportedFileName = "NONE";

        try {
            foreach (string line in fileContent.Split('\n')) {
                if (line.Contains("@use")) {
                    currentlyImportedFileName = "Lib\\" + line.Remove(0, line.IndexOf(' ')).Replace(";", "").Trim() + ".is";
                    finalContent += File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Lib\\" + line.Remove(0, line.IndexOf(' ')).Replace(";", "").Trim() + ".is");
                }
                if (line.Contains("#use")) {
                    currentlyImportedFileName = line.Remove(0, line.IndexOf(' ')).Replace(";", "").Trim() + ".is";
                    finalContent += File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + line.Remove(0, line.IndexOf(' ')).Replace(";", "").Trim() + ".is");
                }
            }
        }
        catch (Exception ex) {
            _ = new RuntimeError($"Can't import {currentlyImportedFileName}. It probably doesn't exist");
        }

        finalContent += fileContent;
        
        AntlrInputStream inputStream = new(finalContent);
        IterkoczeScriptLexer IterkoczeLexer = new(inputStream);
        CommonTokenStream commonTokenStream = new(IterkoczeLexer);

        IterkoczeScriptParser IterkoczeParser = new IterkoczeScriptParser(commonTokenStream);
        IterkoczeParser.AddErrorListener(new ErrorStrategy());

        //IterkoczeScriptParser IterkoczeParser = new(commonTokenStream);
        IterkoczeScriptParser.ProgramContext programContent = IterkoczeParser.program();
        IterkoczeScriptVisitor IterkoczeVisitor = new IterkoczeScriptVisitor();
        IterkoczeVisitor.Visit(programContent);
    }
}