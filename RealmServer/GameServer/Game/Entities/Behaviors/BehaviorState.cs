using Common.Utilities;
using GameServer.Game.Entities.Behaviors.Actions;
using GameServer.Game.Entities.Behaviors.Transitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GameServer.Game.Entities.Behaviors
{
    public interface IStateChild
    {
        // Empty
    }

    public class BehaviorState : IStateChild
    {
        private static readonly Logger _log = new Logger(typeof(BehaviorState));

        public readonly Dictionary<string, BehaviorState> States = new Dictionary<string, BehaviorState>();

        public string Name { get; }
        public List<IBehaviorTransition> Transitions { get; set; }
        public List<IBehaviorAction> Actions { get; set; }
        public List<BehaviorState> ChildStates { get; set; }

        public BehaviorState(string name, params IStateChild[] children)
            : this(children)
        {
            Name = name;
        }

        public BehaviorState(params IStateChild[] children)
        {
            Transitions = new List<IBehaviorTransition>();
            Actions = new List<IBehaviorAction>();
            ChildStates = new List<BehaviorState>();

            foreach (var child in children)
            {
                if (child is IBehaviorTransition trans)
                    Transitions.Add(trans);
                else if (child is IBehaviorAction action)
                    Actions.Add(action);
                else if (child is BehaviorState state)
                {
                    state.AddToDictionary(States);
                    ChildStates.Add(state);
                }
            }
        }

        public static BehaviorState LoadFromXML(XElement xml)
        {
            var children = LoadXMLChildren(xml.Element("State"));
            return new BehaviorState(children);
        }

        private static IStateChild[] LoadXMLChildren(XElement parent)
        {
            var i = 0;
            var childStateCount = parent.Elements("State").Count();
            var childActionCount = parent.Elements("Behavior").Count();
            var childTransitionCount = parent.Elements("Transition").Count();
            var children = new IStateChild[childStateCount + childActionCount + childTransitionCount];
            foreach (var childStateXML in parent.Elements("State"))
            {
                children[i] = new BehaviorState(childStateXML.GetAttribute<string>("name"), LoadXMLChildren(childStateXML));
                i++;
            }
            foreach (var childActionXML in parent.Elements("Behavior"))
            {
                children[i] = LoadXMLBehavior(childActionXML);
                i++;
            }
            foreach (var childTransitionXML in parent.Elements("Transition"))
            {
                children[i] = LoadXMLTransition(childTransitionXML);
                i++;
            }
            return children;
        }

        private static IBehaviorAction LoadXMLBehavior(XElement xml)
        {
            switch (xml.Value)
            {
                case "Follow":
                    return new Follow(xml);
                case "Shoot":
                    return new Shoot(xml);
                case "Taunt":
                    return new Taunt(xml);
                case "Wander":
                    return new Wander(xml);
                default:
                    throw new Exception($"Invalid behavior action {xml.Value}");
            }
        }

        private static IBehaviorTransition LoadXMLTransition(XElement xml)
        {
            switch (xml.Value)
            {
                case "TimedTransition":
                    return new TimedTransition(xml);
                default:
                    throw new Exception($"Invalid behavior transition {xml.Value}");
            }
        }

        public void AddToDictionary(Dictionary<string, BehaviorState> dict)
        {
            dict.Add(Name, this);
            foreach (var state in ChildStates)
                state.AddToDictionary(dict);
        }

        public BehaviorState GetDeepState()
        {
            if (ChildStates.Count != 0)
                return ChildStates[0].GetDeepState();
            return this;
        }

        public void Enter(Character host, BehaviorController controller) // Perform any initial setups we need for current and child states
        {
            foreach (var trans in Transitions)
                trans.Enter(host, controller);

            foreach (var act in Actions)
                act.Enter(host, controller);

            foreach (var state in ChildStates)
                state.Enter(host, controller);
        }

        public string Tick(Character host, RealmTime time, BehaviorController controller)
        {
            foreach (var trans in Transitions) // Check if we have a transition to make
            {
                var targetState = trans.Tick(host, time, controller);
                if (targetState != null)
                    return targetState;
            }

            foreach (var act in Actions)
                act.Tick(host, time, controller);

            return null;
        }

        public void Exit(Character host, RealmTime time, BehaviorController controller)
        {
            foreach (var act in Actions)
                act.Exit(host, time, controller);
        }
    }
}
