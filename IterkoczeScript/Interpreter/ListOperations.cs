using Antlr4.Runtime;

namespace IterkoczeScript.Interpreter;

public static class ListOperations {

    public static object? Invoke(List<object?> List, object[]? val, string operation, string? subjectName = null, ParserRuleContext? ctx = null)  {
        switch (operation)  {
            case "Add":
                List.Add(val[0]);
                break;
            case "Remove":
                List.Remove(val[0]);
                break;
            case "IndexOf":
                return List.IndexOf(val[0]);
            case "Contains":
                return List.Contains(val[0]);
            case "Clear":
                List.Clear();
                break;
            case "Sort":
                try {
                    List.Sort();
                }
                catch { _ = new RuntimeError($"List {subjectName} contains values of not the same type, but was attempted to be sorted", ctx); }
                return 0;
        }
        return 0;
    }
}
