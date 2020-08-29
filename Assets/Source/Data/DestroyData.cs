namespace MarianPekar.FireSpreadingSimulation
{
    using Unity.Entities;

    [GenerateAuthoringComponent]
    public struct DestroyData : IComponentData
    {
        public bool Destroy;
    }
}