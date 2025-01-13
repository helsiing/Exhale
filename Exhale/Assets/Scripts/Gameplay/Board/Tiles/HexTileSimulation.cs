using System;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public class HexTileSimulation : MonoBehaviour
    {
        private Action onTileAction;
        
        public void Init(Action onTileAction)
        {
            this.onTileAction = onTileAction;
        }
    }
}