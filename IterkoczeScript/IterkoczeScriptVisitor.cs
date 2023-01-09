using Antlr4.Runtime.Misc;
using IterkoczeScript.Content;
using IterkoczeScript.Errors;
using IterkoczeScript.Functions;

namespace IterkoczeScript;

public class IterkoczeScriptVisitor : IterkoczeScriptBaseVisitor<object?> {
    private bool ShouldEndLoop = false;
    //private Dictionary<string, Variable> VARS { get; } = new();
    public static Dictionary<string, Variable> PREDEF_VARS { get; } = new();
    public static Dictionary<string, Variable> GLOBAL_VARS { get; } = new ();
    //public static Dictionary<string, Variable> CONST_VARS { get; } = new();
    private Dictionary<string, object?> STANDARD_FUNCTIONS { get; } = new();
    private List<Function> FUNCTIONS { get; } = new();
    private static Function mainFunction = new("Main", null);
    private Function currentFunction = mainFunction;

    public IterkoczeScriptVisitor()
    {
        PREDEF_VARS["PI"] = new(Math.PI,true, true);
        PREDEF_VARS["RED"] = new(ConsoleColor.Red,true,true);
        PREDEF_VARS["GREEN"] = new(ConsoleColor.Green, true, true);
        PREDEF_VARS["BLUE"] = new(ConsoleColor.Blue, true, true);
        PREDEF_VARS["ERROR"] = new("NONE", true, true);

        // Utility
        STANDARD_FUNCTIONS["Argument"] = new Func<object?[], object?>(StandardFunctions.Argument);
        STANDARD_FUNCTIONS["ArgumentCount"] = new Func<object?[], object?>(StandardFunctions.ArgumentCount);
        STANDARD_FUNCTIONS["Exit"] = new Func<object?[], object?>(StandardFunctions.Exit);
        STANDARD_FUNCTIONS["OK"] = new Func<object?[], object?>(StandardFunctions.OK);
        STANDARD_FUNCTIONS["StartRuntimeTimer"] = new Func<object?[], object?>(StandardFunctions.StartRuntimeTimer);
        STANDARD_FUNCTIONS["StopRuntimeTimer"] = new Func<object?[], object?>(StandardFunctions.StopRuntimeTimer);
        STANDARD_FUNCTIONS["GetRuntime"] = new Func<object?[], object?>(StandardFunctions.GetRuntime);
        STANDARD_FUNCTIONS["Execute"] = new Func<object?[], object?>(StandardFunctions.Execute);
        STANDARD_FUNCTIONS["Linux"] = new Func<object?[], object?>(StandardFunctions.Linux);



        // BASIC
        STANDARD_FUNCTIONS["Write"] = new Func<object?[], object?>(StandardFunctions.Write);
        STANDARD_FUNCTIONS["WriteToFile"] = new Func<object?[], object?>(StandardFunctions.WriteToFile);
        STANDARD_FUNCTIONS["ReadFromFile"] = new Func<object?[], object?>(StandardFunctions.ReadFromFile);
        STANDARD_FUNCTIONS["Read"] = new Func<object?[], object?>(StandardFunctions.Read);
        STANDARD_FUNCTIONS["ReadAsInt"] = new Func<object?[], object?>(StandardFunctions.ReadAsInt);
        STANDARD_FUNCTIONS["GetChar"] = new Func<object?[], object?>(StandardFunctions.GetChar);
        
        // CONVERTION
        STANDARD_FUNCTIONS["ConvertToInt"] = new Func<object?[], object?>(StandardFunctions.ConvertToInt);
        STANDARD_FUNCTIONS["ConvertToString"] = new Func<object?[], object?>(StandardFunctions.ConvertToString);

        // NETWORK
        STANDARD_FUNCTIONS["IsServerUp"] = new Func<object?[], object?>(Network.IsServerUp);
    }

    public override object? VisitListCreation(IterkoczeScriptParser.ListCreationContext context) {
        var listName = context.IDENTIFIER().GetText();
        currentFunction.Lists[listName] = new List<object?>();
        return null;
    }

    public override object? VisitAssingment(IterkoczeScriptParser.AssingmentContext context) {
        var varName = context.IDENTIFIER().GetText();
        bool isGlobal = context.start.Text.ToString() == "global" ? true : false;
        bool isConst = context.start.Text.ToString() == "const" ? true : false;
        var value = Visit(context.expression());

        if (GLOBAL_VARS.ContainsKey(varName))
             GLOBAL_VARS[varName].Value = value;

        if (currentFunction.VARS.ContainsKey(varName)) {
            if (currentFunction.VARS[varName].isConstant)
                _ = new RuntimeError($"Can't write to a constant {varName}", context);
             
            currentFunction.VARS[varName].Value = value;
            return null;
        }
        currentFunction.VARS.Add(varName, new(value, isGlobal, isConst));
        if (currentFunction.VARS[varName].isGlobal)
            GLOBAL_VARS.Add(varName, currentFunction.VARS[varName]);

        return null;
    }

    public override object? VisitArrayAssingment(IterkoczeScriptParser.ArrayAssingmentContext context) {
        var arrayName = context.IDENTIFIER().GetText();
        var index = context.INTEGER().GetText();
        var value = Visit(context.expression());
            
        try {
            currentFunction.Arrays[arrayName].SetValue(value, int.Parse(index));
        }
        catch {
           _ = new RuntimeError($"Index {index} was out of bounds for array {arrayName} of size {Utility.GetRealArrayLenght(currentFunction.Arrays[arrayName])}.", context);
        }
        return null;
    }

    public override object? VisitArrayCreation(IterkoczeScriptParser.ArrayCreationContext context) {
        var arrayName = context.IDENTIFIER().GetText();
        var arraySize = 0;
        if (context.expression() == null) {
           _ = new RuntimeError($"Array \"{arrayName}\" has no size at declaration.", context);
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
                _ = new RuntimeError($"Index {index} was out of bounds for array {arrayName} of size {Utility.GetRealArrayLenght(currentFunction.Arrays[arrayName])}.", context);
            }
        }
        foreach (var VARIABLE in currentFunction.Lists) { //If it's a List
            if (VARIABLE.Key == arrayName) {
                if (currentFunction.Lists[arrayName].Count <= (int)index) {
                   _ = new RuntimeError($"Index {index} was out of bounds in the List {arrayName}!");
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

        currentFunction.VARS[context.assingment().IDENTIFIER().ToString()].Value = indexer;
        
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
            if (ShouldEndLoop) goto End;
            VisitBlock(context.block());
            currentFunction.VARS[context.assingment().IDENTIFIER().ToString()].Value = Convert.ToInt32(indexer)+adder;
            indexer += adder;
        }
        goto End;
        GreaterThan:
        for (int i = indexer; i > (int)condition; i += adder) {
            if (ShouldEndLoop) goto End;
            VisitBlock(context.block());
            currentFunction.VARS[context.assingment().IDENTIFIER().ToString()].Value = Convert.ToInt32(indexer)+adder;
            indexer += adder;
        }
        goto End;
        GreaterThanOrEqual:
        for (int i = indexer; i >= (int)condition; i += adder) {
            if (ShouldEndLoop) goto End;
            VisitBlock(context.block());
            currentFunction.VARS[context.assingment().IDENTIFIER().ToString()].Value = Convert.ToInt32(indexer)+adder;
            indexer += adder;
        }
        goto End;
        LessThanOrEqual:
        for (int i = indexer; i <= (int)condition; i += adder) {
            if (ShouldEndLoop) goto End;
            VisitBlock(context.block());
            currentFunction.VARS[context.assingment().IDENTIFIER().ToString()].Value = Convert.ToInt32(indexer)+adder;
            indexer += adder;
        }
        End:
        currentFunction.VARS.Remove(context.assingment().IDENTIFIER().ToString());
        ShouldEndLoop = false;
        return null;
    }

    public override object VisitCrackLoopExp([NotNull] IterkoczeScriptParser.CrackLoopExpContext context)
    {
        ShouldEndLoop = true;
        return base.VisitCrackLoopExp(context);
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

        _ = new RuntimeError($"Struct {structTarget} does not exist but was attempted to be instancieted!", context);
        
        Continue:
        foreach (Struct st in currentFunction.Structs) {
            try {
                if (st.Name == structTarget) {
                    var dic = new Dictionary<string, object?>();
                    foreach (var VAR in st.Variables)
                        dic.Add(VAR.Key, null);
                    
                    currentFunction.StructInstances.Add(structInstanceName, new Struct(st.Name, dic));
                }
            }
            catch (Exception e) {
                new RuntimeError($"The struct {st.Name} has been defined more than once or has more than one instance of the same name!", context);
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
            new RuntimeError($"struct instance {structInstanceName} does not exist!", context);

        if (!currentFunction.StructInstances[structInstanceName].Variables.ContainsKey(varName))
            new RuntimeError($"The struct variable {varName} does not exist in struct {structInstanceName}!", context);
        
        return currentFunction.StructInstances[structInstanceName].Variables[varName];
    }

    public override object? VisitForeachBlock(IterkoczeScriptParser.ForeachBlockContext context)
    {
        var variable = context.IDENTIFIER().GetText();
        var target = Visit(context.expression());

        foreach (var st in currentFunction.Structs) {
            if (st.Name == target) {
                string[] Names = new string[currentFunction.StructInstances.Count];
                int index = 0;
                foreach (var name in currentFunction.StructInstances) {
                    Names[index] = name.Key;
                    index++;
                }
                
                // Do foreach on a struct
                for (int i = 0; i < currentFunction.StructInstances.Count; i++) {
                    currentFunction.StructInstances.Add(variable, new Struct(variable, currentFunction.StructInstances[Names[i]].Variables));
                    VisitBlock(context.block());
                    if (ShouldEndLoop) break;
                    currentFunction.StructInstances.Remove(variable);
                }
                ShouldEndLoop = false;
                currentFunction.StructInstances.Remove(variable);
                return null;
            }
        }

        foreach (var VARIABLE in currentFunction.Lists)
        {
            if (target is List<object?>)
            {
                foreach (var VARIABLE2 in (List<object?>)target)
                {
                    currentFunction.VARS[variable].Value = VARIABLE2;
                    Visit(context.block());
                }
                currentFunction.VARS.Remove(variable);
            }

            return null;
        }

        currentFunction.VARS[variable] = null;

        foreach (var v in target.ToString())
        {
            currentFunction.VARS[variable].Value = v;
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

        if(condition(Visit(context.expression()))) { 
            Visit(context.block());
        }
        else if (context.elseIfBlock() != null) {
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
        Dictionary<string, Variable> OLDVARS;
        foreach (Function function in FUNCTIONS) {
            if (function.Name == name) {
                OLDVARS = currentFunction.VARS;
                function.args = args;
                currentFunction.args = args;
                currentFunction = function;
                VisitBlock(function.Code);
                //currentFunction.VARS = VARS;
                currentFunction.VARS = OLDVARS;

                return function.ReturnValue;
            }
        }

        if (subjectName != null) {
            if (currentFunction.Lists.ContainsKey(subjectName)) // If it's a List
                return ListOperations.Invoke(currentFunction.Lists[subjectName], args[0], context.start.Text);
        }
        
        if (!STANDARD_FUNCTIONS.ContainsKey(name))
            new RuntimeError($"Function {name} is not defined!", context);

        if (STANDARD_FUNCTIONS[name] is not Func<object?[], object?> func)
            throw new Exception($"variable {name} is not a function.");
            
        return func(args);
    }

    public override object? VisitReturnStatement(IterkoczeScriptParser.ReturnStatementContext context) { 
        //TODO: It's good for now...?
        if (currentFunction.ReturnValue == null)
            currentFunction.ReturnValue = Visit(context.expression());
        return null;
    }

    public override object? VisitFunctionDefinition(IterkoczeScriptParser.FunctionDefinitionContext context) {
        var name = context.IDENTIFIER().GetText();
        var code = context.block();

        if (STANDARD_FUNCTIONS.ContainsKey(name))
            _ = new RuntimeError($"Function {name} is a standard function, but an attempt was made to override it", context);
        
        if (currentFunction.VARS.ContainsKey(name))
            _ = new RuntimeError($"Function {name} was already defined!", context);
        
        FUNCTIONS.Add(new Function(name, code));

        return null;
    }

    public override object? VisitArgumentIdentifierExp(IterkoczeScriptParser.ArgumentIdentifierExpContext context) {
        return currentFunction.args[int.Parse(context.INTEGER().GetText())];
    }

    public override object VisitArgumentAssingmentExp([NotNull] IterkoczeScriptParser.ArgumentAssingmentExpContext context) {
        var val = Visit(context.expression());
        currentFunction.args[int.Parse(context.INTEGER().GetText())] = val;
        return null;
    }

    public override object? VisitIdentifierExp(IterkoczeScriptParser.IdentifierExpContext context) {
        var varName = context.IDENTIFIER().GetText();
        
        foreach (var VAR in PREDEF_VARS) {
            if (VAR.Key == varName)
                return PREDEF_VARS[varName];
        }
        
        foreach (var st in currentFunction.Structs) {
            if (st.Name == varName) {
                return st.Name;
            }
        }
        
        foreach (var st in currentFunction.StructInstances) {
            if (st.Key == varName)
                return currentFunction.StructInstances[varName];
        }
        
        foreach (var list in currentFunction.Lists) {
            if (list.Key == varName)
                return currentFunction.Lists[varName];
        }

        if (!currentFunction.VARS.ContainsKey(varName)
            && !GLOBAL_VARS.ContainsKey(varName)) {
            _ = new RuntimeError($"Variable {varName} is not defined!", context);
        }

        if (GLOBAL_VARS.ContainsKey(varName))
            return GLOBAL_VARS[varName].Value;

        if (currentFunction.VARS.ContainsKey(varName))
            return currentFunction.VARS[varName].Value;

        return null;
    }

    public override object? VisitMathExp(IterkoczeScriptParser.MathExpContext context) {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));

        var op = context.mathOp().GetText();

        return op switch {
            "+" => IterkoczeMath.Add(left, right),
            "-" => IterkoczeMath.Subtract(left, right),
            "*" => IterkoczeMath.Multiply(left, right),
            "%" => IterkoczeMath.Modulo(left, right),
            "/" => IterkoczeMath.Divide(left, right),
            "^" => IterkoczeMath.PowerOf(left, right),
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

    public override object VisitCatapult([NotNull] IterkoczeScriptParser.CatapultContext context) {
        var exName = context.IDENTIFIER();

        var oldColour = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("[EXCEPTION CATAPULTED] " + exName + " from line " + context.start.Line);
        Console.ForegroundColor = oldColour;

        Environment.Exit(-1);
        return -1;
    }

    private bool IsTrue(object? value) {
        if (value is IError)
            _ = new RuntimeError($"Tried to compare an error value. Possibly an unhandled error in an if block? {value}");
        if (value is bool b)
            return b;

        _ = new RuntimeError($"{value} is not boolean.");
        return false;
    }

    private bool IsFalse(object? value) => !IsTrue(value);
}