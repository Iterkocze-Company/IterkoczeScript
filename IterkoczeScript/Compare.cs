using IterkoczeScript;

public static class Compare
{
    public static bool GreaterThan(object? left, object? right)
    {
        if (left == null || right == null) // I don't like the looks of this
            return false;
        
        if (left is int l && right is int r)
            return l > r;
        if (left is float lf && right is float rf)
            return lf > rf;
        if (left is int lInt && right is float rFloat)
            return lInt > rFloat;
        if (left is float lFloat && right is int rInt)
            return lFloat > rInt;

        new Error($"Cannot compare values of types {left.GetType()} and {right.GetType()}");
        return false;
    }
    public static bool LessThan(object? left, object? right)
    {
        if (left == null || right == null) // I don't like the looks of this
            return false;
        
        if (left is int l && right is int r)
            return l < r;
        if (left is float lf && right is float rf)
            return lf < rf;
        if (left is int lInt && right is float rFloat)
            return lInt < rFloat;
        if (left is float lFloat && right is int rInt)
            return lFloat < rInt;

        new Error($"Cannot compare values of types {left.GetType()} and {right.GetType()}");
        return false;
    }
    public static bool LessOrEqual(object? left, object? right)
    {
        if (left == null || right == null) // I don't like the looks of this
            return false;
        
        if (left is int l && right is int r)
            return l <= r;
        if (left is float lf && right is float rf)
            return lf <= rf;
        if (left is int lInt && right is float rFloat)
            return lInt <= rFloat;
        if (left is float lFloat && right is int rInt)
            return lFloat <= rInt;

        new Error($"Cannot compare values of types {left.GetType()} and {right.GetType()}");
        return false;
    }
    public static bool IsEqual(object? left, object? right)
    {
        if (left == null || right == null) // I don't like the looks of this
            return false;
        
        if (left is int l && right is int r)
            return l == r;
        if (left is float lf && right is float rf)
            return lf == rf;
        if (left is int lInt && right is float rFloat)
            return lInt == rFloat;
        if (left is float lFloat && right is int rInt)
            return lFloat == rInt;
        
        if (left is string || right is string)
        {
            if (String.Compare(left.ToString(), right.ToString()) != 0)
            {
                return !true; //KEKW
            }
            return !false; //ICANT
        }

        new Error($"Cannot compare values of types {left.GetType()} and {right.GetType()}");
        return false;
    }
    public static bool IsNotEqual(object? left, object? right)
    {
        if (left is int l && right is int r)
            return l != r;
        if (left is float lf && right is float rf)
            return lf != rf;
        if (left is int lInt && right is float rFloat)
            return lInt != rFloat;
        if (left is float lFloat && right is int rInt)
            return lFloat != rInt;
        
        if (left is string || right is string)
        {
            if (String.Compare(left.ToString(), right.ToString()) != 0)
            {
                return true;
            }
            return false;
        }

        new Error($"Cannot compare values of types {left.GetType()} and {right.GetType()}");
        return false;
    }
}