using System.Diagnostics;
using Antlr4.Runtime;
using IterkoczeScriptCompiler.Content;

namespace IterkoczeScriptCompiler
{
    public static class Program
    {
        public static string outFileName = "Output.hx";
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                new Error("You need to provide a valid path for a script file.");
            }
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
            IterkoczeScriptCompilerVisitor IterkoczeVisitor = new IterkoczeScriptCompilerVisitor();
            IterkoczeVisitor.Visit(programContent);
            File.AppendAllText(outFileName, "\n\t}\n}\n\t");
            //File.AppendAllText(outFileName, "\n}");

            Process.Start("haxe", $"-main {outFileName} --cs compile");
        }
    }
}