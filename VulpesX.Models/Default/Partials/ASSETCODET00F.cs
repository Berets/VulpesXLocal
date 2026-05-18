using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class ASSETCODET00F
    {
        public TAB_GEN_CONTACTS_TYPES? ContactType { get; set; }
        public ObservableCollection<TAB_GEN_CONTACTS_TYPES>? ContactTypes { get; set; }
    }
}
