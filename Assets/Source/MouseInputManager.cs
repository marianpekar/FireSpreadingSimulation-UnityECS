﻿namespace MarianPekar.FireSpreadingSimulation
{
    using System;
    using Unity.Collections;
    using Unity.Jobs;
    using UnityEngine;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Physics.Systems;

    public enum MouseMode
    {
        Add, Remove, ToggleFire
    }

    public class MouseInputManager : MonoBehaviour
    {
        [SerializeField] private Spawner spawner;
        [SerializeField] private int spawnableIndex = 0;

        public MouseMode currentMode = MouseMode.Add;

        private int mouseModeCount;
        private int flamableStatesCount;
        private const float raycastDistance = 4000f;

        private Camera camera;

        void Start()
        {
            mouseModeCount = Enum.GetNames(typeof(MouseMode)).Length;
            flamableStatesCount = Enum.GetNames(typeof(FlammableState)).Length;

            camera = Camera.main;
        }

        void OnDestroy()
        {
            if (Raycaster.raycastHits.IsCreated)
                Raycaster.raycastHits.Dispose();
        }

        void LateUpdate()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            float3 origin = ray.origin;
            float3 direction = ray.direction * raycastDistance;

            switch (currentMode)
            {
                case MouseMode.Add:
                    if (Physics.Raycast(ray, out var hit))
                        spawner.SpawnOne(spawnableIndex, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                    
                    break;
                case MouseMode.Remove:
                    var entityToDestroy = Raycaster.GetEntityWithRaycast(origin, direction);
                    if (spawner.Manager.Exists(entityToDestroy))
                        spawner.Manager.DestroyEntity(entityToDestroy);

                    break;
                case MouseMode.ToggleFire:
                    var entityToChange = Raycaster.GetEntityWithRaycast(origin, direction);
                    if (spawner.Manager.Exists(entityToChange))
                    {
                        var flamableData = spawner.Manager.GetComponentData<FlammableData>(entityToChange);

                        flamableData.State++;
                        if ((int)flamableData.State >= flamableStatesCount)
                            flamableData.State = 0;

                        spawner.Manager.SetComponentData(entityToChange, flamableData);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public MouseMode ToggleMode()
        {
            currentMode++;
            if ((int)currentMode >= mouseModeCount)
                currentMode = 0;

            return currentMode;
        }
    }
}
