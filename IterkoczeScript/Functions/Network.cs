using IterkoczeScript.Errors;
using System.Net.NetworkInformation;
using IterkoczeScript.Interpreter;

namespace IterkoczeScript.Functions;

public static class Network {
    public static object? IsServerUp(object?[] args)
    {
        if (args.Length < 1)
            _ = new RuntimeError("Function \"IsServerUp\" expects at least 1 argument.");

        int timeout = 1000;

        if (args.Length == 2)
            timeout = (int)args[1];
        if (!Uri.IsWellFormedUriString(args[0].ToString(), UriKind.RelativeOrAbsolute)) {
            IError err = new ErrorMalformattedURL();
            err.SetError();
            return err;
        }

        var ping = new Ping();
        PingReply reply;
        try {
            reply = ping.Send(args[0].ToString(), timeout);
        }
        catch (Exception e) {
            IError err = new ErrorInvalidHost();
            err.SetError();
            return err;
        }
        if (reply.Status is IPStatus.TimedOut) {
            IError err = new ErrorTimeout();
            err.SetError();
            return err;
        }
        return 0;
    }
}
