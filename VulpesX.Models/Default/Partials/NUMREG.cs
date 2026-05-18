namespace VulpesX.Models.Default
{
    public partial class NUMREG
    {
        public string FullDescriptionSearchable => $"{PERCOD} {PERDE1?.Trim()}";
    }
}
