namespace VulpesX.Models.Default
{
    public partial class ZONE
    {
        public string FullDescriptionSearchable => $"{zoncod} {zondes?.Trim()}";
    }
}
