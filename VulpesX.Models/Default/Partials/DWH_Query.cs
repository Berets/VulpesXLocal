using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class DWH_Query
    {
        public ObservableCollection<DWH_QueryParameter>? Parametri { get; set; }

        public bool HaveDescription { get { return !string.IsNullOrEmpty(description); } }
    }
}
