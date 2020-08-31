namespace MarianPekar.FireSpreadingSimulation
{
    using System;
    using UnityEngine;
    using Unity.Rendering;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;

    public class ChangeMaterialSystem : SystemBase
    {
        private Material burningRed;
        private Material healthyGreen;
        private Material deadBlack;
        private Shader shader;

        protected override void OnCreate()
        {
            shader = Shader.Find("Standard");

            healthyGreen = new Material(shader);
            healthyGreen.SetColor("_Color", Color.green);
            healthyGreen.enableInstancing = true;

            burningRed = new Material(shader);
            burningRed.SetColor("_Color", Color.red);
            burningRed.enableInstancing = true;

            deadBlack = new Material(shader);
            deadBlack.SetColor("_Color", Color.black);
            deadBlack.enableInstancing = true;
        }

        protected override void OnUpdate()
        {
            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;

            Entities
                .WithoutBurst()
                .WithStructuralChanges()
                .WithAll<FlamableData>().ForEach((Entity entity, Translation translation, RenderMesh renderMesh, ref FlamableData flamableData, ref LifetimeData lifetimeData, ref FireSpreadingData fireSpreadingData) =>
                {

                    switch (flamableData.State)
                    {
                        case FlamableState.Healthy:
                            renderMesh.material = healthyGreen;
                            manager.SetSharedComponentData(entity, renderMesh);
                            flamableData.State = FlamableState.Healthy;
                            lifetimeData.LifeTime = GlobalData.DefaultLifeTime;
                            fireSpreadingData.Timer = GlobalData.FireSpreadingTimerInitialValue;

                            break;
                        case FlamableState.OnFire:
                            renderMesh.material = burningRed;
                            manager.SetSharedComponentData(entity, renderMesh);

                            if(!GlobalData.Instance.IsSimulationRunning)
                                break;

                            fireSpreadingData.Timer -= Time.DeltaTime * GlobalData.Instance.WindSpeed;
                            if (fireSpreadingData.Timer <= 0f)
                            {
                                fireSpreadingData.Timer = GlobalData.FireSpreadingTimerInitialValue;
                                // TODO: Raycast from here to set nearest entities to fire
                            }

                            lifetimeData.LifeTime -= Time.DeltaTime;
                            if (lifetimeData.LifeTime <= 0)
                            {
                                flamableData.State = FlamableState.Burned;
                            }

                            break;
                        case FlamableState.Burned:
                            renderMesh.material = deadBlack;
                            manager.SetSharedComponentData(entity, renderMesh);

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                }).Run();
        }

    }
}