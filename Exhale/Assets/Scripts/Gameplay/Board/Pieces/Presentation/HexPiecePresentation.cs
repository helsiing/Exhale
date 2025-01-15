using DG.Tweening;
using UnityEngine;

namespace Exhale.Gameplay
{
    public class HexPiecePresentation : MonoBehaviour, IHexPiecePresentation
    {
        private void Awake()
        {
            transform.localScale = Vector3.zero;
        }

        public virtual void Show()
        {
            transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBounce);
        }
    }
}