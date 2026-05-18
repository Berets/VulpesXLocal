namespace VulpesX.Models.Default
{
    public partial class CLAZIONE
    {
        public string FullDescriptionSearchable => $"{csfcod} {csfdes?.Trim()}";
    }
}
