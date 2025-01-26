using Exhale.ECS.Authoring;
using Exhale.ECS.Systems;
using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class PieceClickSystem : SystemBase
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
            pieceFactorySystem.CreateRandomPiece(tileData.PositionIndex);
            // Example: Add or modify components on the hit entity
            /*if (EntityManager.HasComponent<HexPieceComponent>(hitEntity))
            {
                // Modify existing components
                EntityManager.SetComponentData(hitEntity, new LocalTransform
                {
                    Position = new float3(0, 1, 0), // Example transformation
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
            }
            else
            {
                // Add new components if not already present
                EntityManager.AddComponent<HexPieceComponent>(hitEntity);
                EntityManager.SetComponentData(hitEntity, new LocalTransform
                {
                    Position = new float3(0, 1, 0), // Example transformation
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
            }*/

            Debug.Log($"Entity hit: {hitEntity}");
        }
    }
}

public struct HexPieceComponent : IComponentData { }
