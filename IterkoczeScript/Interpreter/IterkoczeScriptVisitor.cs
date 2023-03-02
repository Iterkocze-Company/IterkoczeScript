using Antlr4.Runtime.Misc;
using IterkoczeScript.Content;
using IterkoczeScript.Errors;
using IterkoczeScript.Functions;
using System.Globalization;
using System.Linq;
using static IterkoczeScript.Interpreter.Variable;

namespace IterkoczeScript.Interpreter;

public class IterkoczeScriptVisitor : IterkoczeScriptBaseVisitor<object?> {

    private bool ShouldEndLoop = false;
    public static bool IsSilent = false; // Determines if the Interpreter will show warnings

    public static Dictionary<string, Variable> PREDEF_VARS { get; } = new();
    public static Dictionary<string, Variable> GLOBAL_VARS { get; } = new();
    private Dictionary<string, object?> STANDARD_FUNCTIONS { get; } = new();
    private List<Function> FUNCTIONS { get; } = new();
    public static Function currentFunction = new("Main", null);

    public IterkoczeScriptVisitor() {
        PREDEF_VARS["PI"] = new(Math.PI, true, true);
        PREDEF_VARS["RED"] = new(ConsoleColor.Red, true, true);
        PREDEF_VARS["GREEN"] = new(ConsoleColor.Green, true, true);
        PREDEF_VARS["BLUE"] = new(ConsoleColor.Blue, true, true);
        PREDEF_VARS["ERROR"] = new("NONE", true, true);

        // Utility
        STANDARD_FUNCTIONS["Argument"] = new Func<object?[], object?>(Functions.Utility.Argument);
        STANDARD_FUNCTIONS["ArgumentCount"] = new Func<object?[], object?>(Functions.Utility.ArgumentCount);
        STANDARD_FUNCTIONS["Exit"] = new Func<object?[], object?>(Functions.Utility.Exit);
        STANDARD_FUNCTIONS["OK"] = new Func<object?[], object?>(Functions.Utility.OK);
        STANDARD_FUNCTIONS["StartRuntimeTimer"] = new Func<object?[], object?>(Functions.Utility.StartRuntimeTimer);
        STANDARD_FUNCTIONS["StopRuntimeTimer"] = new Func<object?[], object?>(Functions.Utility.StopRuntimeTimer);
        STANDARD_FUNCTIONS["GetRuntime"] = new Func<object?[], object?>(Functions.Utility.GetRuntime);
        STANDARD_FUNCTIONS["ClearRuntimeTimer"] = new Func<object?[], object?>(Functions.Utility.ClearRuntimeTimer);
        STANDARD_FUNCTIONS["Execute"] = new Func<object?[], object?>(Functions.Utility.Execute);
        STANDARD_FUNCTIONS["Linux"] = new Func<object?[], object?>(Functions.Utility.Linux);

        // IO
        STANDARD_FUNCTIONS["WriteFile"] = new Func<object?[], object?>(IO.FileWrite);
        STANDARD_FUNCTIONS["ReadFile"] = new Func<object?[], object?>(IO.FileRead);
        STANDARD_FUNCTIONS["FileExists"] = new Func<object?[], object?>(IO.FileExists);
        STANDARD_FUNCTIONS["DirectoryCreate"] = new Func<object?[], object?>(IO.DirectoryCreate);
        STANDARD_FUNCTIONS["AllFilesIn"] = new Func<object?[], object?>(IO.AllFilesIn);
        STANDARD_FUNCTIONS["FileMove"] = new Func<object?[], object?>(IO.FileMove);
        STANDARD_FUNCTIONS["FileGetName"] = new Func<object?[], object?>(IO.FileGetName);
        STANDARD_FUNCTIONS["FileGetExtention"] = new Func<object?[], object?>(IO.FileGetExtention);
        STANDARD_FUNCTIONS["GetConfigDirectory"] = new Func<object?[], object?>(IO.GetConfigDirectory);

        // BASIC
        STANDARD_FUNCTIONS["Write"] = new Func<object?[], object?>(Basic.Write);
        STANDARD_FUNCTIONS["Read"] = new Func<object?[], object?>(Basic.Read);
        STANDARD_FUNCTIONS["ReadAsInt"] = new Func<object?[], object?>(Basic.ReadAsInt);
        STANDARD_FUNCTIONS["Negative"] = new Func<object?[], object?>(Basic.Negative);

        // STRINGS
        STANDARD_FUNCTIONS["GetChar"] = new Func<object?[], object?>(Strings.GetChar);
        STANDARD_FUNCTIONS["ToLower"] = new Func<object?[], object?>(Strings.ToLower);
        STANDARD_FUNCTIONS["ToUpper"] = new Func<object?[], object?>(Strings.ToUpper);
        STANDARD_FUNCTIONS["StringRemove"] = new Func<object?[], object?>(Strings.StringRemove);
        STANDARD_FUNCTIONS["StringRemoveFirst"] = new Func<object?[], object?>(Strings.StringRemoveFirst);
        STANDARD_FUNCTIONS["StringEndsWith"] = new Func<object?[], object?>(Strings.StringEndsWith);


        // CONVERTION
        STANDARD_FUNCTIONS["ConvertToInt"] = new Func<object?[], object?>(Conversion.ConvertToInt);
        STANDARD_FUNCTIONS["ConvertToDouble"] = new Func<object?[], object?>(Conversion.ConvertToDouble);
        STANDARD_FUNCTIONS["ConvertToString"] = new Func<object?[], object?>(Conversion.ConvertToString);

        // NETWORK
        STANDARD_FUNCTIONS["IsServerUp"] = new Func<object?[], object?>(Network.IsServerUp);
        STANDARD_FUNCTIONS["Download"] = new Func<object?[], object?>(Network.Download);

        // "SECURITY" LULW
        STANDARD_FUNCTIONS["SHA1"] = new Func<object?[], object?>(Security.SHA1);
        STANDARD_FUNCTIONS["IterkoczeUUID"] = new Func<object?[], object?>(Security.IterkoczeUUID);

        // JSON
        STANDARD_FUNCTIONS["Json"] = new Func<object?[], object?>(Json.Json1);
        STANDARD_FUNCTIONS["JsonRead"] = new Func<object?[], object?>(Json.JsonRead);
        STANDARD_FUNCTIONS["JsonWrite"] = new Func<object?[], object?>(Json.JsonWrite);
        STANDARD_FUNCTIONS["JsonArrayToArray"] = new Func<object?[], object?>(Json.JsonArrayToArray);

        // DEBUG
        STANDARD_FUNCTIONS["DebugDump"] = new Func<object?[], object?>(Debug.DebugDump);
    }

    public override object? VisitListCreation(IterkoczeScriptParser.ListCreationContext context) {
        var listName = context.IDENTIFIER().GetText();
        currentFunction.Lists[listName] = new List<object?>();
        return null;
    }

    public override object VisitVariableDefinition([NotNull] IterkoczeScriptParser.VariableDefinitionContext context) {
        var varName = context.IDENTIFIER().GetText();
        bool isGlobal = context.GLOBAL()?.GetText() == "global" ? true : false;
        bool isConst = context.CONST()?.GetText() == "const" ? true : false;
        var value = Visit(context.expression());

        if (GLOBAL_VARS.ContainsKey(varName))
            GLOBAL_VARS[varName].Value = value;

        if (!currentFunction.VARS.ContainsKey(varName))
            currentFunction.VARS.Add(varName, new(value, isGlobal, isConst, context.start.Line));
        else
            _ = new RuntimeError($"Variable {varName} already exists", context);

        if (currentFunction.VARS[varName].isGlobal)
            GLOBAL_VARS.Add(varName, currentFunction.VARS[varName]);

        return "VARIABLE DEFINTION";
    }

    public override object? VisitAssingment(IterkoczeScriptParser.AssingmentContext context) {
        var varName = context.IDENTIFIER().GetText();
        var value = Visit(context.expression());

        if (currentFunction.VARS.ContainsKey(varName)) {
            if (currentFunction.VARS[varName].isConstant)
                _ = new RuntimeError($"Can't write to a constant {varName}", context);

            currentFunction.VARS[varName].Value = value;
            return currentFunction.VARS[varName].Value;
        }
        if (!currentFunction.VARS.ContainsKey(varName))
            currentFunction.VARS.Add(varName, new(value, false, false, context.start.Line));

        currentFunction.VARS[varName].Value = value;

        if (!currentFunction.VARS.ContainsKey(varName))
            _ = new RuntimeError($"Variable {varName} wasn't defined but tried to assign a value to.", context);

        return currentFunction.VARS[varName].Value;
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
        try { arraySize = (int)Visit(context.expression()); }
        catch { _ = new RuntimeError($"{arraySize} is invalid array size", context); }

        currentFunction.Arrays[arrayName] = new object?[arraySize];
        return null;
    }

    public override object? VisitArrayAccessExp(IterkoczeScriptParser.ArrayAccessExpContext context) {
        var arrayName = context.IDENTIFIER().GetText();
        var index = Visit(context.expression());

        foreach (var VARIABLE in currentFunction.Arrays) { //If it's an array
            if (VARIABLE.Key == arrayName) {
                try {
                    return currentFunction.Arrays[arrayName].GetValue((int)index);
                }
                catch {
                    _ = new RuntimeError($"Index {index} was out of bounds for array {arrayName} of size {Utility.GetRealArrayLenght(currentFunction.Arrays[arrayName])}.", context);
                }
            }
        }
        //If it's a List
        foreach (var VARIABLE in currentFunction.Lists) {
            if (VARIABLE.Key == arrayName) {
                if (currentFunction.Lists[arrayName].Count <= (int)index) {
                    _ = new RuntimeError($"Index {index} was out of bounds in the List {arrayName}!");
                }
                return currentFunction.Lists[arrayName][(int)index];
            }
        }
        //If it's a Dictionary
        foreach (var VARIABLE in currentFunction.Dictionaries) {
            if (VARIABLE.Key == arrayName) {
                if (currentFunction.Dictionaries[arrayName].TryGetValue(index.ToString(), out object? val)) {
                    return val;
                }
                _ = new RuntimeError($"Element {index} wasn't found in Dictionary {arrayName}", context);
            }
        }
        return null;
    }

    public override object? VisitArrayStructMemberAccess(IterkoczeScriptParser.ArrayStructMemberAccessContext context) {
        var arrayName = context.IDENTIFIER(0).GetText();
        var index = Visit(context.expression());
        var structMemberName = context.IDENTIFIER(1).GetText();

        if (currentFunction.Arrays[arrayName].GetValue((int)index) is Struct) {
            Struct structInstance = (Struct)currentFunction.Arrays[arrayName].GetValue((int)index);
            return structInstance.Variables[structMemberName];
        }
        return currentFunction.Arrays[arrayName].GetValue((int)index);
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
            currentFunction.VARS[context.assingment().IDENTIFIER().ToString()].Value = Convert.ToInt32(indexer) + adder;
            indexer += adder;
        }
        goto End;
    GreaterThan:
        for (int i = indexer; i > (int)condition; i += adder) {
            if (ShouldEndLoop) goto End;
            VisitBlock(context.block());
            currentFunction.VARS[context.assingment().IDENTIFIER().ToString()].Value = Convert.ToInt32(indexer) + adder;
            indexer += adder;
        }
        goto End;
    GreaterThanOrEqual:
        for (int i = indexer; i >= (int)condition; i += adder) {
            if (ShouldEndLoop) goto End;
            VisitBlock(context.block());
            currentFunction.VARS[context.assingment().IDENTIFIER().ToString()].Value = Convert.ToInt32(indexer) + adder;
            indexer += adder;
        }
        goto End;
    LessThanOrEqual:
        for (int i = indexer; i <= (int)condition; i += adder) {
            if (ShouldEndLoop) goto End;
            VisitBlock(context.block());
            currentFunction.VARS[context.assingment().IDENTIFIER().ToString()].Value = Convert.ToInt32(indexer) + adder;
            indexer += adder;
        }
    End:
        currentFunction.VARS.Remove(context.assingment().IDENTIFIER().ToString());
        ShouldEndLoop = false;
        return null;
    }

    public override object VisitCrackLoopExp([NotNull] IterkoczeScriptParser.CrackLoopExpContext context) {
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

    public override object? VisitStructDefinition(IterkoczeScriptParser.StructDefinitionContext context) {
        var vars = VisitBlock(context.block());
        var structName = context.IDENTIFIER().GetText();

        foreach (var s in currentFunction.Structs) {
            if (s.Name == structName)
                _ = new RuntimeError($"Struct with name {structName} already exists, but tried to redefine", context);
        }

        currentFunction.Structs.Add(new Struct(structName, (Dictionary<string, object?>)vars));


        return null;
    }

    public override object? VisitStructCreation(IterkoczeScriptParser.StructCreationContext context) {
        var structInstanceName = context.IDENTIFIER(1).GetText();
        var structTarget = context.IDENTIFIER(0).GetText();

        foreach (var st in currentFunction.Structs) {
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

    public override object? VisitStructAssingment(IterkoczeScriptParser.StructAssingmentContext context) {
        var structInstanceName = context.IDENTIFIER(0).GetText();
        var varName = context.IDENTIFIER(1).GetText();
        var value = Visit(context.expression());

        if (!currentFunction.StructInstances.ContainsKey(structInstanceName))
            _ = new RuntimeError($"Struct {structInstanceName} doesn't exist in context {currentFunction.Name}");

        if (!currentFunction.StructInstances[structInstanceName].Variables.ContainsKey(varName))
            _ = new RuntimeError($"Struct {structInstanceName} doesn't contain a member named {varName}");

        currentFunction.StructInstances[structInstanceName].Variables[varName] = value;

        return null;
    }

    public override object? VisitStructMemberAccessExp(IterkoczeScriptParser.StructMemberAccessExpContext context) {
        var structInstanceName = context.IDENTIFIER(0).GetText();
        var varName = context.IDENTIFIER(1).GetText();

        if (!currentFunction.StructInstances.ContainsKey(structInstanceName))
            new RuntimeError($"struct instance {structInstanceName} does not exist!", context);

        if (!currentFunction.StructInstances[structInstanceName].Variables.ContainsKey(varName))
            new RuntimeError($"The struct variable {varName} does not exist in struct {structInstanceName}!", context);

        return currentFunction.StructInstances[structInstanceName].Variables[varName];
    }

    public override object? VisitForeachBlock(IterkoczeScriptParser.ForeachBlockContext context) {
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

        foreach (var VARIABLE in currentFunction.Lists) {
            if (target is List<object?>) {
                foreach (var VARIABLE2 in (List<object?>)target) {
                    currentFunction.VARS.Add(variable, new(VARIABLE2, false, false));
                    Visit(context.block());
                    currentFunction.VARS.Remove(variable);
                }
                currentFunction.VARS.Remove(variable);
            }

            return null;
        }

        //currentFunction.VARS[variable] = null;

        if (target is Array) {
            foreach (var v in (Array)target) {
                //currentFunction.VARS[variable].Value = v;
                currentFunction.VARS[variable] = new(v);
                Visit(context.block());
            }
            goto finish;
        }

        if (target is string) {
            foreach (var v in target.ToString()) {
                currentFunction.VARS[variable] = new(v);
                Visit(context.block());
            }
            goto finish;
        }

    finish:
        currentFunction.VARS.Remove(variable);
        return null;
    }

    public override object? VisitIfBlock(IterkoczeScriptParser.IfBlockContext context) {
        Func<object?, bool> condition = context.IF().GetText() == "if"
                ? IsTrue
                : IsFalse
            ;

        Random rand = new();
        if (currentFunction.FeatureEnabled.Contains(Function.Features.MichauScript) && rand.NextDouble() <= 0.5) {
            return null;
        }

        if (condition(Visit(context.expression()))) {
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
                function.Args = args;
                currentFunction.Args = args;
                var oldFeatures = currentFunction.FeatureEnabled;
                currentFunction = function;
                VisitBlock(function.Code);
                currentFunction.VARS = OLDVARS;
                currentFunction.FeatureEnabled = oldFeatures;

                return function.ReturnValue;
            }
        }

        if (subjectName != null) {
            // If it's a List
            if (currentFunction.Lists.ContainsKey(subjectName))
                return ListOperations.Invoke(currentFunction.Lists[subjectName], args, context.start.Text, subjectName, context);

            if (currentFunction.Dictionaries.ContainsKey(subjectName))
                return DictionaryOperation.Invoke(currentFunction.Dictionaries[subjectName], args, context.start.Text, subjectName, context);

        }

        if (!STANDARD_FUNCTIONS.ContainsKey(name))
            _ = new RuntimeError($"Function {name} is not defined!", context);

        if (STANDARD_FUNCTIONS[name] is not Func<object?[], object?> func)
            throw new Exception($"variable {name} is not a function.");

        return func(args);
    }

    public override object? VisitReturnStatement(IterkoczeScriptParser.ReturnStatementContext context) {
        //TODO: It's good for now...?
        if (currentFunction.ReturnValue == null) {
            currentFunction.ReturnValue = Visit(context.expression());
        }
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
        return currentFunction.Args[int.Parse(context.INTEGER().GetText())];
    }

    public override object VisitArgumentAssingmentExp([NotNull] IterkoczeScriptParser.ArgumentAssingmentExpContext context) {
        var val = Visit(context.expression());
        currentFunction.Args[int.Parse(context.INTEGER().GetText())] = val;
        return null;
    }

    public override object? VisitIdentifierExp(IterkoczeScriptParser.IdentifierExpContext context) {
        var varName = context.IDENTIFIER().GetText();

        foreach (var VAR in PREDEF_VARS) {
            if (VAR.Key == varName)
                return PREDEF_VARS[varName].Value;
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
            && !GLOBAL_VARS.ContainsKey(varName)
            && !currentFunction.Dictionaries.ContainsKey(varName)) {
            _ = new RuntimeError($"Variable {varName} is not defined!", context);
        }

        if (GLOBAL_VARS.ContainsKey(varName))
            return GLOBAL_VARS[varName].Value;

        if (currentFunction.VARS.ContainsKey(varName))
            return currentFunction.VARS[varName].Value;

        if (currentFunction.Dictionaries[varName] is Dictionary<string, object?>) {
            return currentFunction.Dictionaries[varName];
        }

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

    public override object? VisitWhileBlock(IterkoczeScriptParser.WhileBlockContext context) {
        Func<object?, bool> condition = context.WHILE().GetText() == "while"
                ? IsTrue
                : IsFalse
            ;

        if (condition(Visit(context.expression()))) {
            do {
                Visit(context.block());
            } while (condition(Visit(context.expression())));
        }
        else {
            Visit(context.elseIfBlock());
        }

        return null;
    }

    public override object? VisitNotExp(IterkoczeScriptParser.NotExpContext context) {
        var left = Visit(context.expression());

        var op = context.INVERT_OPERATOR().GetText();

        return op switch {
            "!" => IterkoczeBoolean.Not(left),
            "not" => IterkoczeBoolean.Not(left),
            _ => throw new NotImplementedException()
        };
    }

    public override object? VisitCompareExp(IterkoczeScriptParser.CompareExpContext context) {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));

        var op = context.compareOp().GetText();

        return op switch {
            "==" => Compare.IsEqual(left, right),
            "!=" => Compare.IsNotEqual(left, right),
            ">" => Compare.GreaterThan(left, right),
            "<" => Compare.LessThan(left, right),
            //">=" => IsEqual(left, right),
            "<=" => Compare.LessOrEqual(left, right),
            _ => throw new NotImplementedException()
        };
    }

    public override object? VisitBooleanExp(IterkoczeScriptParser.BooleanExpContext context) {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));

        var op = context.booleanOp().GetText();

        return op switch {
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

    public override object VisitDictionaryCreation([NotNull] IterkoczeScriptParser.DictionaryCreationContext context) {
        var dictionaryName = context.IDENTIFIER().GetText();
        var Dic = new Dictionary<string, object>();

        currentFunction.Dictionaries.Add(dictionaryName, Dic);
        return 0;
    }

    public override object VisitDictionaryAssingment([NotNull] IterkoczeScriptParser.DictionaryAssingmentContext context) {
        var dicName = context.IDENTIFIER().GetText();
        var fieldName = context.STRING().GetText();
        var val = Visit(context.expression());

        if (!currentFunction.Dictionaries.ContainsKey(dicName))
            _ = new RuntimeError($"Dictionary {dicName} wasn't defined");

        currentFunction.Dictionaries[dicName][fieldName.Substring(1, fieldName.Length-2)] = val;

        return 0;
    }

    public override object VisitIncrementVar([NotNull] IterkoczeScriptParser.IncrementVarContext context) {
        var varName = context.IDENTIFIER().GetText();

        if (!currentFunction.VARS.ContainsKey(varName)) {
            _ = new RuntimeError($"Variable {varName} wasn't defined thus it's value can't be raised", context);
        }

        if (currentFunction.VARS[varName].isConstant) {
            _ = new RuntimeError($"Can't write to a constant {varName}", context);
        }

        int? res = null;

        try {
            res = (int)currentFunction.VARS[varName].Value;
        }
        catch (Exception ex) {
            _ = new RuntimeError($"Can't raise a value of type {currentFunction.VARS[varName].Value.GetType()}", context);
        }
        
        res++;
        currentFunction.VARS[varName].Value = res;

        return 0;
    }

    public override object VisitDecrementVar([NotNull] IterkoczeScriptParser.DecrementVarContext context) {
        var varName = context.IDENTIFIER().GetText();

        if (!currentFunction.VARS.ContainsKey(varName)) {
            _ = new RuntimeError($"Variable {varName} wasn't defined thus it's value can't be dropped", context);
        }

        if (currentFunction.VARS[varName].isConstant) {
            _ = new RuntimeError($"Can't write to a constant {varName}", context);
        }

        int? res = null;

        try {
            res = (int)currentFunction.VARS[varName].Value;
        }
        catch (Exception ex) {
            _ = new RuntimeError($"Can't drop a value of type {currentFunction.VARS[varName].Value.GetType()}", context);
        }
        res--;
        currentFunction.VARS[varName].Value = res;

        return 0;
    }

    public override object VisitEnableLanguageFeature([NotNull] IterkoczeScriptParser.EnableLanguageFeatureContext context) {
        var feature = context.IDENTIFIER().GetText();
        switch (feature) {
            case "MichauScript":
                currentFunction.FeatureEnabled.Add(Function.Features.MichauScript);
                break;
            case "Silent":
                IsSilent = true;
                break;
            default:
                _ = new RuntimeWarning($"Feature {feature} wasn't found", context);
                break;
        }
        
        return null;
    }

    public override object VisitForgetVariable([NotNull] IterkoczeScriptParser.ForgetVariableContext context) {
        var variableName = context.IDENTIFIER().GetText();
        
        if (currentFunction.VARS.ContainsKey(variableName)) {
            currentFunction.VARS.Remove(variableName);
        }

        _ = new RuntimeError($"Can't forget variable {variableName}. It doesn't exist in the currect scope {currentFunction.Name}", context);

        return null;
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