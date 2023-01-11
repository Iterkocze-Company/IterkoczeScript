namespace IterkoczeScript.Interpreter;

public class Struct {
    public string Name { get; set; }
    public Dictionary<string, object?> Variables { get; set; } = new();

    public Struct(string name, Dictionary<string, object?> vars) {
        Name = name;
        Variables = vars;
    }
}