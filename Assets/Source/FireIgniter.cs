namespace MarianPekar.FireSpreadingSimulation
{
    using Unity.Collections;
    using Unity.Entities;
    using UnityEngine;

    public class FireIgniter : MonoBehaviour
    {
        [SerializeField] private Spawner spawner;

        [Range(0.0f,1.0f)]
        [SerializeField] private float ignitionFactor = 0.1f;

        private int ignitionCount; 

        private NativeArray<Entity> entities;

        public void IgniteRandomFlammables()
        {
            entities = GlobalData.Instance.EntityManager.CreateEntityQuery(typeof(FlammableData)).ToEntityArray(Allocator.TempJob);
            ignitionCount = Mathf.CeilToInt(entities.Length * ignitionFactor);

            if (entities.Length == 0 || ignitionCount == 0)
            {
                entities.Dispose();
                return;
            }

            if (ignitionCount > entities.Length)
            {
                IgniteAll(entities);
                return;
            }

            for (var i = 0; i < ignitionCount; i++)
                GlobalData.Instance.EntityManager.SetComponentData(entities[Random.Range(0,entities.Length-1)], new FlammableData { State = FlammableState.OnFire });

            entities.Dispose();
        }

        private void IgniteAll(NativeArray<Entity> entities)
        {
            foreach (var entity in entities)
                GlobalData.Instance.EntityManager.SetComponentData(entity, new FlammableData { State = FlammableState.OnFire });

            entities.Dispose();
        }
    }
}
