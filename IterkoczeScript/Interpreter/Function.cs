using IterkoczeScript.Content;

namespace IterkoczeScript.Interpreter;

public class Function {
    public Dictionary<string, Variable> VARS { get; set; } = new();
    public object[] Args { get; set; }
    public string Name { get; set; }
    public object? ReturnValue { get; set; }
    public IterkoczeScriptParser.BlockContext Code { get; set; }
    public List<Struct> Structs { get; set; } = new();
    public Dictionary<string, Struct> StructInstances { get; set; } = new();
    public Dictionary<string, Array> Arrays { get; set; } = new();
    public Dictionary<string, List<object?>> Lists { get; set; } = new();
    public Dictionary<string, Dictionary<string, object?>> Dictionaries { get; } = new();
    public List<Features> FeatureEnabled = new();


    public enum Features {
        MichauScript,
        Silent
    }

    public Function(string name, IterkoczeScriptParser.BlockContext code) {
        Name = name;
        Code = code;
    }
}