using System.Collections.ObjectModel;


namespace VulpesX.Models.Default
{
    public partial class tab_articolo_composizione : Base
    {
        public bool IsRoot { get; set; } = false;
        public string? RevisioneIDOld { get; set; }
        public string? ComponenteRevisioneIDOld { get; set; }

        public string? Descrizione { get; set; }

        public bool EReparto { get { return !string.IsNullOrEmpty(RepartoID); } }

        private bool _EEspanso;
        public bool EEspanso
        {
            get { return _EEspanso; }
            set
            {
                _EEspanso = value;
                NotifyPropertyChanged("EEspanso");
            }
        }

        private bool _ESelezionato;
        public bool ESelezionato
        {
            get { return _ESelezionato; }
            set
            {
                _ESelezionato = value;
                NotifyPropertyChanged("ESelezionato");
            }
        }

        public bool EComponente { get; set; }

        public bool ETempoAlPezzo { get; set; }

        public string? RisorsaDescrizione { get; set; }

        public string? UnitaDescrizione { get; set; }

        public tab_articolo_composizione? Padre { get; set; }

        private ObservableCollection<tab_articolo_composizione> _Componenti = new();
        public ObservableCollection<tab_articolo_composizione> Componenti
        {
            get { return _Componenti; }
            set
            {
                if (_Componenti == value)
                    return;

                _Componenti = value;


                _Componenti.CollectionChanged += (s, e) =>
                {
                    NotifyPropertyChanged("HaComposizione");
                };
            }
        }

        public ObservableCollection<tab_produzione_risorsa>? Risorse { get; set; }

        private bool _HaComposizione;
        public bool HaComposizione
        {
            get
            {
                _HaComposizione = (Componenti ?? new ObservableCollection<tab_articolo_composizione>()).Any();

                return _HaComposizione;
            }
            set
            {
                _HaComposizione = value;
                NotifyPropertyChanged("HaComposizione");
            }
        }

        public bool HaDipendenze { get; set; }

        private string? _revisioneNuova;
        public string? RevisioneNuova { get { return _revisioneNuova; } set { _revisioneNuova = value; NotifyPropertyChanged("RevisioneNuova"); } }

        private decimal? _quantitaNuova;
        public decimal? QuantitaNuova { get { return _quantitaNuova; } set { _quantitaNuova = value; NotifyPropertyChanged("QuantitaNuova"); } }

        private ObservableCollection<tab_articolo_composizione>? _dipendenze;
        public ObservableCollection<tab_articolo_composizione>? Dipendenze
        {
            get { return _dipendenze; }
            set
            {
                _dipendenze = value;
                NotifyPropertyChanged("Dipendenze");
            }
        }

        #region Costs info
        public decimal LastCost { get; set; }
        public string LastCostColor => LastCost > 0 ? "G" : "O";
        public decimal AverageCost { get; set; }
        public decimal LastCostPercentage { get; set; }
        public decimal AverageCostPercentage { get; set; }
        public decimal LastValue => LastCost * (Quantita ?? 0);
        public decimal AverageValue => AverageCost * (Quantita ?? 0);
        #endregion

    }
}
