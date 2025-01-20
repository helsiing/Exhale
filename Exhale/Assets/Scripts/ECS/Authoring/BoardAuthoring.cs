using Exhale.Scripts.Data;
using Unity.Entities;
using UnityEngine;

namespace Exhale.ECS.Authoring
{
    public class BoardAuthoring : MonoBehaviour
    {
        [SerializeField] private BoardConfig boardConfig;

        private class Baker : Baker<BoardAuthoring>
        {
            public override void Bake(BoardAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Renderable);
                BoardDataComponent componentData = authoring.boardConfig.Data;
                
                // bake the prefab into entity
                componentData.HexTilePrefabEntity = GetEntity(authoring.boardConfig.HexTilePiecePrefab, TransformUsageFlags.Dynamic);
                AddComponent(entity, componentData);
            }
        }
    }
}
