namespace VulpesX.Models.Default
{
    public partial class SOCIETA
    {
        public string FullDescriptionSearchable => $"{soctip} {socdes?.Trim()}";
    }
}
