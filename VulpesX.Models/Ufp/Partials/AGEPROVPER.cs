using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Default;

namespace VulpesX.Models.Ufp
{
    public partial class AGEPROVPER
    {
        public bool IsInsert { get; set; }

        private ObservableCollection<ABE>? _Customers;
        public ObservableCollection<ABE>? Customers
        {
            get { return _Customers; }
            set
            {
                _Customers = value;

                if (appclie > 0)
                    SelectedCustomer = _Customers?.Where(w => w.abecod == appclie).FirstOrDefault();
                else
                    SelectedCustomer = null;

                NotifyPropertyChanged("Customers");
                NotifyPropertyChanged("CustomerDescription");
            }
        }

        private ABE? selectedCustomer;
        public ABE? SelectedCustomer
        {
            get
            {
                return selectedCustomer;
            }
            set
            {
                selectedCustomer = value;
                CustomerDescription = selectedCustomer?.FullDescriptionSearchable;
                NotifyPropertyChanged();
            }
        }

        public string? CustomerDescription { get; set; }

        private ObservableCollection<ABE>? _Suppliers;
        public ObservableCollection<ABE>? Suppliers
        {
            get { return _Suppliers; }
            set
            {
                _Suppliers = value;

                if (appfor > 0)
                    SelectedSupplier = _Suppliers?.Where(w => w.abecod == appfor).FirstOrDefault();
                else
                    SelectedSupplier = null;

                NotifyPropertyChanged("Suppliers");
                NotifyPropertyChanged("SupplierDescription");
            }
        }

        private ABE? selectedSupplier;
        public ABE? SelectedSupplier
        {
            get
            {
                return selectedSupplier;
            }
            set
            {
                selectedSupplier = value;
                SupplierDescription = selectedSupplier?.FullDescriptionSearchable;
                NotifyPropertyChanged();
            }
        }

        public string? SupplierDescription { get; set; }

        private ObservableCollection<TAB_CRM_CAUORD>? _orderCausals;
        public ObservableCollection<TAB_CRM_CAUORD>? OrderCausals
        {
            get { return _orderCausals; }
            set
            {
                _orderCausals = value;

                if (!string.IsNullOrEmpty(appcau))
                    SelectedOrderCausal = _orderCausals?.Where(w => w.cauacq == appcau).FirstOrDefault();
                else
                    SelectedOrderCausal = null;

                NotifyPropertyChanged("OrderCausals");
                NotifyPropertyChanged("OrderCausalDescription");
            }
        }

        private TAB_CRM_CAUORD? selectedOrderCausal;
        public TAB_CRM_CAUORD? SelectedOrderCausal
        {
            get
            {
                return selectedOrderCausal;
            }
            set
            {
                selectedOrderCausal = value;
                OrderCausalDescription = selectedOrderCausal?.FullDescriptionSearchable;
                NotifyPropertyChanged();
            }
        }

        public string? OrderCausalDescription { get; set; }
    }
}
