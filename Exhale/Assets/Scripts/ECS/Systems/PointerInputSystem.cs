using Exhale.ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Exhale.ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial class PointerInputSystem : SystemBase
    {
        protected override void OnCreate()
        {
            EntityManager.CreateSingleton<PointerInputData>();
        }

        protected override void OnUpdate()
        {
            var camera = Camera.main;
            if (camera == null || Mouse.current == null)
            {
                SystemAPI.SetSingleton(new PointerInputData { IsValid = false });
                return;
            }

            Vector2 screenPos = Mouse.current.position.ReadValue();
            Ray ray = camera.ScreenPointToRay(screenPos);

            SystemAPI.SetSingleton(new PointerInputData
            {
                RayOrigin    = ray.origin,
                RayDirection = ray.direction,
                IsClickDown  = Mouse.current.leftButton.wasPressedThisFrame,
                IsValid      = true
            });
        }
    }
}
