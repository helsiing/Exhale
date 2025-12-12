using Exhale.Scripts.External.ServiceLocators;
using UnityEngine;

namespace Exhale.Scripts.Services
{
    public interface IGameService : IService
    {
        
    }
    
    public class GameService : MonoBehaviour, IGameService
    {
        [SerializeField] private int remainingMovesAtStart = 20;
        public int RemainingMovesAtStart => remainingMovesAtStart;
        
        public void Dispose()
        {
        }
    }
}