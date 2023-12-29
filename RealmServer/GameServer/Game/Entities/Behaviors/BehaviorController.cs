using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Entities.Behaviors
{
    public class BehaviorController
    {
        private static readonly Logger _log = new Logger(typeof(BehaviorController));

        public BehaviorState RootState { get; }

        private readonly Character _host;
        private readonly Dictionary<int, object> _stateResources;
        private BehaviorState _currentState;

        public BehaviorController(Character host, BehaviorState rootState)
        {
            _host = host;
            _stateResources = new Dictionary<int, object>();

            RootState = rootState;
        }

        public void Initialize()
        {
            _currentState = RootState.GetDeepState();
            _currentState.Enter(_host, this);
        }

        public void Tick(RealmTime time)
        {
            if (_currentState == null)
                return;

            var targetState = _currentState.Tick(_host, time, this); // If this returns true that means a transition has occured
            if (targetState != null)
            {
                _currentState.Exit(_host, time, this);
                _stateResources.Clear();

                _currentState = null;

                if (RootState.States.TryGetValue(targetState, out var newState))
                {
                    _currentState = newState;
                    _currentState.Enter(_host, this);
                    _currentState.Tick(_host, time, this);
                }
                else
                    _log.Error($"{_host.Name}: State {targetState} not found.");
            }
        }

        // State resources are extra information that behaviors/transitions need to execute properly.
        // Information like time left, in what state of the behavior is the current entity.
        // Stuff like Follow behavior use this as a way to function.

        public void InsertResource(IStateChild instance, object resource)
        {
            _stateResources.Add(instance.GetHashCode(), resource); // No need to worry about removing since the dictionary is cleared when the state exits
        }

        public object GetResource(IStateChild instance)
        {
            if (!_stateResources.TryGetValue(instance.GetHashCode(), out var resource))
                throw new ArgumentException($"Behavior resource not found");
            return resource;
        }
    }
}
