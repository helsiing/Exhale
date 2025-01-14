using DG.Tweening;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public class HexPiecePresentation : MonoBehaviour, IHexPiecePresentation
    {
        private void Awake()
        {
            transform.localScale = Vector3.zero;
        }

        public void Show()
        {
            transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBounce);
        }
    }
}