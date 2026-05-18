namespace VulpesX.Models.Default
{
    public partial class ISO
    {
        public string FullDescriptionSearchable => $"{isocod} {isodes?.Trim()}";
        public string FullDescriptionNotSearchable => $"[{isocod}] {isodes?.Trim()}";

        public bool isointrBool
        {
            get
            {
                return isointr == "S";
            }
            set
            {
                if (value)
                    isointr = "S";
                else
                    isointr = "N";
            }
        }

        public string? isolinDescription { get; set; }
    }
}
