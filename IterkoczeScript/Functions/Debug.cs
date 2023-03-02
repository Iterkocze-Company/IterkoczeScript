using IterkoczeScript.Interpreter;

namespace IterkoczeScript.Functions; 
internal class Debug {
    public static object? DebugDump(object?[] args) {
        if (args.Length > 0)
            _ = new RuntimeError($"Function \"DebugDump\" takes 0 arguments but got {args.Length}.");

        Console.WriteLine($"DebugDump.\nList of all variables in currect scope: {IterkoczeScriptVisitor.currentFunction.Name}");
        foreach (var var in IterkoczeScriptVisitor.currentFunction.VARS) {
            Console.WriteLine($"{var} holds value: " + var.Value.Value + $" Is constant? {var.Value.isConstant} Is global? {var.Value.isGlobal} defined at line {var.Value.DefinedOnLine}");
        }

        Console.WriteLine($"List of all language features enabled in the current scope: {IterkoczeScriptVisitor.currentFunction.Name}:");
        foreach (var feature in IterkoczeScriptVisitor.currentFunction.FeatureEnabled) {
            Console.WriteLine(feature);
        }

        Console.WriteLine("List of global language features: ");
        Console.WriteLine($"Silent? {IterkoczeScriptVisitor.IsSilent}");

        return new Variable("None", true, true);
    }
}
