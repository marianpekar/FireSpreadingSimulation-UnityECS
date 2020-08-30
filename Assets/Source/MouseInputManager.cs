using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

namespace MarianPekar.FireSpreadingSimulation
{
    using System;
    using Unity.Collections;
    using Unity.Jobs;
    using UnityEngine;

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

            raycastHits.Clear();

            switch (currentMode)
            {
                case MouseMode.Add:
                    if (Physics.Raycast(ray, out var hit))
                    {
                        spawner.SpawnOne(spawnableIndex, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                    }
                    break;
                case MouseMode.Remove:
                    float3 origin = ray.origin;
                    float3 direction = ray.direction * float.MaxValue;

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
                        if (raycastHit.RigidBodyIndex <= 0) 
                            continue;

                        var entity = buildPhysicsWorld.PhysicsWorld.Bodies[raycastHit.RigidBodyIndex].Entity;
                        if (entity != Entity.Null)
                            spawner.Manager.DestroyEntity(entity);
                    }

                    break;
                case MouseMode.ToggleFire:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
