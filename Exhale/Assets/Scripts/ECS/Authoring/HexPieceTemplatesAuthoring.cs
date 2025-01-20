using Exhale.Scripts.Data;
using Unity.Entities;
using UnityEngine;

namespace Exhale.ECS.Authoring
{
    public class HexPieceTemplatesAuthoring : MonoBehaviour
    {
        [SerializeField] private HexPieceTemplateCollection pieceTemplateCollection;
        
        private class Baker : Baker<HexPieceTemplatesAuthoring>
        {
            public override void Bake(HexPieceTemplatesAuthoring authoring)
            {
                foreach (HexPieceTemplate pieceTemplate in authoring.pieceTemplateCollection)
                {
                    Entity entity = GetEntity(TransformUsageFlags.None);
                    HexPieceTemplateData componentData = pieceTemplate.Data;
                    
                    // bake the prefab into entity
                    componentData.PiecePrefabEntity = GetEntity(pieceTemplate.PiecePrefab, TransformUsageFlags.Dynamic);
                    AddComponent(entity, componentData);
                }
            }
        }
    }
}