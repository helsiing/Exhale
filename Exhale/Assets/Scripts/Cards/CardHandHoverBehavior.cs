using DG.Tweening;
using UnityEngine;

namespace Exhale.Cards.UI
{
    //TOODO: Refactor to use EventTriggers for better performance and flexibility
    public class CardHandHoverBehavior : MonoBehaviour
    {
        [SerializeField] private float hoverScale = 1.2f;
        [SerializeField] private float hoverHeight = 0.5f;
        [SerializeField] private float animationTime = 0.2f;
        [SerializeField] private int hoverSortingBoost = 100;

        private CardHandPose handPose;
        // Tracks a pending pose update that arrived while the card was hovered.
        // Applied when the card un-hovers so it returns to the correct layout slot.
        private CardHandPose pendingPose;
        private bool hasPendingPose;

        private bool isHovered;
        private bool isAnimating;
        private Vector3 initialScale;

        private Tween moveTween;
        private Tween rotateTween;
        private Tween scaleTween;

        private void Awake()
        {
            initialScale = transform.localScale;
        }

        public void SetHandPose(CardHandPose pose)
        {
            if (isHovered)
            {
                // Card is hovered: store the new target pose for when it un-hovers
                // so the card returns to the correct updated layout position.
                pendingPose = pose;
                hasPendingPose = true;
                return;
            }

            ApplyPose(pose);
        }

        private void ApplyPose(CardHandPose pose)
        {
            handPose = pose;
            hasPendingPose = false;
            isAnimating = true;

            KillTweens();
            moveTween = transform.DOMove(pose.position, animationTime)
                .OnComplete(() => isAnimating = false);
            rotateTween = transform.DORotateQuaternion(pose.rotation, animationTime);

            SetSortingOrder(pose.sortingOrder);
        }

        private void OnMouseEnter()
        {
            if (isAnimating) return;
            Hover();
        }

        private void OnMouseExit()
        {
            UnHover();
        }

        private void Hover()
        {
            if (isHovered) return;
            isHovered = true;

            KillTweens();

            Vector3 hoverPos = handPose.position + Vector3.up * hoverHeight;

            moveTween = transform.DOMove(hoverPos, animationTime);
            rotateTween = transform.DORotateQuaternion(Quaternion.identity, animationTime);
            scaleTween = transform.DOScale(initialScale * hoverScale, animationTime);

            SetSortingOrder(handPose.sortingOrder + hoverSortingBoost);
        }

        private void UnHover()
        {
            if (!isHovered) return;
            isHovered = false;

            // If the layout updated while we were hovered, snap to the new pose.
            if (hasPendingPose)
            {
                ApplyPose(pendingPose);
            }
            else
            {
                KillTweens();
                moveTween = transform.DOMove(handPose.position, animationTime);
                rotateTween = transform.DORotateQuaternion(handPose.rotation, animationTime);
                scaleTween = transform.DOScale(initialScale, animationTime);
            }

            SetSortingOrder(handPose.sortingOrder);
        }

        private void SetSortingOrder(int order)
        {
            foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
                sr.sortingOrder = order;
        }

        private void KillTweens()
        {
            moveTween?.Kill();
            rotateTween?.Kill();
            scaleTween?.Kill();
        }
    }
}
