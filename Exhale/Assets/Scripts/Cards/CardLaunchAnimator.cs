using DG.Tweening;
using Exhale.Plugins.ServiceLocators;
using Exhale.Scripts.Data;
using Exhale.Scripts.Services;
using Exhale.Utils;
using Unity.Mathematics;
using UnityEngine;

namespace Exhale.Cards.UI
{
    [RequireComponent(typeof(GameHandView))]
    public class CardLaunchAnimator : MonoBehaviour
    {
        [Header("Arc")]
        [SerializeField] [Range(0.05f, 2f)] private float duration      = 0.25f;
        [SerializeField] [Range(0f, 10f)]   private float arcHeight     = 2f;
        [SerializeField] [Range(0f, 1f)]    private float landingHeight = 0.15f;

        [Header("Impact")]
        [SerializeField] [Range(0f, 1f)]    private float shakeStrength = 0.1f;
        [SerializeField] [Range(0f, 1f)]    private float shakeDuration = 0.15f;

        private GameHandView handView;
        private readonly ServiceReference<IPlacementService> placementService = new();

        private void Awake() => handView = GetComponent<GameHandView>();

        private void Start()
        {
            if (placementService.Reference == null) return;
            placementService.Reference.OnCardLaunchStarted += OnCardLaunchStarted;
        }

        private void OnDestroy()
        {
            if (placementService.Reference == null) return;
            placementService.Reference.OnCardLaunchStarted -= OnCardLaunchStarted;
        }

        private void OnCardLaunchStarted(HexPieceTemplate card, int2 tilePos)
        {
            var leanPivot = FindLeanPivotForCard(card);
            if (leanPivot == null)
            {
                placementService.Reference?.NotifyCardLanded();
                return;
            }

            handView.RemoveCard(leanPivot);
            leanPivot.transform.SetParent(null, true);

            float3 hexWorld = BoardHelper.HexToWorldPosition(tilePos);
            var landing = new Vector3(hexWorld.x, landingHeight, hexWorld.z);
            var start   = leanPivot.transform.position;
            var mid     = Vector3.Lerp(start, landing, 0.5f) + Vector3.up * arcHeight;

            var seq = DOTween.Sequence();
            seq.Append(leanPivot.transform.DOPath(new[] { mid, landing }, duration, PathType.CatmullRom));
            seq.Join(leanPivot.transform.DORotateQuaternion(Quaternion.identity, duration));
            seq.AppendCallback(() =>
            {
                Camera.main.DOShakePosition(shakeDuration, shakeStrength);
                placementService.Reference?.NotifyCardLanded();
                Destroy(leanPivot);
            });
        }

        private GameObject FindLeanPivotForCard(HexPieceTemplate card)
        {
            foreach (var leanPivot in handView.HandCards)
            {
                var cardView = leanPivot.GetComponentInChildren<CardView>();
                if (cardView != null && cardView.Template == card)
                    return leanPivot;
            }
            return null;
        }
    }
}
