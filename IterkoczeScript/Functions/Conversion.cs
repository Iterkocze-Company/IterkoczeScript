using IterkoczeScript.Errors;
using IterkoczeScript.Interpreter;

namespace IterkoczeScript.Functions; 

public static class Conversion {
    public static object? ConvertToInt(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError("Function \"ConvertToInt\" expects 1 argument.");

        if (!int.TryParse(args[0].ToString(), out int output))  {
            IError err = new ErrorConversionFailed();
            err.SetError();
            return err;
        }

        return output;
    }
    public static object? ConvertToDouble(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError("Function \"ConvertToDouble\" expects 1 argument.");

        if (double.TryParse(args[0].ToString(), out double res)) {
            return res;
        } else {
            IError err = new ErrorConversionFailed();
            err.SetError();
            return err;
        }
    }
    public static object? ConvertToString(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError("Function \"ConvertToString\" expects 1 argument.");

        return args[0].ToString();
    }
}
