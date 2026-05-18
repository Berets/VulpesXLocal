namespace VulpesX.Models.Default
{
    public partial class FATTPERSTXT
    {
        public string FullDescriptionSearchable => $"{txtid} {txtdes?.Trim()}";
    }
}
