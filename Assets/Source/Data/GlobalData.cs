namespace MarianPekar.FireSpreadingSimulation
{
    using System;
    using System.Collections.Generic;

    public sealed class GlobalData
    {
        public List<Action<bool>> IsSimulationRunningActions;
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

        public List<Action<float>> WindChangedActions;
        private float windDirection;
        public float WindDirection
        {
            get { return windDirection; }
            set
            {
                windDirection = value;

                foreach (var action in WindChangedActions)
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

