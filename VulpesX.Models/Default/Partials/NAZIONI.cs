namespace VulpesX.Models.Default
{
    public partial class NAZIONI
    {
        public string FullDescriptionSearchable => $"{nazcod} {nazdes?.Trim()}";

        public bool naztipBool
        {
            get
            {
                return naztip == "S";
            }
            set
            {
                if (value)
                    naztip = "S";
                else
                    naztip = "N";
            }
        }
    }
}
