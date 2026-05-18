using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Default;

namespace VulpesX.Models.Ufp
{
    public partial class VETTORISPESE1
    {
        public bool IsInsert { get; set; }

        private ObservableCollection<ISO>? _ISOs;
        public ObservableCollection<ISO>? ISOs
        {
            get { return _ISOs; }
            set
            {
                _ISOs = value;

                if (!string.IsNullOrWhiteSpace(vetiso))
                    SelectedISO = _ISOs?.Where(w => w.isocod == vetiso).FirstOrDefault();
                else
                    SelectedISO = null;

                NotifyPropertyChanged("ISOs");
                NotifyPropertyChanged("ISODescription");
            }
        }

        private ISO? selectedISO;
        public ISO? SelectedISO
        {
            get
            {
                return selectedISO;
            }
            set
            {
                selectedISO = value;
                ISODescription = selectedISO?.FullDescriptionSearchable;
                NotifyPropertyChanged();
            }
        }

        public string? ISODescription { get; set; }
    }
}
