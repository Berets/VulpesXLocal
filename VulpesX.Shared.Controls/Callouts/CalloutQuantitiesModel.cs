using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Default;
using VulpesX.Models.Default.Partials;

namespace VulpesX.Shared.Controls.Callouts
{
    public class CalloutQuantitiesModel : Base
    {
        public ObservableCollection<StockInfo>? Stocks { get; set; }

        private ObservableCollection<store_stocks_lots>? lots;
        public ObservableCollection<store_stocks_lots>? Lots { get => lots; set { lots = value; NotifyPropertyChanged("Lots"); } }

        private StockInfo? selectedStock;
        public StockInfo? SelectedStock
        {
            get
            {
                if (selectedStock != null)
                {
                    return selectedStock;
                }
                else
                {
                    var retValue = Stocks?.FirstOrDefault();
                    if (retValue != null) { Lots = retValue.Lots; }
                    return retValue;
                }
            }
            set
            {
                selectedStock = value;

                if (value != null)
                    Lots = value.Lots;
                else
                    Lots = null;

                NotifyPropertyChanged("SelectedStock");
            }
        }
    }
}
