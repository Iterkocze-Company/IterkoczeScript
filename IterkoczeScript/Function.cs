using IterkoczeScript.Content;

namespace IterkoczeScript;

public class Function
{
    public Dictionary<string, object?> VARS { get; } = new();
    public object[] args { get; set; }
    public string Name {get; set;}
    public object? ReturnValue { get; set; }
    public IterkoczeScriptParser.BlockContext Code {get; set;}

    public Function(string name, IterkoczeScriptParser.BlockContext code)
    {   
        Name = name;
        Code = code;
    }
}