using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class tab_articolo_produzione_sorgenti
    {
        public tab_articolo? Product { get; set; }

        #region Risorse/Sorgenti
        private ObservableCollection<tab_produzione_risorsa>? risorseList;
        public ObservableCollection<tab_produzione_risorsa_sorgenti>? AllSources { get; set; }
        public ObservableCollection<tab_produzione_risorsa>? RisorseList
        {
            get { return risorseList; }
            set
            {
                risorseList = value;
                if (!string.IsNullOrWhiteSpace(RisorsaID))
                    SelectedRisorsa = risorseList?.Where(w => w.SocietaID == SocietaID && w.ID == RisorsaID).FirstOrDefault();
                else
                    SelectedRisorsa = null;
                NotifyPropertyChanged("SelectedRisorsa");
                NotifyPropertyChanged("RisorsaDescription");
            }
        }
        private ObservableCollection<tab_produzione_risorsa_sorgenti>? sorgentiList;
        public ObservableCollection<tab_produzione_risorsa_sorgenti>? SorgentiList
        {
            get { return sorgentiList; }
            set
            {
                sorgentiList = value;
                if (!string.IsNullOrWhiteSpace(RisorsaID) && !string.IsNullOrWhiteSpace(SorgenteID))
                    SelectedSorgente = sorgentiList?.Where(w => w.SocietaID == SocietaID && w.RisorsaID == RisorsaID && w.ID == SorgenteID).FirstOrDefault();
                else
                    SelectedSorgente = null;
                NotifyPropertyChanged("SelectedSorgente");
                NotifyPropertyChanged("SorgenteDescription");
            }
        }

        private tab_produzione_risorsa? selectedRisorsa;
        public tab_produzione_risorsa? SelectedRisorsa
        {
            get
            {
                return selectedRisorsa;
            }
            set
            {
                selectedRisorsa = value;
                SelectedSorgente = null;
                if (selectedRisorsa != null && AllSources != null)
                {
                    SorgentiList = new ObservableCollection<tab_produzione_risorsa_sorgenti>(AllSources.Where(w => w.RisorsaID == selectedRisorsa.ID));
                }
                else
                {
                    SorgentiList = null;
                }
                RisorsaDescription = selectedRisorsa?.FullDescriptionSearchable;
                NotifyPropertyChanged("SelectedRisorsa");
                NotifyPropertyChanged("SelectedSorgente");
            }
        }
        public string? RisorsaDescription { get; set; }

        private tab_produzione_risorsa_sorgenti? selectedSorgente;
        public tab_produzione_risorsa_sorgenti? SelectedSorgente
        {
            get
            {
                return selectedSorgente;
            }
            set
            {
                selectedSorgente = value;
                SorgenteDescription = selectedSorgente?.FullDescriptionSearchable;
                NotifyPropertyChanged("SelectedSorgente");
            }
        }
        public string? SorgenteDescription { get; set; }
        #endregion
    }
}
