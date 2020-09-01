namespace MarianPekar.FireSpreadingSimulation
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Spawner spawner;

        [SerializeField] private MouseInputManager mouseInputManager;
        [SerializeField] private Text mouseModeButtonText;

        [SerializeField] private Image simulateButtonImage;
        [SerializeField] private Text simulateButtonText;

        [SerializeField] private Slider windDirectionSlider;
        [SerializeField] private Text windDirectionText;

        [SerializeField] private Slider windSpeedSlider;
        [SerializeField] private Text windSpeedText;

        public void Start()
        {
            GlobalData.Instance.IsSimulationRunningActions.Add(SetSimulationButtonText);
            GlobalData.Instance.WindDirectionChangedActions.Add((windDirection) => windDirectionText.text = $"Wind Direction {windDirection}°");
            GlobalData.Instance.WindSpeedChangedActions.Add((windSpeed) => windSpeedText.text = $"Wind Speed {windSpeed} km/h");

            SetWindDirection();
            SetWindSpeed();
        }

        public void ToggleSimulationState()
        {
            GlobalData.Instance.IsSimulationRunning = !GlobalData.Instance.IsSimulationRunning;
        }

        public void SetWindDirection()
        {
            GlobalData.Instance.WindDirection = windDirectionSlider.value;
        }

        public void SetWindSpeed()
        {
            GlobalData.Instance.WindSpeed = windSpeedSlider.value;
        }

        private void SetSimulationButtonText(bool isSimulationRunning)
        {
            if (isSimulationRunning)
            {
                simulateButtonText.text = "Stop";
                simulateButtonImage.color = Color.red;
            }
            else
            {
                simulateButtonText.text = "Simulate";
                simulateButtonImage.color = Color.white;
            }
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

        public void ToggleMouseMode()
        {
            var currentMode = mouseInputManager.ToggleMode();
            mouseModeButtonText.text = currentMode.ToString();
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
