using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Exhale.Scripts.External.ServiceLocators;
using Exhale.Scripts.Services;
using Exhale.Utils;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Splines;

namespace Exhale.GameHand
{
    public class GameHandUI : MonoBehaviour
    {
        [SerializeField] private GameObject handCardPrefab;
        [SerializeField] private SplineContainer splineContainer;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform cardsContainer;
        
        private List<GameObject> handCards;
        private ServiceReference<IGameHandService> gameHandService = new ();
        
        private void Awake()
        {
            handCards = ListPool<GameObject>.Get();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                DrawInitialHand();
            }
        }

        private void DrawInitialHand()
        {
            cardsContainer.gameObject.DestroyChildObjects();
            handCards.Clear();
            StartCoroutine(DrawInitialHandCoroutine());
        }

        private IEnumerator DrawInitialHandCoroutine()
        {
            var hand = gameHandService.Reference.GetHexTilesInHand();
            float delay = 0.2f; 
            for (int i = 0; i < hand.Count; i++)
            {
                DrawCard();
                yield return new WaitForSeconds(delay);
            }
        }

        private void DrawCard()
        {
            if(handCards.Count >= gameHandService.Reference.GetInitialHandCount()) return;  
             GameObject card = Instantiate(handCardPrefab, spawnPoint.position, spawnPoint.rotation, cardsContainer);
            handCards.Add(card);
            UpdateCardsPosition();
        }
        
        private void UpdateCardsPosition()
        {
            int cardCount = handCards.Count;
            if (cardCount == 0) return;

            float cardSpacing = 1f / gameHandService.Reference.GetInitialHandCount();
            float firstCardPosition = 0.5f - (handCards.Count - 1) * cardSpacing / 2;
            Spline spline = splineContainer.Spline;
            
            for (int i = 0; i < cardCount; i++)
            {
                float p = firstCardPosition + i * cardSpacing;
                Vector3 position = spline.EvaluatePosition(p);
                Vector3 forward = spline.EvaluateTangent(p);
                Vector3 up = spline.EvaluateUpVector(p);
                Quaternion rotation = Quaternion.LookRotation(-up, Vector3.Cross(-up, forward).normalized);
                
                handCards[i].transform.DOMove(position, 0.25f);
                handCards[i].transform.DOLocalRotateQuaternion(rotation, 0.25f);
            }
        }

        private void OnDestroy()
        {
            ListPool<GameObject>.Release(handCards);
        }
    }
}