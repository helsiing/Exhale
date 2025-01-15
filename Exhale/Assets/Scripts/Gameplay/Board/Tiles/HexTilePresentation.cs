using DG.Tweening;
using UnityEngine;

namespace Exhale.Gameplay
{
    public interface IHexTilePresentation
    {
        public void Show();
        public void Hide();
    }
    
    public class HexTilePresentation : MonoBehaviour, IHexTilePresentation
    {
        public void Hide()
        {
            transform.localScale = Vector3.zero;
        }
        
        public void Show()
        {
            transform.DOScale(Vector3.one, .5f).SetEase(Ease.Linear);
        }
    }
}