using System;
using Unity.Entities;

namespace Exhale.Scripts.Components
{
    [Serializable] 
    public struct PieceTemplateData : IComponentData
    {
        public int PieceId;
    }
}