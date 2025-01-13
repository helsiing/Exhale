using System;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public class HexPieceSimulation : MonoBehaviour
    {
        private Action onTileAction;
        
        public void Init(Action onTileAction)
        {
            this.onTileAction = onTileAction;
        }
    }
}