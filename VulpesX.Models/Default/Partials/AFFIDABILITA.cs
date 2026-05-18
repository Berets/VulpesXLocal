namespace VulpesX.Models.Default
{
    public partial class AFFIDABILITA
    {
        public string FullDescriptionSearchable => $"{affcod} {affdes?.Trim()}";

        public bool affordBool
        {
            get
            {
                return afford == "S";
            }
            set
            {
                if (value)
                    afford = "S";
                else
                    afford = "N";
            }
        }

        public bool afffatBool
        {
            get
            {
                return afffat == "S";
            }
            set
            {
                if (value)
                    afffat = "S";
                else
                    afffat = "N";
            }
        }

        public bool affribBool
        {
            get
            {
                return affrib == "S";
            }
            set
            {
                if (value)
                    affrib = "S";
                else
                    affrib = "N";
            }
        }
    }
}
