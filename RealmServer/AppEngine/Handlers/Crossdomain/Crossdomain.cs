using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEngine.Handlers.Crossdomain
{
    public class Crossdomain : RequestHandler
    {
        private const string CROSSDOMAIN_PATH = "Handlers/Crossdomain/crossdomain.xml";

        public override string Path => "/crossdomain.xml";
        private string _file;

        public override string Handle(string ip, NameValueCollection query)
        {
            if (_file == null)
                _file = File.ReadAllText(CROSSDOMAIN_PATH);

            return _file;
        }
    }
}
