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
            {
                ClearHighlight(ref state);
                return;
            }

            PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.PhysicsWorld.CollisionWorld;

            RaycastInput rayInput = new()
            {
                Start  = inputData.RayOrigin,
                End    = inputData.RayOrigin + inputData.RayDirection * 1000f,
                Filter = new CollisionFilter
                {
                    BelongsTo    = ~0u,
                    CollidesWith = ~0u,
                    GroupIndex   = 0
                }
            };

            if (collisionWorld.CastRay(rayInput, out RaycastHit hit))
            {
                Entity hitEntity = physicsWorldSingleton.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;

                if (SystemAPI.HasComponent<TileData>(hitEntity))
                    HighlightTile(ref state, hitEntity);
                else
                    ClearHighlight(ref state);
            }
            else
            {
                ClearHighlight(ref state);
            }
        }

        [BurstCompile]
        private void HighlightTile(ref SystemState state, Entity tileEntity)
        {
            // Only scale-highlight tiles that ValidTileHighlightSystem has marked as valid for
            // the selected card. When no card is selected there are no tagged tiles, so hover
            // is intentionally inert — avoids misleading feedback in the Idle state.
            if (!SystemAPI.HasComponent<TileValidForPlacementTag>(tileEntity))
            {
                ClearHighlight(ref state);
                return;
            }

            if (tileEntity == highlightedTile) return;

            ClearHighlight(ref state);

            highlightedTile = tileEntity;

            if (SystemAPI.HasComponent<LocalTransform>(tileEntity))
            {
                LocalTransform transform = SystemAPI.GetComponent<LocalTransform>(tileEntity);
                transform.Scale *= 1.1f;
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
            if (highlightedTile == Entity.Null) return;

            if (SystemAPI.HasComponent<LocalTransform>(highlightedTile))
            {
                LocalTransform transform = SystemAPI.GetComponent<LocalTransform>(highlightedTile);
                transform.Scale /= 1.1f;
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
