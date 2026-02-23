using System.Collections.Generic;

namespace Exhale.Plugins.StateMachine
{
    public class StateMachine
    {
        public readonly State Root;
        public readonly TransitionSequencer Sequencer;
        private bool started;
        
        public StateMachine(State root)
        {
            Root = root;
            Sequencer = new TransitionSequencer(this);
        }

        
        public void Start()
        {
            if (!started) return;
            
            started = true;
            Root.Enter();
        }

        public void Tick(float deltaTime)
        {
            if(!started) Start();
            TickInternal(deltaTime);
        }
        internal void TickInternal(float deltaTime) => Root.Update(deltaTime);
        
        // changes the active state from "from" to "to", where "from" is an ancestor of "to" (or the same state)
        public void ChangeState(State from, State to)
        {
            if(from == to || from == null || to == null) return;
            
            State lca = TransitionSequencer.Lca(from, to);
            
            // exit all states from "from" up to (but not including) lca
            for(State state = from; state != lca; state = state.Parent) state.Exit();
            
            // enter all states from "to" up to (but not including) lca, in reverse order
            var stack = new Stack<State>();
            for(State state = to; state != lca; state = state.Parent) stack.Push(state);
            while(stack.Count > 0) stack.Pop().Enter();
        }
    }
}