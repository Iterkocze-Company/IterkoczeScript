using Antlr4.Runtime.Misc;
using IterkoczeScript.Content;

namespace IterkoczeScript;

public class IterkoczeScriptVisitor : IterkoczeScriptBaseVisitor<object?> {
    private Dictionary<string, object?> VARS { get; } = new();
    private Dictionary<string, object?> PREDEF_VARS { get; } = new();
    private Dictionary<string, object?> STANDARD_FUNCTIONS { get; } = new();
    private List<Function> FUNCTIONS { get; } = new();
    private static Function mainFunction = new("Main", null);
    private Function currentFunction = mainFunction;

    public IterkoczeScriptVisitor()
    {
        PREDEF_VARS["PI"] = Math.PI;
        PREDEF_VARS["RED"] = ConsoleColor.Red;
        PREDEF_VARS["GREEN"] = ConsoleColor.Green;
        PREDEF_VARS["BLUE"] = ConsoleColor.Blue;
        PREDEF_VARS["ERROR"] = "ERROR";
        
        STANDARD_FUNCTIONS["Argument"] = new Func<object?[], object?>(StandardFunctions.Argument);
        STANDARD_FUNCTIONS["ArgumentCount"] = new Func<object?[], object?>(StandardFunctions.ArgumentCount);

        STANDARD_FUNCTIONS["Write"] = new Func<object?[], object?>(StandardFunctions.Write);
        STANDARD_FUNCTIONS["WriteToFile"] = new Func<object?[], object?>(StandardFunctions.WriteToFile);
        STANDARD_FUNCTIONS["ReadFromFile"] = new Func<object?[], object?>(StandardFunctions.ReadFromFile);
        STANDARD_FUNCTIONS["Read"] = new Func<object?[], object?>(StandardFunctions.Read);
        STANDARD_FUNCTIONS["ReadAsInt"] = new Func<object?[], object?>(StandardFunctions.ReadAsInt);
        STANDARD_FUNCTIONS["Exit"] = new Func<object?[], object?>(StandardFunctions.Exit);
        STANDARD_FUNCTIONS["GetChar"] = new Func<object?[], object?>(StandardFunctions.GetChar);
        
        STANDARD_FUNCTIONS["ConvertToInt"] = new Func<object?[], object?>(StandardFunctions.ConvertToInt);
        STANDARD_FUNCTIONS["ConvertToString"] = new Func<object?[], object?>(StandardFunctions.ConvertToString);
    }
    
    public override object? VisitListCreation(IterkoczeScriptParser.ListCreationContext context) {
        var listName = context.IDENTIFIER().GetText();
        currentFunction.Lists[listName] = new List<object?>();
        return null;
    }

    /*public override object? VisitListIndexOfOperation(IterkoczeScriptParser.ListIndexOfOperationContext context) {
        var listName = context.IDENTIFIER().GetText();
        var value = Visit(context.expression());
        if (currentFunction.Lists.ContainsKey(listName))
            return currentFunction.Lists[listName].IndexOf(value);
        else
           _ = new Error($"List {listName} does not exist, but you tried to invoke `IndexOf`", context);
        return null;
    }*/

    public override object? VisitAssingment(IterkoczeScriptParser.AssingmentContext context) {
        var varName = context.IDENTIFIER().GetText();
        var value = Visit(context.expression());
        currentFunction.VARS[varName] = value;

        return currentFunction.VARS[varName];
    }

    public override object? VisitArrayAssingment(IterkoczeScriptParser.ArrayAssingmentContext context) {
        var arrayName = context.IDENTIFIER().GetText();
        var index = context.INTEGER().GetText();
        var value = Visit(context.expression());
            
        try {
            currentFunction.Arrays[arrayName].SetValue(value, int.Parse(index));
        }
        catch {
           _ = new Error($"Index {index} was out of bounds for array {arrayName} of size {Utility.GetRealArrayLenght(currentFunction.Arrays[arrayName])}.", context);
        }
        return null;
    }

    public override object? VisitArrayCreation(IterkoczeScriptParser.ArrayCreationContext context) {
        var arrayName = context.IDENTIFIER().GetText();
        var arraySize = 0;
        if (context.expression() == null) {
           _ = new Error($"Array \"{arrayName}\" has no size at declaration.", context);
        }
        arraySize = (int)Visit(context.expression());
        currentFunction.Arrays[arrayName] = new object?[(int)arraySize];
        return null;
    }

    public override object? VisitArrayAccessExp(IterkoczeScriptParser.ArrayAccessExpContext context) {
        var arrayName = context.IDENTIFIER().GetText();
        var index = Visit(context.expression());

        foreach (var VARIABLE in currentFunction.Arrays) { //If it's an array
            if (VARIABLE.Key == arrayName)
            try {
                return currentFunction.Arrays[arrayName].GetValue((int)index);
            }
            catch {
                _ = new Error($"Index {index} was out of bounds for array {arrayName} of size {Utility.GetRealArrayLenght(currentFunction.Arrays[arrayName])}.", context);
            }
        }
        foreach (var VARIABLE in currentFunction.Lists) { //If it's a List
            if (VARIABLE.Key == arrayName) {
                if (currentFunction.Lists[arrayName].Count <= (int)index) {
                   _ = new Error($"Index {index} was out of bounds in the List {arrayName}!");
                }
                return currentFunction.Lists[arrayName][(int)index];
            }
        }
        return null;
    }

    public override object? VisitArrayStructMemberAccess(IterkoczeScriptParser.ArrayStructMemberAccessContext context) {
        var arrayName = context.IDENTIFIER(0).GetText();
        var index = Visit(context.expression());
        var structMemberName = context.IDENTIFIER(1).GetText();
        
        if (currentFunction.Arrays[arrayName].GetValue((int) index) is Struct) {
            Struct structInstance = (Struct)currentFunction.Arrays[arrayName].GetValue((int) index);
            return structInstance.Variables[structMemberName];
        }
        return currentFunction.Arrays[arrayName].GetValue((int) index);
    }

    public override object? VisitArrayStructMemberAccessAssingment(IterkoczeScriptParser.ArrayStructMemberAccessAssingmentContext context) {
        var arrayName = context.IDENTIFIER(0).GetText();
        var index = context.INTEGER().GetText();
        var structMemberName = context.IDENTIFIER(1).GetText();
        var value = Visit(context.expression());

        currentFunction.Arrays[arrayName].SetValue(value, int.Parse(index));
        return null;
    }

    public override object? VisitForBlock(IterkoczeScriptParser.ForBlockContext context) {
        int indexer = (int)VisitAssingment(context.assingment());
        var condition = VisitExpression(context.expression());
        int adder = int.Parse(context.INTEGER().GetText());
        var op = context.GetText();

        currentFunction.VARS[context.assingment().IDENTIFIER().ToString()] = indexer;
        
        if (op.Contains(">=")) 
            goto GreaterThanOrEqual;
        if (op.Contains("<=")) 
            goto LessThanOrEqual;
        if (op.Contains('<')) 
            goto LessThan;
        if (op.Contains('>')) 
            goto GreaterThan;
        
        LessThan:
        for (int i = indexer; i < (int)condition; i += adder) {
            VisitBlock(context.block());
            currentFunction.VARS[context.assingment().IDENTIFIER().ToString()] = Convert.ToInt32(indexer)+adder;
            indexer += adder;
        }
        goto End;
        GreaterThan:
        for (int i = indexer; i > (int)condition; i += adder) {
            VisitBlock(context.block());
            currentFunction.VARS[context.assingment().IDENTIFIER().ToString()] = Convert.ToInt32(indexer)+adder;
            indexer += adder;
        }
        goto End;
        GreaterThanOrEqual:
        for (int i = indexer; i >= (int)condition; i += adder) {
            VisitBlock(context.block());
            currentFunction.VARS[context.assingment().IDENTIFIER().ToString()] = Convert.ToInt32(indexer)+adder;
            indexer += adder;
        }
        goto End;
        LessThanOrEqual:
        for (int i = indexer; i <= (int)condition; i += adder) {
            VisitBlock(context.block());
            currentFunction.VARS[context.assingment().IDENTIFIER().ToString()] = Convert.ToInt32(indexer)+adder;
            indexer += adder;
        }
        End:
        currentFunction.VARS.Remove(context.assingment().IDENTIFIER().ToString());
        return null;
    }

    public override object? VisitBlock(IterkoczeScriptParser.BlockContext context) {
        foreach (var line in context.line()) {
            try {
                if (line.statement().structMemberDefinition() != null) {
                    Dictionary<string, object?> vars = new();
                    foreach (var line2 in context.line()) {
                        if (line2.statement().structMemberDefinition() != null) {
                            vars.Add(line2.statement().structMemberDefinition().IDENTIFIER().GetText(), null);
                        }
                    }
                    return vars;
                }
            }
            catch {
                //new Error("We need to fix something");
            }
        }
        
        return base.VisitBlock(context);
    }

    public override object? VisitStructDefinition(IterkoczeScriptParser.StructDefinitionContext context)
    {
        var vars = VisitBlock(context.block());
        var structName = context.IDENTIFIER().GetText();
        
        currentFunction.Structs.Add(new Struct(structName, (Dictionary<string, object?>)vars));
        
        return null;
    }

    public override object? VisitStructCreation(IterkoczeScriptParser.StructCreationContext context)
    {
        var structInstanceName = context.IDENTIFIER(1).GetText();
        var structTarget = context.IDENTIFIER(0).GetText();

        foreach (var st in currentFunction.Structs)
        {
            if (st.Name == structTarget)
                goto Continue;
        }

        new Error($"Struct {structTarget} does not exist but was attempted to be instancieted!", context);
        
        Continue:
        foreach (Struct st in currentFunction.Structs)
        {
            try
            {
                if (st.Name == structTarget)
                {
                    var dic = new Dictionary<string, object?>();
                    foreach (var VAR in st.Variables)
                        dic.Add(VAR.Key, null);
                    
                    currentFunction.StructInstances.Add(structInstanceName, new Struct(st.Name, dic));
                }
            }
            catch (Exception e)
            {
                new Error($"The struct {st.Name} has been defined more than once or has more than one instance of the same name!", context);
            }
        }
        return null;
    }

    public override object? VisitStructAssingment(IterkoczeScriptParser.StructAssingmentContext context)
    {
        var structInstanceName = context.IDENTIFIER(0).GetText();
        var varName = context.IDENTIFIER(1).GetText();
        var value = Visit(context.expression());
        
        currentFunction.StructInstances[structInstanceName].Variables[varName] = value;
        
        return null;
    }
    
    public override object? VisitStructMemberAccessExp(IterkoczeScriptParser.StructMemberAccessExpContext context)
    {
        var structInstanceName = context.IDENTIFIER(0).GetText();
        var varName = context.IDENTIFIER(1).GetText();
        
        if (!currentFunction.StructInstances.ContainsKey(structInstanceName))
            new Error($"struct instance {structInstanceName} does not exist!", context);

        if (!currentFunction.StructInstances[structInstanceName].Variables.ContainsKey(varName))
            new Error($"The struct variable {varName} does not exist in struct {structInstanceName}!", context);
        
        return currentFunction.StructInstances[structInstanceName].Variables[varName];
    }

    public override object? VisitForeachBlock(IterkoczeScriptParser.ForeachBlockContext context)
    {
        var variable = context.IDENTIFIER().GetText();
        var target = Visit(context.expression());

        foreach (var st in currentFunction.Structs)
        {
            if (st.Name == target)
            {
                string[] Names = new string[currentFunction.StructInstances.Count];
                int index = 0;
                foreach (var name in currentFunction.StructInstances)
                {
                    Names[index] = name.Key;
                    index++;
                }
                
                for (int i = 0; i < currentFunction.StructInstances.Count; i++)
                {
                    currentFunction.StructInstances.Add(variable, new Struct(variable, currentFunction.StructInstances[Names[i]].Variables));
                    VisitBlock(context.block());
                    currentFunction.StructInstances.Remove(variable);
                }
                return null;
            }
        }

        foreach (var VARIABLE in currentFunction.Lists)
        {
            if (target is List<object?>)
            {
                foreach (var VARIABLE2 in (List<object?>)target)
                {
                    currentFunction.VARS[variable] = VARIABLE2;
                    Visit(context.block());
                }
                currentFunction.VARS.Remove(variable);
            }

            return null;
        }

        currentFunction.VARS[variable] = null;

        foreach (var v in target.ToString())
        {
            currentFunction.VARS[variable] = v;
            Visit(context.block());
        }

        currentFunction.VARS.Remove(variable);
        return null;
    }

    public override object? VisitIfBlock(IterkoczeScriptParser.IfBlockContext context)
    {
        Func<object?, bool> condition = context.IF().GetText() == "if"
                ? IsTrue
                : IsFalse
            ;

        if(condition(Visit(context.expression())))
        { 
            Visit(context.block());
        }
        else if (context.elseIfBlock() != null)
        {
            Visit(context.elseIfBlock());
        }
        
        return null;
    }

    public override object? VisitFunctionCall(IterkoczeScriptParser.FunctionCallContext context) {
        var name = context.IDENTIFIER().GetText();
        string subjectName;
        try {
            subjectName = context.Parent.GetText().Remove(context.Parent.GetText().IndexOf('.')); // I can't just access `start` like a normal person *cries*
        }
        catch (Exception ex) { subjectName = null; }
        var args = context.expression().Select(Visit).ToArray();
        foreach (Function function in FUNCTIONS) {
            if (function.Name == name) {
                function.args = args;
                currentFunction.args = args;
                currentFunction = function;
                VisitBlock(function.Code);
                currentFunction.VARS = VARS;
                return function.ReturnValue;
            }
        }

        if (subjectName != null) {
            if (name == "Add" && currentFunction.Lists.ContainsKey(subjectName)) {
                ListOperations.Add(currentFunction.Lists[subjectName], args[0]);
                return 0;
            }
            if (name == "Remove" && currentFunction.Lists.ContainsKey(subjectName)) {
                ListOperations.Remove(currentFunction.Lists[subjectName], args[0]);
                return 0;
            }
            if (name == "IndexOf" && currentFunction.Lists.ContainsKey(subjectName)) {
                return ListOperations.IndefOf(currentFunction.Lists[subjectName], args[0]);
            }
        }
        
        if (!STANDARD_FUNCTIONS.ContainsKey(name))
            new Error($"Function {name} is not defined!", context);

        if (STANDARD_FUNCTIONS[name] is not Func<object?[], object?> func)
            throw new Exception($"variable {name} is not a function.");
            
        return func(args);
    }

    public override object? VisitReturnStatement(IterkoczeScriptParser.ReturnStatementContext context)
    { 
        //TODO: It's good for now...?
        if (currentFunction.ReturnValue == null)
            currentFunction.ReturnValue = Visit(context.expression());
        return null;
    }

    public override object? VisitFunctionDefinition(IterkoczeScriptParser.FunctionDefinitionContext context)
    {
        var name = context.IDENTIFIER().GetText();
        var code = context.block();
        
        if (VARS.ContainsKey(name))
            new Error($"Function {name} was already defined!", context);
        
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
        
        foreach (var VAR in PREDEF_VARS)
        {
            if (VAR.Key == varName)
                return PREDEF_VARS[varName];
        }
        
        foreach (var st in currentFunction.Structs)
        {
            if (st.Name == varName)
            {
                return st.Name;
            }
        }
        
        foreach (var st in currentFunction.StructInstances)
        {
            if (st.Key == varName)
                return currentFunction.StructInstances[varName];
        }
        
        foreach (var list in currentFunction.Lists)
        {
            if (list.Key == varName)
                return currentFunction.Lists[varName];
        }

        if (!currentFunction.VARS.ContainsKey(varName))
        {
            new Error($"Variable {varName} is not defined!", context);
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
            "-" => IterkoczeMath.Subtract(left, right),
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

    public override object? VisitConstant(IterkoczeScriptParser.ConstantContext context) {
        if (context.INTEGER() is { } i)
            return int.Parse(i.GetText());
        if (context.FLOAT() is { } f)
            return float.Parse(f.GetText());
        if (context.STRING() is { } s)
            return s.GetText()[1..^1];
        if (context.BOOLEAN() is { } b)
            return b.GetText() == "true";
        if (context.NULL() is { })
            return null;
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

    public override object? VisitNotExp(IterkoczeScriptParser.NotExpContext context)
    {
        var left = Visit(context.expression());

        var op = context.INVERT_OPERATOR().GetText();

        return op switch
        {
            "!" => IterkoczeBoolean.Not(left),
            "not" => IterkoczeBoolean.Not(left),
            _ => throw new NotImplementedException()
        };
    }

    public override object? VisitCompareExp(IterkoczeScriptParser.CompareExpContext context)
    {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));

        var op = context.compareOp().GetText();

        return op switch
        {
            "==" => Compare.IsEqual(left, right),
            "!=" => Compare.IsNotEqual(left, right),
            ">" => Compare.GreaterThan(left, right),
            "<" => Compare.LessThan(left, right),
            //">=" => IsEqual(left, right),
            "<=" => Compare.LessOrEqual(left, right),
            _ => throw new NotImplementedException()
        };
    }

    public override object? VisitBooleanExp(IterkoczeScriptParser.BooleanExpContext context)
    {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));

        var op = context.booleanOp().GetText();

        return op switch
        {
            "and" => IterkoczeBoolean.And(left, right),
            "or" => IterkoczeBoolean.Or(left, right),
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