namespace MarianPekar.FireSpreadingSimulation
{
    using Unity.Entities;

    public enum FlamableState
    {
        Healthy, OnFire, Burned
    }

    [GenerateAuthoringComponent]
    public struct FlamableData : IComponentData
    {
        public FlamableState State;
    }
}