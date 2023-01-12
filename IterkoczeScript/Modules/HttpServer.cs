using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IterkoczeScript.Modules;
internal class HttpServer {
    private readonly HttpListener _listener = new HttpListener();
    private readonly string _rootDirectory;

    public HttpServer(string[] prefixes, string rootDirectory) {
        if (!HttpListener.IsSupported)
            throw new NotSupportedException("Needs Windows XP SP2, Server 2003 or later.");

        if (prefixes == null || prefixes.Length == 0)
            throw new ArgumentException("prefixes");

        _rootDirectory = rootDirectory;

        // Add the prefixes.
        foreach (string s in prefixes) {
            _listener.Prefixes.Add(s);
        }

        _listener.Start();
    }

    public void Run() {
        ThreadPool.QueueUserWorkItem((o) => {
            Console.WriteLine("Webserver running on localhost:8080...");
            try {
                while (_listener.IsListening) {
                    ThreadPool.QueueUserWorkItem((c) => {
                        var ctx = c as HttpListenerContext;
                        try {
                            string filename = ctx.Request.Url.LocalPath;
                            string path;
                            if (filename == "/")
                                path = Path.Combine(_rootDirectory, "index.html");
                            else
                                path = Path.Combine(_rootDirectory, filename.TrimStart('/'));
                            IPAddress clientIP = ((IPEndPoint)ctx.Request.RemoteEndPoint).Address;
                            Console.WriteLine($"Client {clientIP} requested: " + path);

                            // check if the file exists
                            if (!File.Exists(path)) {
                                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                                return;
                            }

                            // get the file extension
                            string extension = Path.GetExtension(path);

                            // set the content type based on the file extension
                            switch (extension) {
                                case ".html":
                                    ctx.Response.ContentType = "text/html";
                                    break;
                                case ".css":
                                    ctx.Response.ContentType = "text/css";
                                    break;
                                case ".js":
                                    ctx.Response.ContentType = "application/javascript";
                                    break;
                                case ".jpg":
                                case ".jpeg":
                                    ctx.Response.ContentType = "image/jpeg";
                                    break;
                                case ".png":
                                    ctx.Response.ContentType = "image/png";
                                    break;
                                default:
                                    ctx.Response.ContentType = "application/octet-stream";
                                    break;
                            }

                            // read the file into a byte array
                            byte[] file = File.ReadAllBytes(path);
                            ctx.Response.ContentLength64 = file.Length;
                            ctx.Response.OutputStream.Write(file, 0, file.Length);
                            Console.WriteLine("File '" + path + "' sent to " + clientIP);
                        }
                        catch { } // suppress any exceptions
                        finally {
                            // always close the stream
                            ctx.Response.OutputStream.Close();
                        }
                    }, _listener.GetContext());
                }
            }
            catch { } // suppress any exceptions
        });
    }

    public void Stop() {
        _listener.Stop();
        _listener.Close();
    }

}
    

