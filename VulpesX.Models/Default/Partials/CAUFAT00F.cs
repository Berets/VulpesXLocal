using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class CAUFAT00F
    {
        public string FullDescriptionSearchable => $"{fatcod} {fatdes?.Trim()}";

        public string? fattifDescription => DocumentTypes?.Where(w => w.ID == fattif).FirstOrDefault()?.Description;

        public ObservableCollection<GenericIDDescription>? DocumentTypes => CommonsService.InvoiceTypes;

        public bool flgconBool
        {
            get
            {
                return flgcon == "S";
            }
            set
            {
                if (value)
                    flgcon = "S";
                else
                    flgcon = "N";
            }
        }

        public bool fatautBool
        {
            get
            {
                return fataut == "S";
            }
            set
            {
                if (value)
                    fataut = "S";
                else
                    fataut = "N";
            }
        }

        public string? fatITE { get; set; }
        public string? fatCEE { get; set; }
        public string? fatEXTCEE { get; set; }
        public string? fattipo { get; set; }
        public string? fatdoctip { get; set; }

        public string? FATGRUp { get; set; }
        public string? fatcont{ get; set; }
        public string? fatsott { get; set; }
    }
}
