using System.Collections.Generic;
using Exhale.Scripts.Data;
using Exhale.Scripts.External.ServiceLocators;
using UnityEngine;
using UnityEngine.Pool;

namespace Exhale.Scripts.Services
{
    public interface IGameHandService : IService
    {
        public void InitializeHand();
        public int GetInitialHandCount();
        public int GetMaxHandCount();
        public List<HexPieceTemplate> GetHexTilesInHand();
    }
    
    public class GameHandService : MonoBehaviour, IGameHandService
    {
        [SerializeField] private int initialHandCount = 7;
        [SerializeField] private int maxHandCount = 10;

        private readonly ServiceReference<IDataService> dataService = new ();
        private List<HexPieceTemplate> hexTilesInHand;
        private List<HexPieceTemplate> hexTilesAvailable;
        
        
        private void Awake()
        {
            hexTilesInHand = ListPool<HexPieceTemplate>.Get();
            hexTilesAvailable = ListPool<HexPieceTemplate>.Get();
        }
        
        private void Start()
        {
            InitializeHand();
        }

        public void InitializeHand()
        {
            hexTilesAvailable = dataService.Reference.GetHexTilesAvailable();
            for (var i = 0; i < initialHandCount; i++)
            {
                var randomIndex = Random.Range(0, hexTilesAvailable.Count);
                hexTilesInHand.Add(hexTilesAvailable[randomIndex]);
            }
        }

        public int GetInitialHandCount()
        {
            return initialHandCount;
        }

        public int GetMaxHandCount()
        {
            return maxHandCount;
        }

        public List<HexPieceTemplate> GetHexTilesInHand()
        {
            return hexTilesInHand;
        }

        public void Dispose()
        {
            ListPool<HexPieceTemplate>.Release(hexTilesInHand);
            ListPool<HexPieceTemplate>.Release(hexTilesAvailable);
        }
    }
}