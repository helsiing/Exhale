using System;
using TouchScript.Gestures;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    [RequireComponent(typeof(LongPressGesture))]
    public class TileSimulation : MonoBehaviour
    {
        private LongPressGesture longPressGesture;
        private Action onTileAction;
        
        private void OnEnable()
        {
            longPressGesture = GetComponent<LongPressGesture>();
            longPressGesture.StateChanged += LongPressGestureOnStateChanged;
        }

        public void Init(Action onTileAction)
        {
            this.onTileAction = onTileAction;
        }
        
        private void OnDisable()
        {
            longPressGesture.StateChanged -= LongPressGestureOnStateChanged;
        }
        
        
        private void LongPressGestureOnStateChanged(object sender, GestureStateChangeEventArgs e)
        {
            if(e.State == Gesture.GestureState.Recognized && e.State != e.PreviousState)
                onTileAction?.Invoke();
        }
    }
}