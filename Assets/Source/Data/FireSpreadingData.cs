namespace MarianPekar.FireSpreadingSimulation
{
    using Unity.Entities;

    [GenerateAuthoringComponent]
    public struct FireSpreadingData : IComponentData
    {
        public float Timer;
    }
}