using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class AUTH_ACCESS_ROLES
    {
        public ObservableCollection<GenericIDDescription>? CrmRoles => CommonsService.ActivityTypes;
        public ObservableCollection<AGENTI>? Agents { get; set; }
    }
}
