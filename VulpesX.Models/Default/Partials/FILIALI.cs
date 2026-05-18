namespace VulpesX.Models.Default
{
    public partial class FILIALI
    {
        public string FullDescriptionSearchable => $"{filcod} {fildes?.Trim()}";
        public string? filbase { get; set; }

        public bool filbaseBool
        {
            get
            {
                return filbase == "S";
            }
            set
            {
                if (value)
                    filbase = "S";
                else
                    filbase = "N";
            }
        }
    }
}
