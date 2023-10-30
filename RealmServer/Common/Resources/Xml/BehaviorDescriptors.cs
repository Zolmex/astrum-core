using Common.Utilities;
using System.Linq;
using System.Xml.Linq;

namespace Common.Resources.Xml
{
    public class StateDesc
    {
        public readonly string Name;
        public readonly StateDesc[] States;
        public readonly StateChildDesc[] Behaviors;
        public readonly StateChildDesc[] Transitions;
        public readonly StateDesc DeepestState;

        public StateDesc(XElement e)
        {
            Name = e.GetAttribute<string>("name");
            States = e.Elements("State").Select(i => new StateDesc(i)).ToArray();
            Behaviors = e.Elements("Behavior").Select(i => new StateChildDesc(i)).ToArray();
            Transitions = e.Elements("Transition").Select(i => new StateChildDesc(i)).ToArray();
            DeepestState = GetDeepestState();
        }

        public StateDesc GetDeepestState()
        {
            if (States.Any())
                return States[0].GetDeepestState();
            return this;
        }

        public StateDesc GetState(string name)
        {
            for (var i = 0; i < States.Length; i++)
            {
                var ret = States[i];
                if (ret.Name == name)
                    return ret;
            }

            for (var i = 0; i < States.Length; i++)
            {
                var ret = States[i].GetState(name);
                if (ret != null)
                    return ret;
            }

            return null;
        }
    }

    public class StateChildDesc
    {
        public readonly string Name;
        public readonly XElement Params;

        public StateChildDesc(XElement e)
        {
            Name = (string)e;
            Params = e;
        }
    }

    public class LootTableDesc
    {
        public StateChildDesc[] Loots;

        public LootTableDesc(XElement e)
            => Loots = e.Elements("Loot").Select(i => new StateChildDesc(i)).ToArray();
    }
}
