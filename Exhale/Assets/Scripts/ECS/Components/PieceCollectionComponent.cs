using Exhale.Scripts.Data;
using Unity.Entities;

namespace Exhale.ECS.Components
{
    public struct PieceCollectionComponent : IComponentData
    {
        public HexPieceTemplateCollection Data; // Reference to the entity holding the data
    }
}