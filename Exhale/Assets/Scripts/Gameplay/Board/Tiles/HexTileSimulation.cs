using System;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public interface IHexTileSimulation
    {
        public void Init(Action onTileAction);
    }
    
    public class HexTileSimulation : MonoBehaviour
    {
        private Action onTileAction;
        
        public void Init(Action onTileAction)
        {
            this.onTileAction = onTileAction;
        }
    }
}