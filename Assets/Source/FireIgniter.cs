namespace MarianPekar.FireSpreadingSimulation
{
    using Unity.Collections;
    using Unity.Entities;
    using UnityEngine;

    public class FireIgniter : MonoBehaviour
    {
        [SerializeField] private Spawner spawner;

        [SerializeField] private int ignitionCount;
        private NativeArray<Entity> entities;

        public void IgniteRandomFlamables()
        {
            entities = spawner.Manager.CreateEntityQuery(typeof(FlamableData)).ToEntityArray(Allocator.Persistent);

            if(entities.Length == 0)
                return;

            if (ignitionCount > entities.Length)
            {
                IgniteAll(entities);
                return;
            }

            for (var i = 0; i < ignitionCount; i++)
                spawner.Manager.SetComponentData(entities[Random.Range(0,entities.Length-1)], new FlamableData { State = FlamableState.OnFire });

            entities.Dispose();
        }

        private void IgniteAll(NativeArray<Entity> entities)
        {
            foreach (var entity in entities)
                spawner.Manager.SetComponentData(entity, new FlamableData { State = FlamableState.OnFire });

            entities.Dispose();
        }

        public void OnDestroy()
        {
            entities.Dispose();
        }
    }
}
