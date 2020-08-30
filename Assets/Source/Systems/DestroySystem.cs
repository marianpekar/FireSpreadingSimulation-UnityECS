namespace MarianPekar.FireSpreadingSimulation
{
    using Unity.Jobs;
    using Unity.Entities;

    public class DestroySystem : JobComponentSystem
    {
        EndSimulationEntityCommandBufferSystem bufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            bufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            bufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            var commandBuffer = bufferSystem.CreateCommandBuffer().AsParallelWriter();

            var jobHandle = Entities.WithAll<DestroyData>().ForEach((Entity entity, int entityInQueryIndex, ref DestroyData destroyData) =>
            {
                if(destroyData.Destroy)
                    commandBuffer.DestroyEntity(entityInQueryIndex, entity);
            }).Schedule(inputDeps);

            bufferSystem.AddJobHandleForProducer(jobHandle);

            jobHandle.Complete();
            return default;
        }
    }
}
