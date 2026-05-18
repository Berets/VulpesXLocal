namespace VulpesX.Models.Default
{
    public partial class CATEGORIA
    {
        public string FullDescriptionSearchable => $"{catcod} {catdes?.Trim()}";
    }
}
