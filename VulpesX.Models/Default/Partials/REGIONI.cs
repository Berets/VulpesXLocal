namespace VulpesX.Models.Default
{
    public partial class REGIONI
    {
        public string FullDescriptionSearchable => $"{regcod} {regdes?.Trim()}";
    }
}
