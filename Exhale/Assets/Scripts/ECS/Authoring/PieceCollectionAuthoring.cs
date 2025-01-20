using Exhale.ECS.Components;
using Exhale.Scripts.Data;
using Unity.Entities;
using UnityEngine;

namespace Exhale.ECS.Authoring
{
    public class PieceCollectionAuthoring : MonoBehaviour
    {
        [SerializeField] private HexPieceTemplateCollection pieceTemplateCollection;

        private class Baker : Baker<PieceCollectionAuthoring>
        {
            public override void Bake(PieceCollectionAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                PieceCollectionComponent componentData = new PieceCollectionComponent
                {
                    Data = authoring.pieceTemplateCollection
                };
                //AddComponent(entity, componentData);
            }
        }
    }
}