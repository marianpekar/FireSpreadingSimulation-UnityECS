namespace MarianPekar.FireSpreadingSimulation
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Spawner spawner;

        [SerializeField] private MouseInputManager mouseInputManager;
        [SerializeField] private Text mouseModeButtonText;

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

        public void ToogleMouseMode()
        {
            var currentMode = mouseInputManager.ToogleMode();
            mouseModeButtonText.text = currentMode.ToString();
        }
    }
}
