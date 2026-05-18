using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Default;

namespace VulpesX.Models.Ufp
{
    public partial class ANAFAT_ROW
    {
        #region Info
        public string AddedText => added.HasValue ? added.Value.ToString() : "---";
        public string AddedUserText => !string.IsNullOrWhiteSpace(addedUserID) ? addedUserID : "---";
        public string UpdatedText => updated.HasValue ? updated.Value.ToString() : "---";
        public string UpdatedUserText => !string.IsNullOrWhiteSpace(updateUserID) ? updateUserID : "---";
        public string CanceledText => canceled.HasValue ? canceled.Value.ToString() : "---";
        public string CanceledUserText => !string.IsNullOrWhiteSpace(canceledUserID) ? canceledUserID : "---";
        #endregion

        private ObservableCollection<ABE>? customers;
        public ObservableCollection<ABE>? Customers
        {
            get => customers;
            set
            {
                customers = value;
                // state
                if (afcli.HasValue)
                    Customer = customers?.Where(w => w.abecod == afcli).FirstOrDefault();
                else
                    Customer = null;

                NotifyPropertyChanged("Customers");
            }
        }
        
        private ABE? customer;
        public ABE? Customer
        {
            get => customer;
            set
            {
                customer = value;
                NotifyPropertyChanged("Customer");
            }
        }

        private ObservableCollection<tab_articolo>? articles;
        public ObservableCollection<tab_articolo>? Articles
        {
            get => articles;
            set
            {
                articles = value;

                NotifyPropertyChanged("Articles");
            }
        }

        private tab_articolo? article;
        public tab_articolo? Article
        {
            get => article;
            set
            {
                article = value;

                NotifyPropertyChanged("Article");
            }
        }

        private tab_articolo? material;
        public tab_articolo? Material
        {
            get => material;
            set
            {
                material = value;
                NotifyPropertyChanged("Material");
            }
        }

        public string afcomplexitytypeDescription
        {
            get
            {
                switch (afcomplexitytype)
                {
                    case ("S"):
                        return "Standard";
                    case ("M"):
                        return "Media";
                    case ("C"):
                        return "Complessa";
                    default:
                        return string.Empty;
                }
            }
        }
        public string afcomplexitytypeColor
        {
            get
            {
                switch (afcomplexitytype)
                {
                    case ("S"):
                        return "G";
                    case ("M"):
                        return "Y";
                    case ("C"):
                        return "R";
                    default:
                        return string.Empty;
                }
            }
        }

        public string afcustomertypeDescription
        {
            get
            {
                switch (afcustomertype)
                {
                    case ("I"):
                        return "ICO";
                    case ("D"):
                        return "Diretto";
                    default:
                        return string.Empty;
                }
            }
        }
        public string afcustomertypeColor
        {
            get
            {
                switch (afcustomertype)
                {
                    case ("I"):
                        return "B";
                    case ("D"):
                        return "Y";
                    default:
                        return string.Empty;
                }
            }
        }

        private ANAFAT_CONST? constant;
        public ANAFAT_CONST? Constant
        {
            get => constant;
            set
            {
                constant = value;
                NotifyPropertyChanged("Constant");
            }
        }
    }
}
