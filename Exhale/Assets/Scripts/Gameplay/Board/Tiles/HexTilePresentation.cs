using DG.Tweening;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public interface IHexTilePresentation
    {
        public void Show();
    }
    
    public class HexTilePresentation : MonoBehaviour, IHexTilePresentation
    {
        public void Awake()
        {
            transform.localScale = Vector3.zero;
        }
        
        public void Show()
        {
            transform.DOScale(Vector3.one, .5f).SetEase(Ease.Linear);
        }
    }
}