using Unity.Entities;

namespace Exhale.ECS.Authoring
{
    public class TileAuthoring : UnityEngine.MonoBehaviour
    {
        public float radius = 1f;   // Radius of the hexagon
        public float height = 1f;   // Height of the hexagonal prism
    }

    public class HexTileBaker : Baker<TileAuthoring>
    {
        public override void Bake(TileAuthoring authoring)
        {
            var hexEntity = GetEntity(TransformUsageFlags.Renderable);
            
        }
    }
}