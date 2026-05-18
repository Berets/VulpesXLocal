using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Default
{
    public partial class TAB_AGENTI_ENASARCO : Base
    {
        public bool IsInsert { get; set; } = false;
        public int? SupplierID { get; set; }
        public decimal? Used { get; set; }

        private ObservableCollection<ACC_EINVOICE_HEADS>? items;
        public ObservableCollection<ACC_EINVOICE_HEADS>? Items
        {
            get { return items; }
            set
            {
                items = value;

                NotifyPropertyChanged("Items");
            }
        }
    }
}
