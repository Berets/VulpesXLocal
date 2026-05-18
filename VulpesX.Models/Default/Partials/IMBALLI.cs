namespace VulpesX.Models.Default
{
    public partial class IMBALLI
    {
        public string FullDescriptionSearchable => $"{imbcod} {imbdes?.Trim()}";
    }
}
