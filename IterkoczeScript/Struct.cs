using System.Collections;

namespace IterkoczeScript;

public class Struct : IEnumerable<Struct>
{
    public string Name { get; set; }
    public Dictionary<string, object?> Variables { get; set; } = new();

    public Struct(string name, Dictionary<string, object?> vars)
    {
        Name = name;
        Variables = vars;
    }

    public IEnumerator<Struct> GetEnumerator()
    {
        foreach(var val in Variables.Values)
        {
            yield return (Struct)val;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}