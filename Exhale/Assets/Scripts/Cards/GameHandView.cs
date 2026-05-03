using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Exhale.Plugins.ServiceLocators;
using Exhale.Scripts.Data;
using Exhale.Scripts.Services;
using Exhale.Utils;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Splines;

namespace Exhale.Cards.UI
{
    public struct CardHandPose
    {
        public Vector3 position;
        public Quaternion rotation;
        public int sortingOrder;
    }
    
    public class GameHandView : MonoBehaviour
    {
        [SerializeField] private SplineContainer splineContainer;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform cardsContainer;
        [SerializeField] private float delay = 0.2f;   
        
        private List<GameObject> handCards;
        public IReadOnlyList<GameObject> HandCards => handCards;

        private ServiceReference<IGameHandService> gameHandService = new();
        private readonly ServiceReference<IPlacementService> placementService = new();

        private void Awake()
        {
            handCards = ListPool<GameObject>.Get();
        }

        private void Start()
        {
            if (placementService.Reference != null)
                placementService.Reference.OnCardLanded += OnCardLanded;
            StartCoroutine(AutoDrawInitialHand());
        }

        private IEnumerator AutoDrawInitialHand()
        {
            yield return null; // wait one frame so GameHandService.Start() has populated the hand
            DrawInitialHand();
        }

        private void DrawInitialHand()
        {
            cardsContainer.gameObject.DestroyChildObjects();
            handCards.Clear();
            StartCoroutine(DrawInitialHandCoroutine());
        }

        private void OnCardLanded(HexPieceTemplate _, int2 __)
        {
            var nextCard = gameHandService.Reference?.DrawNextCard();
            if (nextCard == null) return;
            if (nextCard.TryGetTrait(out CardObject cardObject))
                DrawCard(nextCard, cardObject.Prefab);
        }

        private IEnumerator DrawInitialHandCoroutine()
        {
            var hand = gameHandService.Reference.GetHand();
            foreach (var handCard in hand)
            {
                if(handCard.TryGetTrait(out CardObject cardObject))
                {
                    DrawCard(handCard, cardObject.Prefab);
                    yield return new WaitForSeconds(delay);
                }
                else
                {
                    Debug.LogError($"Card {handCard.name} does not have a CardObject trait.");
                }
            }
        }

        private void DrawCard(HexPieceTemplate template, GameObject cardPrefab)
        {
            if(handCards.Count >= gameHandService.Reference.GetInitialHandCount()) return;

            // LeanPivot sits between cardsContainer and the card visual. Its localRotation
            // is the dedicated layer for CardHandLeanController; CardHandHoverBehavior lives
            // on the pivot so position/scale tweens operate on the same transform, leaving
            // the card's own transform free for any future per-card local animation.
            var leanPivot = new GameObject("LeanPivot");
            leanPivot.transform.SetParent(cardsContainer, false);
            leanPivot.transform.position = spawnPoint.position;
            leanPivot.transform.rotation = spawnPoint.rotation;

            var card = Instantiate(cardPrefab, leanPivot.transform);
            card.transform.localPosition = Vector3.zero;
            card.transform.localRotation = Quaternion.identity;
            card.GetOrAddComponent<CardMouseEventForwarder>();
            card.GetOrAddComponent<CardView>().Initialize(template);

            handCards.Add(leanPivot);
            UpdateCardsPosition();
        }
        
        public void RemoveCard(GameObject leanPivot)
        {
            handCards.Remove(leanPivot);
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

                var cardHandHoverUI = handCards[i].GetOrAddComponent<CardHandHoverBehavior>();
                cardHandHoverUI.SetHandPose(new CardHandPose
                {
                    position = position,
                    rotation = rotation,
                    sortingOrder = i
                });
            }
        }

        private void OnDestroy()
        {
            if (placementService.Reference != null)
                placementService.Reference.OnCardLanded -= OnCardLanded;
            ListPool<GameObject>.Release(handCards);
        }
    }
}