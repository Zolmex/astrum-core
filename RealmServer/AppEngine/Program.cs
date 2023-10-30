using AppEngine.Handlers;
using Common.Control;
using Common.Database;
using Common.Resources.Config;
using Common.Resources.Xml;
using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;

namespace AppEngine
{
    class Program
    {
        private static readonly Logger Log = new Logger(typeof(Program));

        static void Main(string[] args)
        {
            Console.Title = $"Realm Server v{Assembly.GetExecutingAssembly().GetName().Version} - AppEngine";
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            // AppEngine - as opposed to GameServer - doesn't need to be a high priority process
            // since its operations are not time-critical

            // The program should always be closed by pressing Ctrl + C
            // to ensure the saving of any data that wasn't saved to the database

            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            Console.CancelKeyPress += Close;

            var listener = new HttpListener();
            var config = AppEngineConfig.Config;
            using (var timer = new EasyTimer(LogLevel.Info, "Starting server...", $"Listening on {config.Address + ":" + config.Port} ({EasyTimer.Time})"))
            {
                EnumUtils.Load(); // Initialize and prepare our static classes for later use
                RequestHandler.Load();
                XmlLibrary.Load(config.XmlsDir);

                DbClient.Connect(DatabaseConfig.Config); // Connect redis client used for communication with the database

                ServerControl.Connect(MemberType.AppEngine, "AppEngine",
                    new ServerInfo()
                    {
                        Address = config.Address
                    });

                listener.Prefixes.Add($"http://{config.Address}:{config.Port}/"); // Start receiving HTTP requests
                listener.Start();
            }

            while (true)
            {
                HttpListenerContext context = listener.GetContext(); // Blocks thread until a request is received
                string request = context.Request.Url.LocalPath;
                string ip = GetContextIP(context);

                if (!RequestHandler.Exists(request)) // Request handlers are identified by a request path. Like "/account/register"
                {
                    Log.Warn($"Unknown request '{request}' from '{ip}'");
                    context.Response.Close();
                    continue;
                }
                Log.Debug($"Received '{request}' from '{ip}'");

                NameValueCollection query;
                using (Stream inputStream = context.Request.InputStream)
                using (StreamReader reader = new StreamReader(inputStream, Encoding.UTF8))
                    query = HttpUtility.ParseQueryString(reader.ReadToEnd());

                string response = RequestHandler.Handle(request, ip, query); // Request handlers will always return data in the form of an XML string
                response ??= "<Error>Internal error.</Error>";

                try
                {
                    byte[] data = Encoding.UTF8.GetBytes(response);
                    context.Response.ContentType = "text/*";
                    context.Response.OutputStream.Write(data, 0, data.Length);
                    context.Response.Close();
                }
                catch (HttpListenerException) { }
            }
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Log.Fatal(args.ExceptionObject);

            ServerControl.Publish(ControlChannel.MemberLeave, ServerControl.Host.InstanceID, null, ServerControl.Host);
        }

        private static void Close(object sender, ConsoleCancelEventArgs args)
        {
            ServerControl.Publish(ControlChannel.MemberLeave, ServerControl.Host.InstanceID, null, ServerControl.Host);
        }

        private static string GetContextIP(HttpListenerContext context)
        {
            return context.Request.RemoteEndPoint.ToString().Split(':')[0];
        }
    }
}
