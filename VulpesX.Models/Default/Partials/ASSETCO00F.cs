using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class ASSETCO00F
    {
        public TAB_GEN_CONTACTS_ROLES? Role { get; set; }
        public ObservableCollection<TAB_GEN_CONTACTS_ROLES>? ContactRoles { get; set; }
        public ObservableCollection<ASSETCODET00F>? ContactDetails { get; set; }
    }
}
