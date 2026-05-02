using Exhale.ECS.Authoring;
using Exhale.ECS.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using RaycastHit = Unity.Physics.RaycastHit;

namespace ECS.Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(TileHighlightSystem))]
    public partial struct TileClickSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<PointerInputData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            PointerInputData inputData = SystemAPI.GetSingleton<PointerInputData>();
            if (!inputData.IsValid || !inputData.IsClickDown)
                return;

            PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            RaycastInput rayInput = new()
            {
                Start  = inputData.RayOrigin,
                End    = inputData.RayOrigin + inputData.RayDirection * 1000f,
                Filter = CollisionFilter.Default
            };

            if (!physicsWorldSingleton.PhysicsWorld.CollisionWorld.CastRay(rayInput, out RaycastHit hit))
                return;

            Entity hitEntity = physicsWorldSingleton.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;

            if (!SystemAPI.HasComponent<TileData>(hitEntity))
                return;

            TileData tileData = SystemAPI.GetComponent<TileData>(hitEntity);
            if (tileData.IsOccupied)
                return;

            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            Entity requestEntity = ecb.CreateEntity();
            ecb.AddComponent(requestEntity, new PieceCreationRequest
            {
                PositionIndex = tileData.PositionIndex,
                PieceId       = -1
            });

            tileData.IsOccupied = true;
            SystemAPI.SetComponent(hitEntity, tileData);
        }
    }
}
