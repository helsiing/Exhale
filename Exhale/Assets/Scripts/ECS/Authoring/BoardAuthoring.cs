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
                var componentData = authoring.boardConfig.Data;
                componentData.HexTilePrefab = GetEntity(authoring.boardConfig.HexTilePiecePrefab, TransformUsageFlags.Renderable);
                AddComponent(entity, componentData);
            }
        }
    }
}
