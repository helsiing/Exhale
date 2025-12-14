using DG.Tweening;
using UnityEngine;

namespace Exhale.GameHand
{
    //TOODO: Refactor to use EventTriggers for better performance and flexibility
    public class CardHandHoverUI : MonoBehaviour
    {
        [SerializeField] private float hoverScale = 1.2f;
        [SerializeField] private float hoverHeight = 0.5f;
        [SerializeField] private float animationTime = 0.2f;
        [SerializeField] private int hoverSortingBoost = 100;

        private CardHandPose handPose;
        private bool isHovered;
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
            if (isHovered) return;

            handPose = pose;

            transform.DOMove(pose.position, animationTime);
            transform.DORotateQuaternion(pose.rotation, animationTime);

            SetSortingOrder(pose.sortingOrder);
        }

        private void OnMouseEnter()
        {
            Hover();
        }

        private void OnMouseExit()
        {
            Unhover();
        }

        private void Hover()
        {
            if (isHovered) return;
            isHovered = true;

            KillTweens();

            Vector3 hoverPos = handPose.position + Vector3.up * hoverHeight;

            moveTween = transform.DOMove(hoverPos, animationTime);
            //rotateTween = transform.DORotate(Vector3.zero, animationTime);
            scaleTween = transform.DOScale(initialScale * hoverScale, animationTime);

            SetSortingOrder(handPose.sortingOrder + hoverSortingBoost);
        }

        private void Unhover()
        {
            if (!isHovered) return;
            isHovered = false;

            KillTweens();

            moveTween = transform.DOMove(handPose.position, animationTime);
            //rotateTween = transform.DORotateQuaternion(handPose.rotation, animationTime);
            scaleTween = transform.DOScale(initialScale, animationTime);

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