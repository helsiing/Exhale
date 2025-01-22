using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace Exhale.ECS.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(PhysicsSystemGroup))]
    [BurstCompile]
    public partial class PieceClickSystem : SystemBase
    {
        protected override void OnCreate()
        {
            Debug.Log($"Starting {nameof(PieceClickSystem)}...");
            base.OnCreate();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            // Only process when the left mouse button is clicked
            if (!Input.GetMouseButtonDown(0)) return;

            var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            // Get the mouse click position in screen space and convert to a ray
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float3 rayOrigin = ray.origin;
            float3 rayDirection = ray.direction;

            // Access the CollisionWorld via the BuildPhysicsWorld singleton
            var collisionWorld = physicsWorldSingleton.PhysicsWorld.CollisionWorld;

            // Create a RaycastInput for the physics raycast
            var rayInput = new RaycastInput
            {
                Start = rayOrigin,
                End = rayOrigin + rayDirection * 1000f, // Extend the ray far into the world
                Filter = CollisionFilter.Default // Default collision filter
            };

            // Perform the raycast and check for hits
            if (collisionWorld.CastRay(rayInput, out var hit))
            {
                // Get the entity that was hit
                var hitEntity = physicsWorldSingleton.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                Debug.Log("HIT");
                // Process the hit entity
                //ProcessTileClick(hitEntity);
            }
        }

        /*private void ProcessTileClick(Entity tileEntity)
        {
            var entityManager = EntityManager;

            // Check if the entity has a HexTileTag component
            if (entityManager.HasComponent<HexTileTag>(tileEntity))
            {
                // Remove the HexTileTag to mark the entity as converted
                entityManager.RemoveComponent<HexTileTag>(tileEntity);

                // Add a HexPieceComponent to represent the new piece
                entityManager.AddComponent<HexPieceComponent>(tileEntity);

                // Optionally modify the transform (e.g., scale up the piece)
                if (entityManager.HasComponent<LocalTransform>(tileEntity))
                {
                    var transform = entityManager.GetComponentData<LocalTransform>(tileEntity);
                    transform.Scale *= 1.2f; // Visually indicate the conversion
                    entityManager.SetComponentData(tileEntity, transform);
                }
            }
        }*/
    }
}