using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GameServer.Game.Entities.Behaviors.Transitions
{
    public class TimeInfo
    {
        public int Time;
    }

    public class TimedTransition : IBehaviorTransition
    {
        private readonly string _targetState;
        private readonly int _time;

        public TimedTransition(XElement xml)
        {
            _targetState = xml.GetAttribute<string>("targetState");
            _time = xml.GetAttribute<int>("time");
        }

        public TimedTransition(string targetState, int time)
        {
            _targetState = targetState;
            _time = time;
        }

        public void Enter(Character host, BehaviorController controller)
        {
            var timeInfo = new TimeInfo()
            {
                Time = _time
            };

            controller.InsertResource(this, timeInfo);
        }

        public string Tick(Character host, RealmTime time, BehaviorController controller)
        {
            var resource = (TimeInfo)controller.GetResource(this);
            resource.Time -= time.ElapsedMsDelta;

            if (resource.Time <= 0)
                return _targetState;
            return null;
        }
    }
}
