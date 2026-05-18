namespace VulpesX.Models.Default
{
    public partial class TAB_CRM_CAUOFFCLO
    {
        public string FullDescriptionSearchable => $"{id.Trim()} {description?.Trim()}";
    }
}
