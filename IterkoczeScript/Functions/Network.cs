using IterkoczeScript.Errors;
using System.Net.NetworkInformation;
using IterkoczeScript.Interpreter;
using System.Net;
using Newtonsoft.Json.Linq;
using System;

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
        return null;
    }
    public static object? Download(object?[] args) {
        if (args.Length != 2)
            _ = new RuntimeError("Function \"Download\" expects at 2 arguments. URL and destination");

        string url = args[0].ToString();
        string savePath = args[1].ToString();
        if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute)) {
            IError err = new ErrorMalformattedURL();
            err.SetError();
            return err;
        }
        if (!Uri.IsWellFormedUriString(savePath, UriKind.RelativeOrAbsolute)) {
            _ = new RuntimeError($"Path: {savePath} isn't valid");
        }

        try {
            using WebClient client = new();
            client.DownloadFile(url, savePath);
        } catch (Exception e) {
            _ = new RuntimeError("Error while downloading file: " + e.Message);
        }
        return null;
    }
    public async static Task<object?> Fetch(object?[] args) {
        if (args.Length != 1)
            _ = new RuntimeError("Function \"Fetch\" expects at 1 argument. URL");

        string url = args[0].ToString();

        if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute)) {
            IError err = new ErrorMalformattedURL();
            err.SetError();
            return err;
        }

        using (HttpClient client = new HttpClient()) {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return JObject.Parse(responseBody);
        }
    }
}
