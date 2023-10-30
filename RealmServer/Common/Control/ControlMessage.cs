using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Control
{
    public enum ControlChannel
    {
        MemberEnter,
        MemberLeave,
        KeepAlive
    }

    public class ControlMessage<T>
    {
        public string InstanceID { get; set; }
        public string TargetID { get; set; }
        public T Content { get; set; }
    }
}
