using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.General;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.General
{
    public class ARTCompositionReplaceWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ARTCompositionReplaceWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyPropertyChanged("IsBusy");
            }
        }

        public ObservableCollection<tab_articolo>? Products { get; set; }

        private tab_articolo? selectedProduct;
        public tab_articolo? SelectedProduct
        {
            get { return selectedProduct; }
            set
            {
                selectedProduct = value;
                NotifyPropertyChanged("SelectedProduct");
            }
        }

        private ObservableCollection<string>? _revisions;
        public ObservableCollection<string>? Revisions { get { return _revisions; } set { _revisions = value; NotifyPropertyChanged("Revisions"); } }

        private string? selectedRevision;
        public string? SelectedRevision
        {
            get { return selectedRevision; }
            set { selectedRevision = value; NotifyPropertyChanged("SelectedRevision"); }
        }

        private ObservableCollection<tab_articolo_composizione>? _dependencies;
        public ObservableCollection<tab_articolo_composizione>? Dependencies { get { return _dependencies; } set { _dependencies = value; NotifyPropertyChanged("Dependencies"); } }

        public ObservableCollection<tab_articolo>? ProductsNew { get; set; }

        private tab_articolo? selectedProductNew;
        public tab_articolo? SelectedProductNew
        {
            get { return selectedProductNew; }
            set
            {
                selectedProductNew = value;
                NotifyPropertyChanged("SelectedProductNew");
            }
        }

        private ObservableCollection<string>? _revisionsNew;
        public ObservableCollection<string>? RevisionsNew { get { return _revisionsNew; } set { _revisionsNew = value; NotifyPropertyChanged("RevisionsNew"); } }

        private string? selectedRevisionNew;
        public string? SelectedRevisionNew
        {
            get { return selectedRevisionNew; }
            set { selectedRevisionNew = value; NotifyPropertyChanged("SelectedRevisionNew"); }
        }

        public void LoadProducts()
        {
            var products = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSimpleList(CompanyID);

            Products = products;
            ProductsNew = products;
        }

        public ObservableCollection<string>? GetRevisions(string ProductID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().GetRevisioniSimpleList(CompanyID, ProductID);
        }

        public bool ExistRevision(string ProductID, string RevisionID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().ExistRevision(CompanyID, ProductID, RevisionID);
        }

        public ObservableCollection<tab_articolo_composizione>? GetDependencies(string ProductID, string RevisionID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().GetDipendenzeLevel(CompanyID, ProductID, RevisionID, null);
        }

        public ObservableCollection<tab_articolo_composizione>? GetDependencies(string ProductID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().GetDipendenzeLevel(CompanyID, ProductID, null);
        }

        public bool Exchange(List<tab_articolo_composizione> Roots)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().Exchange(CompanyID, Roots, SelectedProduct!.ID, SelectedRevision!, SelectedProductNew!.ID, SelectedRevisionNew!);

        }
    }
}
