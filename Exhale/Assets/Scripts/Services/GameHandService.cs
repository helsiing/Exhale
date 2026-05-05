using System.Collections.Generic;
using Exhale.Scripts.Data;
using Exhale.Plugins.ServiceLocators;
using UnityEngine;
using UnityEngine.Pool;

namespace Exhale.Scripts.Services
{
    public interface IGameHandService : IService
    {
        public void InitializeHand();
        public int GetInitialHandCount();
        public int GetMaxHandCount();
        public List<HandCard> GetHand();
        public HandCard DrawNextCard();
    }

    public class GameHandService : MonoBehaviour, IGameHandService
    {
        [SerializeField] private int initialHandCount = 7;
        [SerializeField] private int maxHandCount = 10;

        private readonly ServiceReference<IDataService> dataService = new ();
        private List<HandCard> hexTilesInHand;
        private List<HexPieceTemplate> hexTilesAvailable;


        private void Awake()
        {
            hexTilesInHand = ListPool<HandCard>.Get();
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
                hexTilesInHand.Add(new HandCard(hexTilesAvailable[randomIndex]));
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

        public List<HandCard> GetHand()
        {
            return hexTilesInHand;
        }

        public HandCard DrawNextCard()
        {
            if (hexTilesAvailable == null || hexTilesAvailable.Count == 0) return null;
            return new HandCard(hexTilesAvailable[Random.Range(0, hexTilesAvailable.Count)]);
        }

        public void Dispose()
        {
            ListPool<HandCard>.Release(hexTilesInHand);
            ListPool<HexPieceTemplate>.Release(hexTilesAvailable);
        }
    }
}