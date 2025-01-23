using Exhale.Scripts.Data;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Authoring
{
    public class PieceAuthoring : MonoBehaviour
    {
        [SerializeField] private HexPieceTemplate pieceTemplate;
        
        public class Baker : Baker<PieceAuthoring>
        {
            public override void Bake(PieceAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                
                // Add the PieceTemplateData component to the entity
                PieceTemplateData componentData = new PieceTemplateData
                {
                    PieceId = authoring.pieceTemplate.GetId()
                };

                AddComponent(entity, componentData);
                
                AddComponent<LocalToWorld>(entity); 
                AddComponent<Prefab>(entity);
            }
        }
    }
}