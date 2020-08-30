namespace MarianPekar.FireSpreadingSimulation
{
    using System;
    using Unity.Collections;
    using Unity.Jobs;
    using UnityEngine;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Physics.Systems;

    public enum MouseMode
    {
        Add, Remove, ToggleFire
    }

    public class MouseInputManager : MonoBehaviour
    {
        private RaycastInput raycastInput;
        private NativeList<Unity.Physics.RaycastHit> raycastHits;

        private BuildPhysicsWorld buildPhysicsWorld;
        private StepPhysicsWorld stepPhysicsWorld;

        [SerializeField] private Spawner spawner;
        [SerializeField] private int spawnableIndex = 0;

        public MouseMode currentMode = MouseMode.Add;

        private int mouseModeCount;
        private int flamableStatesCount;
        private const float raycastDistance = 4000f;

        private Camera camera;

        public struct RaycastJob : IJob
        {
            public RaycastInput Input;
            public NativeList<Unity.Physics.RaycastHit> Hits;
            [ReadOnly] public PhysicsWorld World;

            public void Execute()
            {
                World.CastRay(Input, out var hit);
                Hits.Add(hit);
            }
        }

        void Start()
        {
            mouseModeCount = Enum.GetNames(typeof(MouseMode)).Length;
            flamableStatesCount = Enum.GetNames(typeof(FlamableState)).Length;

            camera = Camera.main;

            raycastHits = new NativeList<Unity.Physics.RaycastHit>(Allocator.Persistent);

            buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
            stepPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<StepPhysicsWorld>();
        }

        void OnDestroy()
        {
            if (raycastHits.IsCreated)
                raycastHits.Dispose();
        }

        void LateUpdate()
        {
            stepPhysicsWorld.GetOutputDependency().Complete();

            if (!Input.GetMouseButtonDown(0)) return;
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            float3 origin = ray.origin;
            float3 direction = ray.direction * raycastDistance;

            raycastHits.Clear();

            switch (currentMode)
            {
                case MouseMode.Add:
                    if (Physics.Raycast(ray, out var hit))
                        spawner.SpawnOne(spawnableIndex, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                    
                    break;
                case MouseMode.Remove:
                    var entityToDestroy = GetEntityWithRaycast(origin, direction);
                    if (entityToDestroy != Entity.Null)
                        spawner.Manager.DestroyEntity(entityToDestroy);

                    break;
                case MouseMode.ToggleFire:
                    var entityToChange = GetEntityWithRaycast(origin, direction);
                    if (entityToChange != Entity.Null)
                    {
                        var flamableData = spawner.Manager.GetComponentData<FlamableData>(entityToChange);

                        flamableData.State++;
                        if ((int)flamableData.State >= flamableStatesCount)
                            flamableData.State = 0;

                        spawner.Manager.SetComponentData(entityToChange, flamableData);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Entity GetEntityWithRaycast(float3 origin, float3 direction)
        {
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

        public MouseMode ToogleMode()
        {
            currentMode++;
            if ((int)currentMode >= mouseModeCount)
                currentMode = 0;

            return currentMode;
        }
    }
}
