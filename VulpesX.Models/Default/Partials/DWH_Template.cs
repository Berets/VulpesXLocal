using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class DWH_Template
    {
        public bool IsPersonal { get { return !(IsShared ?? false); } }
        public bool IsTemplate { get; set; } = false;

        public bool HaveDescription { get { return !string.IsNullOrEmpty(description); } }

        public ObservableCollection<DWH_TemplateParameter>? Parametri { get; set; }
        public DWH_Query? Query { get; set; }
        public List<DWH_Template>? Childs { get; set; }
    }
}
