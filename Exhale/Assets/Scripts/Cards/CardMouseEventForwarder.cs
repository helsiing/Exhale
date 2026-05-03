using UnityEngine;

namespace Exhale.Cards.UI
{
    public class CardMouseEventForwarder : MonoBehaviour
    {
        private void OnMouseEnter() =>
            GetComponentInParent<CardHandHoverBehavior>()?.TriggerHover();

        private void OnMouseExit() =>
            GetComponentInParent<CardHandHoverBehavior>()?.TriggerUnhover();
    }
}
