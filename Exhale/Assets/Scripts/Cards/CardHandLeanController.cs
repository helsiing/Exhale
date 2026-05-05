using Exhale.Plugins.ServiceLocators;
using Exhale.Scripts.Data;
using Exhale.Scripts.Services;
using Exhale.Utils;
using Unity.Mathematics;
using UnityEngine;

namespace Exhale.Cards.UI
{
    [RequireComponent(typeof(GameHandView))]
    public class CardHandLeanController : MonoBehaviour
    {
        [Header("Lean")]
        [SerializeField] [Range(0f, 45f)]   private float leanAngle     = 10f;
        [SerializeField] [Range(0.05f, 1f)] private float animationTime = 0.3f;

        private GameHandView handView;
        private readonly ServiceReference<IPlacementService> placementService = new();

        private void Awake() => handView = GetComponent<GameHandView>();

        private void Start()
        {
            if (placementService.Reference == null) return;
            placementService.Reference.OnCardLaunchStarted += OnCardLaunchStarted;
            placementService.Reference.OnCardDeselected    += ResetLean;
            placementService.Reference.OnCardLanded        += OnCardLanded;
        }

        private void OnDestroy()
        {
            if (!placementService.HasCachedReference) return;
            placementService.CachedReference.OnCardLaunchStarted -= OnCardLaunchStarted;
            placementService.CachedReference.OnCardDeselected    -= ResetLean;
            placementService.CachedReference.OnCardLanded        -= OnCardLanded;
        }

        private void OnCardLaunchStarted(HandCard _, int2 tilePos)
        {
            Vector3 tileWorldPos = BoardHelper.HexToWorldPosition(tilePos);
            foreach (var leanPivot in handView.HandCards)
            {
                var hover = leanPivot.GetComponent<CardHandHoverBehavior>();
                if (hover == null) continue;
                hover.SetLeanDelta(ComputeLeanDelta(leanPivot.transform.position, tileWorldPos));
            }
        }

        private void OnCardLanded(HandCard _, int2 __) => ResetLean();

        private void ResetLean()
        {
            foreach (var leanPivot in handView.HandCards)
                leanPivot.GetComponent<CardHandHoverBehavior>()?.SetLeanDelta(Quaternion.identity);
        }

        private Quaternion ComputeLeanDelta(Vector3 cardPos, Vector3 tilePos)
        {
            Vector3 dir = tilePos - cardPos;
            dir.y = 0;
            if (dir.sqrMagnitude < 0.01f) return Quaternion.identity;
            dir.Normalize();
            // Lean axis is perpendicular to world-up and the card→tile direction,
            // producing a forward tilt of leanAngle degrees toward the tile.
            return Quaternion.AngleAxis(leanAngle, Vector3.Cross(Vector3.up, dir));
        }
    }
}
