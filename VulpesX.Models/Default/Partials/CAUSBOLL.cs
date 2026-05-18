namespace VulpesX.Models.Default
{
    public partial class CAUSBOLL
    {
        public string FullDescriptionSearchable => $"{bolcod} {boldes?.Trim()}";

        public bool bolcliBool
        {
            get
            {
                return bolcli == "S";
            }
            set
            {
                if (value)
                    bolcli = "S";
                else
                    bolcli = "N";
            }
        }

        public bool bolforBool
        {
            get
            {
                return bolfor == "S";
            }
            set
            {
                if (value)
                    bolfor = "S";
                else
                    bolfor = "N";
            }
        }

        public bool bolmagBool
        {
            get
            {
                return bolmag == "S";
            }
            set
            {
                if (value)
                    bolmag = "S";
                else
                    bolmag = "N";
            }
        }

        public bool bolfatBool
        {
            get
            {
                return bolfat == "S";
            }
            set
            {
                if (value)
                    bolfat = "S";
                else
                    bolfat = "N";
            }
        }
    }
}
