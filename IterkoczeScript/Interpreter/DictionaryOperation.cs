using Antlr4.Runtime;

namespace IterkoczeScript.Interpreter; 

internal class DictionaryOperation {
    public static object? Invoke(Dictionary<string, object?> dic, object[]? val, string operation, string? subjectName = null, ParserRuleContext? ctx = null) {
        switch (operation) {
            case "ContainsKey":
                return dic.ContainsKey(val[0].ToString());
        }
        return 0;
    }

}
