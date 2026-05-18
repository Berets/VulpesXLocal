using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class TAB_ACC_TIPPAG
    {
        public string FullDescriptionSearchable => $"{inccod} {incdes?.Trim()}";
        public string FullDescriptionNotSearchable => $"[{inccod}] {incdes?.Trim()}";
        public string? incsupDescription => SupportTypes.Where(w => w.ID == incsup).FirstOrDefault()?.Description;
        public bool incodsBool
        {
            get
            {
                return incods == "S";
            }
            set
            {
                if (value)
                    incods = "S";
                else
                    incods = "N";
            }
        }
        public bool incporBool
        {
            get
            {
                return incpor == "S";
            }
            set
            {
                if (value)
                    incpor = "S";
                else
                    incpor = "N";
            }
        }

        public ObservableCollection<GenericIDDescription> SupportTypes => CommonsService.SupportTypes;
    }
}
