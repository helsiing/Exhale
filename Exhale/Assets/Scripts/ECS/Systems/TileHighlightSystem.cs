using Exhale.ECS.Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

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
            highlightedTile = Entity.Null; // No tile highlighted initially
        }

        public void OnUpdate(ref SystemState state)
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var ray = Camera.main.ScreenPointToRay(mousePosition);

            // Access the physics world
            var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var collisionWorld = physicsWorldSingleton.PhysicsWorld.CollisionWorld;

            // Perform raycast
            var rayInput = new RaycastInput
            {
                Start = ray.origin,
                End = ray.origin + ray.direction * 1000f,
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,    // Collides with everything
                    CollidesWith = ~0u, // Detect all collision layers
                    GroupIndex = 0
                }
            };

            if (collisionWorld.CastRay(rayInput, out var hit))
            {
                var hitEntity = physicsWorldSingleton.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;

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

        private void HighlightTile(ref SystemState state, Entity tileEntity)
        {
            if (tileEntity == highlightedTile)
                return; // The tile is already highlighted

            // Clear the previous highlight
            ClearHighlight(ref state);

            // Apply the highlight to the new tile
            highlightedTile = tileEntity;

            if (SystemAPI.HasComponent<LocalTransform>(tileEntity))
            {
                var transform = SystemAPI.GetComponent<LocalTransform>(tileEntity);
                transform.Scale *= 1.1f; // Slightly scale up the tile
                SystemAPI.SetComponent(tileEntity, transform);
            }

            if (SystemAPI.HasComponent<TileDataHighlight>(tileEntity))
            {
                var highlight = SystemAPI.GetComponent<TileDataHighlight>(tileEntity);
                highlight.IsHighlighted = true;
                SystemAPI.SetComponent(tileEntity, highlight);
            }
        }

        private void ClearHighlight(ref SystemState state)
        {
            if (highlightedTile != Entity.Null)
            {
                if (SystemAPI.HasComponent<LocalTransform>(highlightedTile))
                {
                    var transform = SystemAPI.GetComponent<LocalTransform>(highlightedTile);
                    transform.Scale /= 1.1f; // Restore the original scale
                    SystemAPI.SetComponent(highlightedTile, transform);
                }

                if (SystemAPI.HasComponent<TileDataHighlight>(highlightedTile))
                {
                    var highlight = SystemAPI.GetComponent<TileDataHighlight>(highlightedTile);
                    highlight.IsHighlighted = false;
                    SystemAPI.SetComponent(highlightedTile, highlight);
                }

                highlightedTile = Entity.Null;
            }
        }
    }
}