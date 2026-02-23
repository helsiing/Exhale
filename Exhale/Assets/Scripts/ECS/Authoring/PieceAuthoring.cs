using Exhale.Scripts.Components;
using Exhale.Scripts.Data;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Exhale.ECS.Authoring
{
    public class PieceAuthoring : MonoBehaviour
    {
        [SerializeField] private HexPieceTemplate pieceTemplate;
        public HexPieceTemplate PieceTemplate => pieceTemplate;

        public void Init(HexPieceTemplate pieceTemplate)
        {
            this.pieceTemplate = pieceTemplate;
        }
        
        private class Baker : Baker<PieceAuthoring>
        {
            public override void Bake(PieceAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Renderable);
                
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