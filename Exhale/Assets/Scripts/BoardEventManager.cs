using System;
using Unity.Mathematics;
using UnityEngine;

namespace ECS.Scripts.Managers
{
    public class BoardEventManager : MonoBehaviour
    {
        public static event Action<int2> OnBoardInitialized;
        
        public static void TriggerBoardInitialized(int2 startPosition)
        {
            OnBoardInitialized?.Invoke(startPosition);
        }
    }
}