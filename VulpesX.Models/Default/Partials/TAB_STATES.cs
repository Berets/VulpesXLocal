namespace VulpesX.Models.Default
{
    public partial class TAB_STATES
    {
        public string FullDescriptionSearchable => $"{cappro} {capdpr?.Trim()}";
    }
}
