using Exhale.ECS.Authoring;
using Exhale.ECS.Systems;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace ECS.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class TileClickSystem : SystemBase
    {
        private PieceFactorySystem pieceFactorySystem;

        protected override void OnCreate()
        {
            // Ensure the system only runs when the PhysicsWorldSingleton exists
            RequireForUpdate<PhysicsWorldSingleton>();
            pieceFactorySystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PieceFactorySystem>();
        }

        protected override void OnUpdate()
        {
            // Check if the user has clicked the left mouse button
            if (!Input.GetMouseButtonDown(0)) return;

            // Retrieve the PhysicsWorldSingleton for raycasting
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

            // Get the mouse click position and generate a ray
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var rayInput = new RaycastInput
            {
                Start = ray.origin,
                End = ray.origin + ray.direction * 1000f,
                Filter = CollisionFilter.Default
            };

            // Perform the raycast
            if (physicsWorld.CollisionWorld.CastRay(rayInput, out var hit))
            {
                // Get the entity that was hit
                var hitEntity = physicsWorld.Bodies[hit.RigidBodyIndex].Entity;

                var tileData = EntityManager.GetComponentData<TileData>(hitEntity);
                if(tileData.IsOccupied) return; // Ignore if the tile is already occupied
                
                pieceFactorySystem.CreateRandomPiece(tileData.PositionIndex);
                
                tileData.IsOccupied = true;
                EntityManager.SetComponentData(hitEntity, tileData);
                
                Debug.Log($"Entity hit: {hitEntity}");
            }
        }
    }
}