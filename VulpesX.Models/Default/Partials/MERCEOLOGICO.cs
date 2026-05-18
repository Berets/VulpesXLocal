namespace VulpesX.Models.Default
{
    public partial class MERCEOLOGICO
    {
        public string FullDescriptionSearchable => $"{smecod} {smedes?.Trim()}";
        public string? smerie { get; set; }

        public bool smerieBool
        {
            get
            {
                return smerie == "S";
            }
            set
            {
                if (value)
                    smerie = "S";
                else
                    smerie = "N";
            }
        }
    }
}
