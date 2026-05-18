namespace VulpesX.Models.Default
{
    public partial class FE_RFIDOC
    {
        public string FullDescriptionSearchable => $"{regicod} {regides}";
        public string FullDescriptionNotSearchable => $"[{regicod}] {regides}";
    }
}
