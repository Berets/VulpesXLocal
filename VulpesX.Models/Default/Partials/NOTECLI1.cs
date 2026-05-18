

namespace VulpesX.Models.Default
{
    public partial class NOTECLI1 : Base
    {
        public bool Updated { set { NotifyPropertyChanged("NOTETI"); NotifyPropertyChanged("notdes"); } }

        public string? nosoc { get; set; }
        public string? NOTTIP { get; set; } = "N";
        public string? NOTDIS { get; set; }
        public string? NOTRIF { get; set; }
        public string? NOTTESTO { get; set; }
        public string? NOTLOGO { get; set; }
        public string? NOTSUPP { get; set; }

        public bool NOTDISBool
        {
            get
            {
                return NOTDIS == "S";
            }
            set
            {
                if (value)
                    NOTDIS = "S";
                else
                    NOTDIS = "N";
            }

        }
        public bool NOTRIFBool
        {
            get
            {
                return NOTRIF == "S";
            }
            set
            {
                if (value)
                    NOTRIF = "S";
                else
                    NOTRIF = "N";
            }

        }

        public bool MarkDetailVisibility { get { return NOTETI == "M" || NOTETI == "C"; } }
    }
}
