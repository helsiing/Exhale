using Exhale.ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Exhale.ECS.Authoring
{
    public class TileHighlightConfigAuthoring : MonoBehaviour
    {
        [Header("Tile Colors")]
        [SerializeField] private Color previewColor             = new(0.55f, 0.75f, 1.00f, 1.0f); // soft blue
        [SerializeField] private Color validPlacementColor      = new(0.40f, 0.90f, 0.40f, 0.5f); // soft green
        [SerializeField] private Color invalidRequirementsColor = new(0.90f, 0.30f, 0.30f, 0.5f); // muted red
        [SerializeField] private Color requirementSatisfiedColor = new(0.30f, 0.85f, 0.75f, 0.5f); // teal
        [SerializeField] private Color requirementMissingColor  = new(0.95f, 0.25f, 0.25f, 0.5f); // bright red

        private class Baker : Baker<TileHighlightConfigAuthoring>
        {
            public override void Bake(TileHighlightConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new TileHighlightConfigData
                {
                    PreviewColor             = ToFloat4(authoring.previewColor),
                    ValidPlacementColor      = ToFloat4(authoring.validPlacementColor),
                    InvalidRequirementsColor = ToFloat4(authoring.invalidRequirementsColor),
                    RequirementSatisfiedColor = ToFloat4(authoring.requirementSatisfiedColor),
                    RequirementMissingColor  = ToFloat4(authoring.requirementMissingColor),
                });
            }

            private static float4 ToFloat4(Color c) => new(c.r, c.g, c.b, c.a);
        }
    }
}
