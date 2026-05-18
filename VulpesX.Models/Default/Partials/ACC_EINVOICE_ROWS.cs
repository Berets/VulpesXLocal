using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class ACC_EINVOICE_ROWS
    {
        public tab_articolo? Product { get; set; }
        public ASSOGGETAMENTI? Rate { get; set; }
        public ObservableCollection<ACC_EINVOICE_ROWS_PIDS>? PIDs { get; set; }
        public ObservableCollection<ACC_EINVOICE_ROWS_SM>? SMs { get; set; }
        public FE_IVADOC? Nature { get; set; }
    }
}
