using System.Reflection.Metadata;

namespace IterkoczeScript.Interpreter;

public class Variable {
    public Variable(object? val, bool global = false, bool constant = false, int line = -1)  {
        Value = val;
        isGlobal = global;
        isConstant = constant;
        DefinedOnLine = line;
    }
    public bool isGlobal { get; set; }
    public bool isConstant { get; set; }
    public object? Value { get; set; }
    public int? DefinedOnLine { get; set; }

}

