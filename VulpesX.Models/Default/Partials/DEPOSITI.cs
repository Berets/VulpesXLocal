namespace VulpesX.Models.Default
{
    public partial class DEPOSITI
    {
        public string FullDescriptionSearchable => $"{depcod} {depdes?.Trim()}";
    }
}
