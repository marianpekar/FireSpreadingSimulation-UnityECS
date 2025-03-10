﻿namespace MarianPekar.FireSpreadingSimulation
{
    using System;
    using System.Collections.Generic;
    using Unity.Entities;
    using Unity.Transforms;
    using UnityEngine;

    [Serializable]
    public class Spawnable
    {
        public GameObject Prefab;
        public Vector2 Area;
        public float NoiseScale;
        public float NoiseThreshold;
        public uint Seed;
    }

    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Spawnable[] spawnables;

        private GameObjectConversionSettings settings;
        private readonly List<Entity> instances = new List<Entity>();

        public void SetSeed(uint seed, int index)
        {
            spawnables[index].Seed = seed;
        }

        public void Start()
        {
            settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, new BlobAssetStore());
        }

        public void SpawnOne(int index, Vector3 position, Quaternion rotation)
        {
            var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(spawnables[index].Prefab, settings);
            Spawn(entity, position, rotation);
        }

        public void SpawnAll()
        {
            foreach (var spawnable in spawnables)
            {
                var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(spawnable.Prefab, settings);

                for (var k = 0; k < (int)spawnable.Area.x; k++)
                for (var l = 0; l < (int) spawnable.Area.y; l++)
                {
                    if (Mathf.PerlinNoise((k + spawnable.Seed) * spawnable.NoiseScale, (l + spawnable.Seed) * spawnable.NoiseScale) < spawnable.NoiseThreshold) 
                        continue;

                    var raycastOrigin = new Vector3((transform.position.x - spawnable.Area.x * 0.5f) + k, 
                                                       transform.position.y, 
                                                    (transform.position.z - spawnable.Area.y * 0.5f) + l);
                    if (!Physics.Raycast(raycastOrigin, Vector3.down, out var hit, int.MaxValue))
                        continue;

                    Spawn(entity, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                }
            }
        }

        private void Spawn(Entity entity, Vector3 position, Quaternion rotation)
        {
            var instance = GlobalData.Instance.EntityManager.Instantiate(entity);
            GlobalData.Instance.EntityManager.SetComponentData(instance, new Translation { Value = position });
            GlobalData.Instance.EntityManager.SetComponentData(instance, new Rotation { Value = rotation });
            GlobalData.Instance.EntityManager.AddComponentData(instance, new DestroyData { Destroy = false });
            GlobalData.Instance.EntityManager.AddComponentData(instance, new FlammableData { State = FlammableState.Healthy });
            GlobalData.Instance.EntityManager.AddComponentData(instance, new HealthData { Health = GlobalData.DefaultHealth });
            GlobalData.Instance.EntityManager.AddComponentData(instance, new FireSpreadingData { Timer = GlobalData.FireSpreadingTimerInitialValue });

            instances.Add(instance);
        }

        public void ClearAll()
        {
            foreach (var instance in instances)
            {
                if(GlobalData.Instance.EntityManager.Exists(instance))
                    GlobalData.Instance.EntityManager.SetComponentData(instance, new DestroyData { Destroy = true });
            }

            instances.Clear();
        }

        private void OnDestroy()
        {
            settings.BlobAssetStore.Dispose();
        }
    }
}