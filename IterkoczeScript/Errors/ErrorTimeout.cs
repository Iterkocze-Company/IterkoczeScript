namespace IterkoczeScript.Errors;

public class ErrorTimeout : IError {
    public string Message => "Connection timeout";
}
