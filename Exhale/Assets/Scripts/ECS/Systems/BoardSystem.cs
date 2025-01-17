using Exhale.ECS.Jobs;
using Exhale.Scripts.Data;
using Unity.Entities;

namespace Exhale.ECS.Systems
{
    public partial class BoardSystem : SystemBase
    {
        protected override void OnCreate()
        {
            // Run this system once on startup
            RequireForUpdate<BoardData>();
        }

        protected override void OnStartRunning()
        {
            EndSimulationEntityCommandBufferSystem ecbSystem =
                World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            EntityCommandBuffer.ParallelWriter ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

            CreateGridJob job = new()
            {
                Ecb = ecb
            };

            Dependency = job.ScheduleParallel(Dependency);
            ecbSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnUpdate()
        {
        }
    }
}