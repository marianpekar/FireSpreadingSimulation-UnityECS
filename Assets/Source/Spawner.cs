using System.Collections.Generic;

namespace MarianPekar.FireSpreadingSimulation
{
    using System;
    using Unity.Entities;
    using Unity.Transforms;
    using UnityEngine;

    [Serializable]
    public class Spawnable
    {
        public GameObject Prefab;
        public Vector2 Area;
        public Vector3 Offset;
        public float NoiseScale;
        public float NoiseThreshold;
        public uint Seed;
    }

    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Spawnable[] spawnables;

        private EntityManager manager;
        private GameObjectConversionSettings settings;
        private readonly List<Entity> instances = new List<Entity>();

        public void SetSeed(uint seed, int spawnableId)
        {
            spawnables[spawnableId].Seed = seed;
        }

        public void Start()
        {
            manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, new BlobAssetStore());
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

                    var instance = manager.Instantiate(entity);
                    manager.SetComponentData(instance, new Translation { Value = hit.point + spawnable.Offset });
                    manager.SetComponentData(instance, new Rotation { Value = Quaternion.FromToRotation(Vector3.up, hit.normal) });
                    manager.AddComponentData(instance, new DestroyData { Destroy = false });

                    instances.Add(instance);
                }
            }
        }

        public void ClearAll()
        {
            foreach (var instance in instances)
                manager.SetComponentData(instance, new DestroyData { Destroy = true } );

            instances.Clear();
        }

        private void OnDestroy()
        {
            settings.BlobAssetStore.Dispose();
        }
    }
}