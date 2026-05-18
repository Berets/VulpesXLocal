namespace VulpesX.Models.Default
{
    public partial class RIVENDITORI
    {
        public string FullDescriptionSearchable => $"{rivcod} {rivdes?.Trim()}";
    }
}
