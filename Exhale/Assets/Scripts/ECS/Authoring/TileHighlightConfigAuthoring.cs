using Exhale.ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Exhale.ECS.Authoring
{
    public class TileHighlightConfigAuthoring : MonoBehaviour
    {
        [Header("Tile Colors")]
        [SerializeField] private Color armedColor   = new(1.00f, 0.65f, 0.10f, 1f); // warm amber
        [SerializeField] private Color previewColor = new(0.55f, 0.75f, 1.00f, 1f); // soft blue

        private class Baker : Baker<TileHighlightConfigAuthoring>
        {
            public override void Bake(TileHighlightConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new TileHighlightConfigData
                {
                    ArmedColor   = ToFloat4(authoring.armedColor),
                    PreviewColor = ToFloat4(authoring.previewColor),
                });
            }

            private static float4 ToFloat4(Color c) => new(c.r, c.g, c.b, c.a);
        }
    }
}
