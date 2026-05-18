using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Production
{
    public class ProductionOrderConfirmRawWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ProductionOrderConfirmRawWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required pro_ordine Order { get; set; }

        private ObservableCollection<pro_ordine_composizione>? components;
        public ObservableCollection<pro_ordine_composizione>? Components
        {
            get => components; set
            {
                components = value;
                NotifyPropertyChanged("Components");
                NotifyPropertyChanged("RawsList");
            }
        }

        public ObservableCollection<pro_ordine_composizione> RawsList => new ObservableCollection<pro_ordine_composizione>((Components ?? new ObservableCollection<pro_ordine_composizione>()).Where(w => w.ComponenteArticoloID != null && w.ComponenteRevisioneID == null));

    }
}
