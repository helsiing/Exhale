using Exhale.Plugins.ServiceLocators;
using Exhale.Scripts.Services;
using Unity.Entities;
using UnityEngine.InputSystem;

namespace ECS.Systems
{
    [UpdateAfter(typeof(TileConfirmSystem))]
    [UpdateBefore(typeof(PiecePlacementSystem))]
    public partial class CancelInputSystem : SystemBase
    {
        private readonly ServiceReference<IPlacementService> placementService = new();

        protected override void OnUpdate()
        {
            var cancelPressed =
                (Mouse.current   != null && Mouse.current.rightButton.wasPressedThisFrame) ||
                (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame);

            if (cancelPressed)
                placementService.Reference?.Cancel();
        }
    }
}
