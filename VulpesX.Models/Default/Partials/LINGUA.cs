namespace VulpesX.Models.Default
{
    public partial class LINGUA
    {
        public string FullDescriptionSearchable => $"{lincod} {lindes?.Trim()}";

    }
}
