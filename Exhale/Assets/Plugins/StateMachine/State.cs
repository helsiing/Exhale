using System.Collections.Generic;

namespace Exhale.Plugins.StateMachine
{
    public class State
    {
        public readonly StateMachine Machine;
        public readonly State Parent;
        public State ActiveChild;
        
        public State(StateMachine machine, State parent)
        {
            Machine = machine;
            Parent = parent;
        }
        
        // initial child to enter when this state starts (null = this is the leaf)
        protected virtual State GetInitialState() => null;
        // target state to switch to this framme (null = stay in current state)
        protected virtual State GetTransition() => null;
        
        // lifecycle methods
        protected virtual void OnEnter() {}
        protected virtual void OnExit() {}
        protected virtual void OnUpdate(float deltaTime) {}
        
        internal void Enter()
        {
            if (Parent != null) Parent.ActiveChild = this;
            OnEnter();
            State init = GetInitialState();
            if (init != null) init.Enter();
        }
        internal void Exit()
        {
            if (ActiveChild != null) ActiveChild.Exit();
            ActiveChild = null;
            OnExit();
        }
        internal void Update(float deltaTime)
        {
            State target = GetTransition();
            if (target != null)
            {
                Machine.Sequencer.RequestTransition(this, target);
                return;
            }

            ActiveChild?.Update(deltaTime);
            OnUpdate(deltaTime);
        }
        
        // return the deepest currently-active descendent state (the leaf of the active path)
        public State Leaf()
        {
            State state = this;
            while(state.ActiveChild != null) state = state.ActiveChild;
            return state;
        }
        
        // yields this state and then each ancestor up to the root (self -> parent -> grandparent -> ... -> root)
        public IEnumerable<State> PathToRoot()
        {
            for(State state = this; state != null; state = state.Parent) yield return state;
        }
    }
}