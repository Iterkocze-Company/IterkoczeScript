using IterkoczeScript.Content;

namespace IterkoczeScript;

public class Function
{
    public Dictionary<string, object?> VARS { get; set; } = new();
    public object[] args { get; set; }
    public string Name {get; set;}
    public object? ReturnValue { get; set; }
    public List<Struct> Structs { get; set; } = new();
    public Dictionary<string, Struct> StructInstances { get; set; } = new();
    public IterkoczeScriptParser.BlockContext Code {get; set;}

    public Function(string name, IterkoczeScriptParser.BlockContext code)
    {   
        Name = name;
        Code = code;
    }
}