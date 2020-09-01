namespace MarianPekar.FireSpreadingSimulation
{
    using System;
    using System.Collections.Generic;

    public sealed class GlobalData
    {
        public const float FireSpreadingTimerInitialValue = 10f;
        public const float DefaultHealth = 20f;

        public List<Action<bool>> IsSimulationRunningActions = new List<Action<bool>>();
        private bool isSimulationRunning = false;
        public bool IsSimulationRunning
        {
            get { return isSimulationRunning; }
            set
            {
                isSimulationRunning = value;

                foreach (var action in IsSimulationRunningActions)
                    action.Invoke(value);
            }
        }

        public List<Action<float>> WindDirectionChangedActions = new List<Action<float>>();
        private float windDirection;
        public float WindDirection
        {
            get { return windDirection; }
            set
            {
                windDirection = value;

                foreach (var action in WindDirectionChangedActions)
                    action.Invoke(value);
            }
        }

        public List<Action<float>> WindSpeedChangedActions = new List<Action<float>>();
        private float windSpeed;
        public float WindSpeed
        {
            get { return windSpeed; }
            set
            {
                windSpeed = value;

                foreach (var action in WindSpeedChangedActions)
                    action.Invoke(value);
            }
        }

        private static GlobalData instance;
        public static GlobalData Instance
        {
            get
            {
                if (instance == null)
                    instance = new GlobalData();

                return instance;
            }
        }
    }
}

