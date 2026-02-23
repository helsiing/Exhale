using System.Collections.Generic;

namespace Exhale.Plugins.StateMachine
{
    public class TransitionSequencer
    {
        public readonly StateMachine Machine;

        public TransitionSequencer(StateMachine machine)
        {
            Machine = machine;
        }

        public void RequestTransition(State from, State to)
        {
            Machine.ChangeState(from, to);
        }
        
        public static State Lca(State a, State b)
        {
            // create a set of all ancestors of a
            var ap = new HashSet<State>();
            for(var state = a; state != null; state = state.Parent) ap.Add(state);
            
            // find the first ancestor of b that is also a parent of a
            for(var state = b; state != null; state = state.Parent) if (ap.Contains(state)) return state;
            
            // if no common ancestor is found (should never happen since root is common ancestor of all states), return null
            return null;
        }
    }
}