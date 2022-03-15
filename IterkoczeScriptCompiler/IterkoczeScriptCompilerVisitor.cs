using IterkoczeScriptCompiler.Content;

namespace IterkoczeScriptCompiler;

public class IterkoczeScriptCompilerVisitor : IterkoczeScriptBaseVisitor<object?>
{
    private static string _path = Program.outFileName;

    private static void Write(string text)
    {
        File.AppendAllText(_path, "\n\t\t" + text);
    }
    
    public IterkoczeScriptCompilerVisitor()
    {
        File.Create(_path).Close();
        //File.AppendAllText(_path, "class Output\n{\n\tstatic public function main():Void\n\t{");
        File.AppendAllText(_path, "class Output\n{\n\t");
    }

    public override object? VisitFunctionCall(IterkoczeScriptParser.FunctionCallContext context)
    {
        var name = context.IDENTIFIER().GetText();
        var args = context.expression().Select(Visit).ToArray();

        if (name != "Write")
        {
            var passArgs = string.Empty;
            Write($"var {name + "Args"} = new Array<Dynamic>();");
            foreach (var VARIABLE in args)
            {
                Write($"{name + "Args"}.push({VARIABLE});");
                passArgs += VARIABLE + ",";
            }
            passArgs = passArgs.Remove(passArgs.Length - 1);
        
            Write(name + $"({name + "Args"});");
            return null;
        }
        
        if (name == "Write")
        {
            var passArgs2 = string.Empty;
            foreach (var VARIABLE in args)
            {
                passArgs2 += VARIABLE + ",";
            }
            passArgs2 = passArgs2.Remove(passArgs2.Length - 1);
            if (name != "Write")
                Write($"Sys.println({name}({name + "Args"}));");
            else
            {
                if (passArgs2 != String.Empty)
                {
                    Write($"Sys.println({passArgs2});");
                }
            }
            return null;
        }
        
        return null;
    }

    public override object? VisitFunctionDefinition(IterkoczeScriptParser.FunctionDefinitionContext context)
    {
        var name = context.IDENTIFIER().GetText();
        

        if (name == "Main")
        {
            Write($"static public function main():Void");
            Write("{");
            Visit(context.block());
            //Write("}");
            return null;
        }
        
        Write($"static dynamic public function {name}(args:Array<Dynamic>)");
        Write("{");
        Visit(context.block());
        Write("}");
        
        return null;
    }

    public override object? VisitReturnStatement(IterkoczeScriptParser.ReturnStatementContext context)
    {
        Write($"return {Visit(context.expression())};");
        return null;
    }

    public override object? VisitArgumentIdentifierExp(IterkoczeScriptParser.ArgumentIdentifierExpContext context)
    {
        int index = int.Parse(context.INTEGER().GetText());
        
        return $"args[{index}]";
    }

    public override object? VisitConstant(IterkoczeScriptParser.ConstantContext context)
    {
        if (context.INTEGER() is { } i)
        {
            return int.Parse(i.GetText());
        }
        if (context.FLOAT() is { } f)
        {
            return float.Parse(f.GetText());
        }
        if (context.STRING() is { } s)
        {
            return s.GetText();
        }
        if (context.BOOLEAN() is { } b)
        {
            return b.GetText() == "true";
        }
        if (context.NULL() is { })
        {
            return null;
        }

        throw new NotImplementedException();
    }
}