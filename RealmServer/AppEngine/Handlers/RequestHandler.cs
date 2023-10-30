using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;

namespace AppEngine.Handlers
{
    public abstract class RequestHandler
    {
        private static Dictionary<string, RequestHandler> _handlers = new Dictionary<string, RequestHandler>();

        public abstract string Path { get; } // Request identifier

        public abstract string Handle(string ip, NameValueCollection query);

        protected string WriteError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return "<Error/>";
            return $"<Error>{message}</Error>";
        }

        protected string WriteSuccess(string message = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                return "<Success/>";
            return $"<Success>{message}</Success>";
        }

        // Loads every RequestHandler child from the assembly into a Dictionary.
        public static void Load()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];
                if (!type.IsAbstract && type.IsSubclassOf(typeof(RequestHandler)))
                {
                    var handler = (RequestHandler)Activator.CreateInstance(type);
                    _handlers.Add(handler.Path, handler);
                }
            }
        }

        public static bool Exists(string path)
        {
            return _handlers.ContainsKey(path);
        }

        public static string Handle(string path, string ip, NameValueCollection query)
        {
            if (!_handlers.TryGetValue(path, out var handler))
                throw new ArgumentException($"Handler for {path} does not exist."); // Application shouldn't even get to this point

            return handler.Handle(ip, query);
        }
    }
}
