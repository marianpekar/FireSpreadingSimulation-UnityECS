namespace MarianPekar.FireSpreadingSimulation
{
    using System;
    using UnityEngine;
    using Unity.Rendering;
    using Unity.Jobs;
    using Unity.Entities;

    public class FireSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            
            Entities
                .WithAll<FlamableData>().ForEach((Entity entity, int entityInQueryIndex, ref FlamableData flamableData) =>
            {
                if(entity == Entity.Null)
                    return;

                switch (flamableData.State)
                {
                    case FlamableState.Healthy:

                        break;
                    case FlamableState.OnFire:
                        
                        break;
                    case FlamableState.Burned:
    
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }).Schedule();
        }

    }
}