using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Default;

namespace VulpesX.Models.Ufp
{
    public partial class AGENTI_SOTTOLIVELLO
    {
        public bool IsInsert { get; set; }

        private ObservableCollection<ABE>? _Customers;
        public ObservableCollection<ABE>? Customers
        {
            get { return _Customers; }
            set
            {
                _Customers = value;

                if (agecli > 0)
                    SelectedCustomer = _Customers?.Where(w => w.abecod == agecli).FirstOrDefault();
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

        private ObservableCollection<tab_articolo>? _articles;
        public ObservableCollection<tab_articolo>? Articles
        {
            get { return _articles; }
            set
            {
                _articles = value;

                if (!string.IsNullOrEmpty(ageart))
                    SelectedArticle = _articles?.Where(w => w.ID == ageart).FirstOrDefault();
                else
                    SelectedArticle = null;

                NotifyPropertyChanged("Articles");
                NotifyPropertyChanged("ArticleDescription");
            }
        }

        private tab_articolo? selectedArticle;
        public tab_articolo? SelectedArticle
        {
            get
            {
                return selectedArticle;
            }
            set
            {
                selectedArticle = value;
                ArticleDescription = selectedArticle?.FullDescriptionSearchableUfp;
                NotifyPropertyChanged();
            }
        }

        public string? ArticleDescription { get; set; }
    }
}
