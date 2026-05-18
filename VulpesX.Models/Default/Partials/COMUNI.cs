namespace VulpesX.Models.Default
{
    public partial class COMUNI
    {
        public string FullDescriptionSearchable => $"{comdes?.Trim()}";
    }
}
