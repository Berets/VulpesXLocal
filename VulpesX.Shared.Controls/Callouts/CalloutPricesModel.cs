using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.Windows.Controls;
using VulpesX.Models.Models;

namespace VulpesX.Shared.Controls.Callouts
{
    public class CalloutPricesModel : Base
    {
        public CalloutPricesModel()
        {
            this.SelectLastListGeneralPrice = new DelegateCommand(OnSelectLastListGeneralPriceCommandExecuted);
            this.SelectLastListEntityPrice = new DelegateCommand(OnSelectLastListEntityPriceCommandExecuted);
            this.SelectLastOrderPrice = new DelegateCommand(OnSelectLastOrderPriceCommandExecuted);
            this.SelectLastOrderEntityPrice = new DelegateCommand(OnSelectLastOrderEntityPriceCommandExecuted);
            this.SelectLastCostValuationPrice = new DelegateCommand(OnSelectLastCostValuationPriceCommandExecuted);
        }

        public GenericPriceInfo? LastListGeneralPrice { get; set; }
        public GenericPriceInfo? LastListEntityPrice { get; set; }
        public GenericPriceInfo? LastOrderPrice { get; set; }
        public GenericPriceInfo? LastOrderEntityPrice { get; set; }
        public GenericPriceInfo? LastCostValuationPrice { get; set; }
        public GenericPriceInfo? SelectedPrice { get; set; }


        public ICommand SelectLastListGeneralPrice { get; set; }

        private void OnSelectLastListGeneralPriceCommandExecuted(object obj)
        {
            SelectedPrice = LastListGeneralPrice;
            CalloutPopupService.CloseAll();
        }

        public ICommand SelectLastListEntityPrice { get; set; }

        private void OnSelectLastListEntityPriceCommandExecuted(object obj)
        {
            SelectedPrice = LastListEntityPrice;
            CalloutPopupService.CloseAll();
        }

        public ICommand SelectLastOrderPrice { get; set; }

        private void OnSelectLastOrderPriceCommandExecuted(object obj)
        {
            SelectedPrice = LastOrderPrice;
            CalloutPopupService.CloseAll();
        }

        public ICommand SelectLastOrderEntityPrice { get; set; }

        private void OnSelectLastOrderEntityPriceCommandExecuted(object obj)
        {
            SelectedPrice = LastOrderEntityPrice;
            CalloutPopupService.CloseAll();
        }
        public ICommand SelectLastCostValuationPrice { get; set; }

        private void OnSelectLastCostValuationPriceCommandExecuted(object obj)
        {
            SelectedPrice = LastCostValuationPrice;
            CalloutPopupService.CloseAll();
        }
    }

}
