namespace MarianPekar.FireSpreadingSimulation
{
    using UnityEngine;

    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Spawner spawner;

        public void Generate()
        {
            spawner.ClearAll();
            spawner.SetSeed((uint)Random.Range(1,10000),0);
            spawner.SpawnAll();
        }

        public void Clear()
        {
            spawner.ClearAll();
        }
    }
}
