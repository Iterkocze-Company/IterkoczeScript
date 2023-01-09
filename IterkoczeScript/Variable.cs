using System.Reflection.Metadata;

namespace IterkoczeScript;

public class Variable {
    public Variable(object? val, bool global = false, bool constant = false) {
        Value = val;
        isGlobal= global;
        isConstant= constant;
    }
    public bool isGlobal { get; set; }
    public bool isConstant { get; set; }
    public object? Value { get; set; }
}

