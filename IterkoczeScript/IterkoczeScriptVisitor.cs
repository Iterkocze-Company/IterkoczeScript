using System.Net.Sockets;
using Antlr4.Runtime.Atn;
using IterkoczeScript.Content;

namespace IterkoczeScript;

public class IterkoczeScriptVisitor : IterkoczeScriptBaseVisitor<object?>
{
    private Dictionary<string, object?> VARS { get; } = new();
    private Dictionary<string, object?> STANDARD_FUNCTIONS { get; } = new();
    private List<Function> FUNCTIONS { get; } = new();
    private Function currentFunction = null;

    public IterkoczeScriptVisitor()
    {
        VARS["PI"] = Math.PI;

        STANDARD_FUNCTIONS["Write"] = new Func<object?[], object?>(StandardFunctions.Write);
        STANDARD_FUNCTIONS["WriteToFile"] = new Func<object?[], object?>(StandardFunctions.WriteToFile);
        STANDARD_FUNCTIONS["Read"] = new Func<object?[], object?>(StandardFunctions.Read);
        STANDARD_FUNCTIONS["ReadAsInt"] = new Func<object?[], object?>(StandardFunctions.ReadAsInt);
    }

    public override object? VisitAssingment(IterkoczeScriptParser.AssingmentContext context)
    {
        var varName = context.IDENTIFIER().GetText();
        var value = Visit(context.expression());
        currentFunction.VARS[varName] = value;
        
        return null;
    }

    public override object? VisitFunctionCall(IterkoczeScriptParser.FunctionCallContext context)
    {
        var name = context.IDENTIFIER().GetText();
        var args = context.expression().Select(Visit).ToArray();

        foreach (Function function in FUNCTIONS)
        {
            if (function.Name == name)
            {
                function.args = args;
                currentFunction = function;
                VisitBlock(function.Code);
                currentFunction = null;
                return function.ReturnValue;
            }
        }
        
        if (!STANDARD_FUNCTIONS.ContainsKey(name))
            new Error($"Function {name} is not defined!");

        if (STANDARD_FUNCTIONS[name] is not Func<object?[], object?> func)
            throw new Exception($"variable {name} is not a function.");
            
        return func(args);
    }

    public override object? VisitReturnStatement(IterkoczeScriptParser.ReturnStatementContext context)
    { 
        currentFunction.ReturnValue = Visit(context.expression());
        return null;
    }

    public override object? VisitFunctionDefinition(IterkoczeScriptParser.FunctionDefinitionContext context)
    {
        var name = context.IDENTIFIER().GetText();
        var code = context.block();
        
        if (VARS.ContainsKey(name))
            new Error($"Function {name} was already defined!");
        
        FUNCTIONS.Add(new Function(name, code));

        return null;
    }

    public override object? VisitArgumentIdentifierExp(IterkoczeScriptParser.ArgumentIdentifierExpContext context)
    {
        return currentFunction.args[int.Parse(context.INTEGER().GetText())];
    }

    public override object? VisitIdentifierExp(IterkoczeScriptParser.IdentifierExpContext context)
    {
        var varName = context.IDENTIFIER().GetText();

        if (!currentFunction.VARS.ContainsKey(varName))
        {
            new Error($"Variable {varName} is not defined!");
        }

        return currentFunction.VARS[varName];
    }

    public override object? VisitAddExp(IterkoczeScriptParser.AddExpContext context)
    {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));

        var op = context.addOp().GetText();

        return op switch
        {
            "+" => IterkoczeMath.Add(left, right),
            //"-" => Subtract(left, right),
            _ => throw new NotImplementedException()
        };
    }

    public override object? VisitMulExp(IterkoczeScriptParser.MulExpContext context)
    {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));

        var op = context.mulOp().GetText();

        return op switch
        {
            "*" => IterkoczeMath.Multiply(left, right),
            //"/" => Subtract(left, right),
            _ => throw new NotImplementedException()
        };
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
            return s.GetText()[1..^1];
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

    public override object? VisitWhileBlock(IterkoczeScriptParser.WhileBlockContext context)
    {
        Func<object?, bool> condition = context.WHILE().GetText() == "while"
                ? IsTrue
                : IsFalse
            ;

        if(condition(Visit(context.expression())))
        {
            do
            {
                Visit(context.block());
            } while (condition(Visit(context.expression())));
        }
        else
        {
            Visit(context.elseIfBlock());
        }

        return null;
    }

    public override object? VisitCompareExp(IterkoczeScriptParser.CompareExpContext context)
    {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));

        var op = context.compareOp().GetText();

        return op switch
        {
            //"==" => IsEqual(left, right),
            //"!=" => IsEqual(left, right),
            ">" => Compare.GreaterThan(left, right),
            "<" => Compare.LessThan(left, right),
            //">=" => IsEqual(left, right),
            //"<=" => IsEqual(left, right),
            _ => throw new NotImplementedException()
        };
    }

    private bool IsTrue(object? value)
    {
        if (value is bool b)
            return b;

        new Error("Value is not boolean.");
        return false;
    }

    private bool IsFalse(object? value) => !IsTrue(value);
}