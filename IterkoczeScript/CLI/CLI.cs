using Antlr4.Runtime;
using IterkoczeScript.Content;
using IterkoczeScript.Interpreter;
using IterkoczeScript.Modules;
using Newtonsoft.Json;

namespace IterkoczeScript.CLI;

public static class CLI
{
    public static string[]? ProgramArgs;
    private const string versionInfo = "Iterkocze IterkoczeScript Interpreter 1.2.4";
    public static void Main(string[] args) {
        if (args.Length == 0) {
            Console.WriteLine(versionInfo);
            Console.WriteLine("-v or --version -> Show version");
            Console.WriteLine("-d name or --download name -> Download a package");
            Console.WriteLine("Visit https://iterkoczescriptpackages.xlx.pl for package list");
            Environment.Exit(0);
        }

        if (args.Contains("--http-server")) {
            if (args.Length != 2) {
                Console.WriteLine("Provide the directory to host");
                Environment.Exit(-1);
            }
                
            string[] prefixes = { "http://localhost:8080/" };
            string rootDirectory = args[1];
            HttpServer server = new(prefixes, rootDirectory);
            server.Run();
            while (true) {
                Console.ReadLine();
                break;
            }
        }


        if (args.Contains("-v") || args.Contains("--version")) {
            Console.WriteLine(versionInfo);
            Environment.Exit(0);
        }
        if (args.Contains("-d") || args.Contains("--download")) {
            PackageManager.Download(args);
        }

        ProgramArgs = new string[args.Length - 1];
        Array.Copy(args, 1, ProgramArgs, 0, ProgramArgs.Length);
        string fileName = args[0];
        string fileContent = "";
        try  {
            fileContent = File.ReadAllText(fileName);
        } catch (Exception e) {
            Console.WriteLine($"Can't open {fileName}");
            Environment.Exit(-1);
        }
        string finalContent = string.Empty;
        string currentlyImportedFileName = "NONE";

        try {
            foreach (string line in fileContent.Split('\n'))  {
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
        catch {
            _ = new RuntimeError($"Can't import {currentlyImportedFileName}. It probably doesn't exist");
        }

        finalContent += fileContent;

        AntlrInputStream inputStream = new(finalContent);
        IterkoczeScriptLexer IterkoczeLexer = new(inputStream);
        CommonTokenStream commonTokenStream = new(IterkoczeLexer);

        IterkoczeScriptParser IterkoczeParser = new(commonTokenStream);
        IterkoczeParser.AddErrorListener(new ErrorStrategy());

        //IterkoczeScriptParser IterkoczeParser = new(commonTokenStream);
        IterkoczeScriptParser.ProgramContext programContent = IterkoczeParser.program();
        IterkoczeScriptVisitor IterkoczeVisitor = new();
        IterkoczeVisitor.Visit(programContent);
    }
}