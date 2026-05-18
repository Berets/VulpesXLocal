namespace VulpesX.Models.Default
{
    public partial class FORNITORI
    {
        public ABE? Basic { get; set; }

        public bool FOSOSPBool
        {
            get
            {
                return FOSOSP == "S";
            }
            set
            {
                if (value)
                    FOSOSP = "S";
                else
                    FOSOSP = "N";
            }

        }

        public string? regcod { get; set; }
        public string? imbcod { get; set; }
    }
}
