using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class TAB_ACC_TIPINC
    {
        public FE_PAGDOC? FatturaElettronicaInfo { get; set; }
        public string FullDescriptionSearchable => $"{icscod} {icsdes?.Trim()}";
        public string FullDescriptionNotSearchable => $"[{icscod}] {icsdes?.Trim()}";
        public string? icssupDescription => SupportTypes.Where(w => w.ID == icssup).FirstOrDefault()?.Description;
        public string? icssupDescriptionUfp => SupportTypesUfp.Where(w => w.ID == icssup).FirstOrDefault()?.Description;

        public string? icsicsDescription => ElementTypes.Where(w => w.ID == icsics).FirstOrDefault()?.Description;

        public ObservableCollection<GenericIDDescription> SupportTypes => CommonsService.SupportTypes;
        public ObservableCollection<GenericIDDescription> SupportTypesUfp => CommonsService.SupportTypesUfp;

        public ObservableCollection<GenericIDDescription> ElementTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "I", Description = "Incasso" },
            new GenericIDDescription(){ ID = "S", Description = "Insoluto" },
            new GenericIDDescription(){ ID = "R", Description = "Richiamo" }
        };

        public string? icsods { get; set; }
        public bool icsodsBool
        {
            get
            {
                return icsods == "S";
            }
            set
            {
                if (value)
                    icsods = "S";
                else
                    icsods = "N";
            }
        }

        public string? icspor { get; set; }
        public bool icsporBool
        {
            get
            {
                return icspor == "S";
            }
            set
            {
                if (value)
                    icspor = "S";
                else
                    icspor = "N";
            }
        }
    }
}
