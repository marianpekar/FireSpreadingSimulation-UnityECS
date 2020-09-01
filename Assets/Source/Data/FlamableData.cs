namespace MarianPekar.FireSpreadingSimulation
{
    using Unity.Entities;

    public enum FlammableState
    {
        Healthy, OnFire, Burned
    }

    [GenerateAuthoringComponent]
    public struct FlammableData : IComponentData
    {
        public FlammableState State;
    }
}