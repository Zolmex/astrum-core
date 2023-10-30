using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public interface ILogger
    {
        void Info(object message, bool saveToFile = false);
        void Warn(object message, bool saveToFile = false);
        void Error(object message, bool saveToFile = false);
        void Fatal(object message, bool saveToFile = false);
        void Debug(object message, bool saveToFile = false);
    }
}
