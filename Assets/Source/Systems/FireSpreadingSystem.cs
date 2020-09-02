namespace MarianPekar.FireSpreadingSimulation
{
    using System;
    using UnityEngine;
    using Unity.Rendering;
    using Unity.Entities;
    using Unity.Transforms;

    public class FireSpreadingSystem : SystemBase
    {
        private Material burningRed;
        private Material healthyGreen;
        private Material deadBlack;

        private const float FlameSpreadAngle = 45f;
        private const float WindSpeedScaleFactor = 0.2f;

        protected override void OnCreate()
        {
            var standardShader = Shader.Find("Standard");

            healthyGreen = CreateMaterial(Color.green, standardShader);
            burningRed = CreateMaterial(Color.red, standardShader);
            deadBlack = CreateMaterial(Color.black, standardShader);
        }

        private Material CreateMaterial(Color color, Shader shader)
        {
            var material = new Material(shader);
            material.SetColor("_Color", color);
            material.enableInstancing = true;
            return material;
        }

        protected override void OnUpdate()
        {
            var manager = GlobalData.Instance.EntityManager;

            Entities
                .WithoutBurst()
                .WithStructuralChanges()
                .WithAll<FlammableData>().ForEach((Entity entity, Translation translation, RenderMesh renderMesh, ref FlammableData flammableData, ref HealthData healthData, ref FireSpreadingData fireSpreadingData) =>
                {

                    switch (flammableData.State)
                    {
                        case FlammableState.Healthy:
                            ChangeEntityMaterial(ref manager, ref entity, ref renderMesh, healthyGreen);
                            ResetEntityData(ref flammableData, ref healthData, ref fireSpreadingData);
                            break;

                        case FlammableState.OnFire:
                            ChangeEntityMaterial(ref manager, ref entity, ref renderMesh, burningRed);

                            if(!GlobalData.Instance.IsSimulationRunning || GlobalData.Instance.WindSpeed <= 0f || flammableData.State == FlammableState.Burned)
                                break;

                            NextSimulationStep(ref fireSpreadingData, ref translation, ref manager, ref healthData, ref flammableData);
                            break;

                        case FlammableState.Burned:
                            ChangeEntityMaterial(ref manager, ref entity, ref renderMesh, deadBlack);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                }).Run();
        }

        private void NextSimulationStep(ref FireSpreadingData fireSpreadingData, ref Translation translation, ref EntityManager manager,
            ref HealthData healthData, ref FlammableData flammableData)
        {
            fireSpreadingData.Timer -= Time.DeltaTime * GlobalData.Instance.WindSpeed;
            if (fireSpreadingData.Timer <= 0f)
                SpreadFire(ref translation, ref manager, ref fireSpreadingData);

            healthData.Health -= Time.DeltaTime;
            if (healthData.Health <= 0)
                flammableData.State = FlammableState.Burned;
        }

        private void ChangeEntityMaterial(ref EntityManager manager, ref Entity entity, ref RenderMesh renderMesh, Material material)
        {
            renderMesh.material = material;
            manager.SetSharedComponentData(entity, renderMesh);
        }

        private void ResetEntityData(ref FlammableData flammableData, ref HealthData healthData, ref FireSpreadingData fireSpreadingData)
        {
            flammableData.State = FlammableState.Healthy;
            healthData.Health = GlobalData.DefaultHealth;
            fireSpreadingData.Timer = GlobalData.FireSpreadingTimerInitialValue;
        }

        private void SpreadFire(ref Translation translation, ref EntityManager manager, ref FireSpreadingData fireSpreadingData)
        {
            fireSpreadingData.Timer = GlobalData.FireSpreadingTimerInitialValue;

            var (start, end) = GetRay(ref translation);

            Debug.DrawLine(start, end, Color.red, 1f);

            var entityToIgnite = Raycaster.GetEntityWithRaycast(start, end - start);
            if(GlobalData.Instance.EntityManager.Exists(entityToIgnite)) 
                GlobalData.Instance.EntityManager.SetComponentData(entityToIgnite, new FlammableData() {State = FlammableState.OnFire});
        }

        private Tuple<Vector3, Vector3> GetRay(ref Translation translation)
        {
            var randomSpread = UnityEngine.Random.Range(-FlameSpreadAngle, FlameSpreadAngle);
            var maxSpreadingDistance = GlobalData.Instance.WindSpeed * WindSpeedScaleFactor;
            var randomDistance = UnityEngine.Random.Range(1f, maxSpreadingDistance > 1f ? maxSpreadingDistance : 1f);

            var x = randomDistance * Mathf.Cos((GlobalData.Instance.WindDirection + randomSpread) * Mathf.Deg2Rad);
            var y = randomDistance * Mathf.Sin((GlobalData.Instance.WindDirection + randomSpread) * Mathf.Deg2Rad);

            var start = new Vector3(translation.Value.x + x, translation.Value.y + 5f, translation.Value.z + y);
            var end = new Vector3(translation.Value.x + x, translation.Value.y, translation.Value.z + y);
            return new Tuple<Vector3, Vector3>(start, end);
        }
    }
}