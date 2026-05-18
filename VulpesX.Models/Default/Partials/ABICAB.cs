namespace VulpesX.Models.Default
{
    public partial class ABICAB
    {
        public string FullDescriptionSearchable => $"{abiabi} {abicab} {abiban?.Trim()} {abiage?.Trim()}";
    }
}
