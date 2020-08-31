namespace MarianPekar.FireSpreadingSimulation
{
    using System;
    using UnityEngine;
    using Unity.Rendering;
    using Unity.Entities;

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
                .WithAll<FlamableData>().ForEach((Entity entity, RenderMesh renderMesh, ref FlamableData flamableData, ref LifetimeData lifetimeData) =>
                {

                    switch (flamableData.State)
                    {
                        case FlamableState.Healthy:
                            renderMesh.material = healthyGreen;
                            manager.SetSharedComponentData(entity, renderMesh);
                            manager.SetComponentData(entity, new FlamableData { State = FlamableState.Healthy });
                            manager.SetComponentData(entity, new LifetimeData { LifeTime = 5f });

                            break;
                        case FlamableState.OnFire:
                            renderMesh.material = burningRed;
                            manager.SetSharedComponentData(entity, renderMesh);

                            lifetimeData.LifeTime -= Time.DeltaTime;
                            if (lifetimeData.LifeTime <= 0)
                            {
                                manager.SetComponentData(entity, new FlamableData
                                {
                                    State = FlamableState.Burned
                                });
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