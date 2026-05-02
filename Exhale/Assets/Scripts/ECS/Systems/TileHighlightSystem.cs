using Exhale.ECS.Authoring;
using Exhale.ECS.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using RaycastHit = Unity.Physics.RaycastHit;

namespace ECS.Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(BuildPhysicsWorld))]
    public partial struct TileHighlightSystem : ISystem
    {
        private Entity highlightedTile;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<PointerInputData>();
            highlightedTile = Entity.Null;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            PointerInputData inputData = SystemAPI.GetSingleton<PointerInputData>();
            if (!inputData.IsValid)
                return;

            // Access the physics world
            PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.PhysicsWorld.CollisionWorld;

            // Perform raycast
            RaycastInput rayInput = new()
            {
                Start = inputData.RayOrigin,
                End   = inputData.RayOrigin + inputData.RayDirection * 1000f,
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u, // Collides with everything
                    CollidesWith = ~0u, // Detect all collision layers
                    GroupIndex = 0
                }
            };

            if (collisionWorld.CastRay(rayInput, out RaycastHit hit))
            {
                Entity hitEntity = physicsWorldSingleton.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;

                if (SystemAPI.HasComponent<TileData>(hitEntity))
                {
                    // Highlight the hovered tile
                    HighlightTile(ref state, hitEntity);
                }
            }
            else
            {
                // Clear the highlight if no tile is hovered
                ClearHighlight(ref state);
            }
        }

        [BurstCompile]
        private void HighlightTile(ref SystemState state, Entity tileEntity)
        {
            if (tileEntity == highlightedTile)
            {
                return; // The tile is already highlighted
            }
            
            if (SystemAPI.HasComponent<TileDataHighlight>(tileEntity))
            {
                TileDataHighlight highlightData = SystemAPI.GetComponent<TileDataHighlight>(highlightedTile);
                if(highlightData.IsHighlighted)
                {
                    return; // The tile is already highlighted
                }
            }

            // Clear the previous highlight
            ClearHighlight(ref state);

            // Apply the highlight to the new tile
            highlightedTile = tileEntity;

            if (SystemAPI.HasComponent<LocalTransform>(tileEntity))
            {
                LocalTransform transform = SystemAPI.GetComponent<LocalTransform>(tileEntity);
                transform.Scale *= 1.1f; // Slightly scale up the tile
                SystemAPI.SetComponent(tileEntity, transform);
            }

            if (SystemAPI.HasComponent<TileDataHighlight>(tileEntity))
            {
                TileDataHighlight highlight = SystemAPI.GetComponent<TileDataHighlight>(tileEntity);
                highlight.IsHighlighted = true;
                SystemAPI.SetComponent(tileEntity, highlight);
            }
        }

        [BurstCompile]
        private void ClearHighlight(ref SystemState state)
        {
            if (highlightedTile != Entity.Null)
            {
                if (SystemAPI.HasComponent<LocalTransform>(highlightedTile))
                {
                    LocalTransform transform = SystemAPI.GetComponent<LocalTransform>(highlightedTile);
                    transform.Scale /= 1.1f; // Restore the original scale
                    SystemAPI.SetComponent(highlightedTile, transform);
                }

                if (SystemAPI.HasComponent<TileDataHighlight>(highlightedTile))
                {
                    TileDataHighlight highlight = SystemAPI.GetComponent<TileDataHighlight>(highlightedTile);
                    highlight.IsHighlighted = false;
                    SystemAPI.SetComponent(highlightedTile, highlight);
                }

                highlightedTile = Entity.Null;
            }
        }
    }
}