using System;
using TouchScript.Gestures;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public class HexTileSimulation : MonoBehaviour
    {
        private Action onTileAction;
        
        private void OnEnable()
        {
           
        }

        public void Init(Action onTileAction)
        {
            this.onTileAction = onTileAction;
        }
        
        private void OnDisable()
        {
        }
        
        private void LongPressGestureOnStateChanged(object sender, GestureStateChangeEventArgs e)
        {
            if(e.State == Gesture.GestureState.Recognized && e.State != e.PreviousState)
                onTileAction?.Invoke();
        }
    }
}