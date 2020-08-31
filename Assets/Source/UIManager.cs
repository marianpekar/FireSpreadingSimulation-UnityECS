namespace MarianPekar.FireSpreadingSimulation
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Spawner spawner;

        [SerializeField] private MouseInputManager mouseInputManager;
        [SerializeField] private Text mouseModeButtonText;

        [SerializeField] private Text simulateButtonText;

        [SerializeField] private Slider windDirectionSlider;
        [SerializeField] private Text windDirectionText;

        public void Start()
        {
            GlobalData.Instance.IsSimulationRunningActions.Add(SetSimulationButtonText);
            GlobalData.Instance.WindChangedActions.Add((windDirection) => windDirectionText.text = $"Wind Direction {windDirection}°");
        }

        public void ToggleSimulationState()
        {
            GlobalData.Instance.IsSimulationRunning = !GlobalData.Instance.IsSimulationRunning;
        }

        public void SetWindDirection()
        {
            GlobalData.Instance.WindDirection = windDirectionSlider.value;
        }

        private void SetSimulationButtonText(bool isSimulationRunning)
        {
            if (isSimulationRunning)
                simulateButtonText.text = "Stop";
            else
                simulateButtonText.text = "Simulate";
        }

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
