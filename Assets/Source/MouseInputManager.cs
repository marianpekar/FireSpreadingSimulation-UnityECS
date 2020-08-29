using System;

namespace MarianPekar.FireSpreadingSimulation
{
    using UnityEngine;

    public enum MouseMode
    {
        Add, Remove, ToggleFire
    }

    public class MouseInputManager : MonoBehaviour
    {
        [SerializeField] private Spawner spawner;
        [SerializeField] private int spawnableIndex = 0;

        public MouseMode currentMode = MouseMode.Add;

        private Camera camera;
        void Start()
        {
            camera = Camera.main;
        }

        void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;

            switch (currentMode)
            {
                case MouseMode.Add:
                    var ray = camera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out var hit))
                    {
                        spawner.SpawnOne(spawnableIndex, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                    }
                    break;
                case MouseMode.Remove:
                    break;
                case MouseMode.ToggleFire:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
