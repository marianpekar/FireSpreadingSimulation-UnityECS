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
            get => isSimulationRunning;
            set => isSimulationRunning = NotifySubscribersAndReturnValue(value, IsSimulationRunningActions);
        }

        public List<Action<float>> WindDirectionChangedActions = new List<Action<float>>();
        private float windDirection;
        public float WindDirection
        {
            get => windDirection;
            set => windDirection = NotifySubscribersAndReturnValue(value, WindDirectionChangedActions);
        }

        public List<Action<float>> WindSpeedChangedActions = new List<Action<float>>();
        private float windSpeed;
        public float WindSpeed
        {
            get => windSpeed;
            set => windSpeed = NotifySubscribersAndReturnValue(value, WindSpeedChangedActions);
        }

        private T NotifySubscribersAndReturnValue<T>(T value, List<Action<T>> subscribers)
        {
            foreach (var subscriber in subscribers)
                subscriber.Invoke(value);

            return value;
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

