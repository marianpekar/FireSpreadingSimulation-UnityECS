namespace MarianPekar.FireSpreadingSimulation
{
    using Unity.Physics.Systems;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;
    using Unity.Physics;

    public static class Raycaster
    {
        private static RaycastInput raycastInput;
        private static BuildPhysicsWorld buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
        private static StepPhysicsWorld stepPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<StepPhysicsWorld>();
        public static NativeList<RaycastHit> raycastHits = new NativeList<RaycastHit>(Allocator.Persistent);

        public struct RaycastJob : IJob
        {
            public RaycastInput Input;
            public NativeList<RaycastHit> Hits;
            [ReadOnly] public PhysicsWorld World;

            public void Execute()
            {
                World.CastRay(Input, out var hit);
                Hits.Add(hit);
            }
        }

        public static Entity GetEntityWithRaycast(float3 origin, float3 direction)
        {
            stepPhysicsWorld.GetOutputDependency().Complete();
            raycastHits.Clear();

            raycastInput = new RaycastInput
            {
                Start = origin,
                End = origin + direction,
                Filter = CollisionFilter.Default
            };

            var jobHandle = new RaycastJob
            {
                Input = raycastInput,
                Hits = raycastHits,
                World = buildPhysicsWorld.PhysicsWorld
            }.Schedule();

            jobHandle.Complete();

            foreach (var raycastHit in raycastHits.ToArray())
            {
                var entity = buildPhysicsWorld.PhysicsWorld.Bodies[raycastHit.RigidBodyIndex].Entity;
                return entity;
            }

            return Entity.Null;
        }
    }
}

