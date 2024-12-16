using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public class TilePresentation : MonoBehaviour
    {
        void SetTileTransparency(float alpha)
        {
            // Set the transparency of the tile (assuming it has a Renderer component)
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                Color color = renderer.material.color;
                color.a = alpha;
                renderer.material.color = color;
            }
        }
    }
}