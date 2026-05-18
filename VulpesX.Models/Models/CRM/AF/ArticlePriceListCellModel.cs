using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Default;

namespace VulpesX.Models.Models.CRM.AF
{
    public class ArticlePriceListCellModel : Base
    {
        public required string Key { get; set; }
        public required tab_articolo Article { get; set; }
        public decimal? Price { get; set; }
        public int SupplierID { get; set; }
        public DateTime? Date { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }

        public required string Color { get; set; }
    }
}
