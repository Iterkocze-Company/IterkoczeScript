namespace IterkoczeScript.Interpreter;

public static class IterkoczeBoolean {
    public static bool And(object? left, object? right) {
        if (left is int l && right is int r)
            return l == r;
        if (left is float lf && right is float rf)
            return lf == rf;
        if (left is int lInt && right is float rFloat)
            return lInt == rFloat;
        if (left is float lFloat && right is int rInt)
            return lFloat == rInt;
        if (left is bool lBool && right is bool rBool)
            return lBool && rBool;

        _ = new RuntimeError($"Cannot compare values of types {left.GetType()} and {right.GetType()}");
        return false;
    }
    public static bool Or(object? left, object? right)  {
        if (left is bool lBool && right is bool rBool)
            return lBool || rBool;

        _ = new RuntimeError($"Cannot compare values of types {left.GetType()} and {right.GetType()}");
        return false;
    }
    public static bool Not(object? exp) {
        if (exp is bool lBool)
            return !lBool;

        _ = new RuntimeError($"You can't invert non-booleans with `not`, but tried to invert type {exp.GetType()}");
        return false;
    }
}