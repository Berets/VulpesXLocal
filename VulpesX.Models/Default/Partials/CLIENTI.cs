namespace VulpesX.Models.Default
{
    public partial class CLIENTI
    {
        public ABE? Basic { get; set; }

        public bool clpaymBool
        {
            get
            {
                return clpaym == "S";
            }
            set
            {
                if (value)
                    clpaym = "S";
                else
                    clpaym = "N";
            }
        }

        public bool CLSOSPBool
        {
            get
            {
                return CLSOSP == "S";
            }
            set
            {
                if (value)
                    CLSOSP = "S";
                else
                    CLSOSP = "N";
            }

        }

        public string? zoncod { get; set; }
        public string? regcod { get; set; }
        public string? smecod { get; set; }
        public decimal? climkup { get; set; }
    }
}
