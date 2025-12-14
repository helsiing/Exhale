using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Exhale.Scripts.Data;
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
        [SerializeField] private SplineContainer splineContainer;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform cardsContainer;
        [SerializeField] private float delay = 0.2f;   
        
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
            var hand = gameHandService.Reference.GetHand();
            foreach (var handCard in hand)
            {
                if(handCard.TryGetTrait(out CardObject cardObject))
                {
                    DrawCard(cardObject.Prefab);
                    yield return new WaitForSeconds(delay);
                }
                else
                {
                    Debug.LogError($"Card {handCard.name} does not have a CardObject trait.");
                }
            }
        }

        private void DrawCard(GameObject cardPrefab)
        {
            if(handCards.Count >= gameHandService.Reference.GetInitialHandCount()) return;  
            
            GameObject card = Instantiate(cardPrefab, spawnPoint.position, spawnPoint.rotation, cardsContainer);
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
                
                
                // Update sorting order
                var spriteRenderers = handCards[i].GetComponentsInChildren<SpriteRenderer>();
                foreach (var spriteRenderer in spriteRenderers)
                {
                    spriteRenderer.sortingOrder = i; // or cardCount - i for reverse
                }
            }
        }

        private void OnDestroy()
        {
            ListPool<GameObject>.Release(handCards);
        }
    }
}