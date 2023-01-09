namespace IterkoczeScript;

public static class IterkoczeMath {
    public static object? Add(object? left, object? right) {
        if (left is int l && right is int r)
            return l + r;
        if (left is float lf && right is float rf)
            return lf + rf;
        if (left is int lInt && right is float rFloat)
            return lInt + rFloat;
        if (left is float lFloat && right is int rInt)
            return lFloat + rInt;
        
        if (left is string || right is string)
            return $"{left}{right}";

        _ = new RuntimeError($"Cannot add value of type {left.GetType()} to {right.GetType()}");
        return false;
    }
    public static object? Subtract(object? left, object? right) {
        if (left is int l && right is int r)
            return l - r;
        if (left is float lf && right is float rf)
            return lf - rf;
        if (left is int lInt && right is float rFloat)
            return lInt - rFloat;
        if (left is float lFloat && right is int rInt)
            return lFloat - rInt;
        
        _ = new RuntimeError($"Cannot subtract value of type {left.GetType()} to {right.GetType()}");
        return false;
    }
    public static object? Multiply(object? left, object? right) {
        if (left is int l && right is int r)
            return l * r;
        if (left is float lf && right is float rf)
            return lf * rf;
        if (left is int lInt && right is float rFloat)
            return lInt * rFloat;
        if (left is float lFloat && right is int rInt)
            return lFloat * rInt;
        
        _ = new RuntimeError($"Cannot multiply value of type {left.GetType()} with {right.GetType()}");
        return false;
    }
    public static object? Modulo(object? left, object? right) {
        if (left is int l && right is int r)
            return l % r;
        if (left is float lf && right is float rf)
            return lf % rf;
        if (left is int lInt && right is float rFloat)
            return lInt % rFloat;
        if (left is float lFloat && right is int rInt)
            return lFloat % rInt;

        _ = new RuntimeError($"Cannot modulo value of type {left.GetType()} with {right.GetType()}");
        return false;
    }
    public static object? Divide(object? left, object? right) {
        if (left is int l && right is int r)
            return l / r;
        if (left is float lf && right is float rf)
            return lf / rf;
        if (left is int lInt && right is float rFloat)
            return lInt / rFloat;
        if (left is float lFloat && right is int rInt)
            return lFloat / rInt;

        _ = new RuntimeError($"Cannot divide value of type {left.GetType()} with {right.GetType()}");
        return false;
    }
    public static object? PowerOf(object? left, object? right) {
        if (left is int l && right is int r)
            return Math.Pow(l, r);
        if (left is float lf && right is float rf)
            return Math.Pow(lf, rf);
        if (left is int lInt && right is float rFloat)
            return Math.Pow(lInt, rFloat);
        if (left is float lFloat && right is int rInt)
            return Math.Pow(lFloat, rInt);

        _ = new RuntimeError($"Cannot calculate the power for value of type {left.GetType()} with {right.GetType()}");
        return false;
    }
}