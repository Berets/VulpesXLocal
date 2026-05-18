 

namespace VulpesX.Models.Default
{
    public partial class NOTEFOR : Base
    {
        public string? nofsoc { get; set; }
        public bool Updated { set { NotifyPropertyChanged("Nofnot"); } }
    }
}
