using Exhale.Scripts.Data;
using Unity.Entities;
using UnityEngine;

namespace Exhale.ECS.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class PieceTemplatesInitializationSystem : SystemBase
    {
        protected override void OnStartRunning()
        {
            // Retrieve the HexPrefabCollection singleton from the ECS world
            Entities
                .WithAll<HexPieceTemplateCollection>()
                .ForEach((HexPieceTemplateCollection hexPrefabCollection) =>
                {
                    // Access and process your prefab data
                    foreach (var hexPiece in hexPrefabCollection.Items) Debug.Log($"Loaded Hex Piece: {hexPiece.name}");
                }).WithoutBurst().Run(); // WithoutBurst is necessary since we're using managed objects
        }

        protected override void OnUpdate()
        {
        } // No need for regular updates
    }
}