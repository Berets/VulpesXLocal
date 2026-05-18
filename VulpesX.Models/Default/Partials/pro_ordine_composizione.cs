
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using VulpesX.Models.Default.Partials;
using VulpesX.Models.Models.Production;

namespace VulpesX.Models.Default
{
    public partial class pro_ordine_composizione : Base
    {
        [JsonIgnore]
        public pro_ordine_composizione? Padre { get; set; }

        public string? ComponenteArticoloDescrizione { get; set; }

        private decimal quantitaImpegnata;
        public decimal QuantitaImpegnata { get => quantitaImpegnata; set { quantitaImpegnata = value; NotifyPropertyChanged("QuantitaImpegnata"); } }
        private decimal quantitaGiaImpegnata;
        public decimal QuantitaGiaImpegnata { get => quantitaGiaImpegnata; set { quantitaGiaImpegnata = value; NotifyPropertyChanged("QuantitaGiaImpegnata"); } }
        private decimal quantitaModificataDaImpegni;
        public decimal QuantitaModificataDaImpegni { get => quantitaModificataDaImpegni; set { quantitaModificataDaImpegni = value; NotifyPropertyChanged("QuantitaModificataDaImpegni"); } }
        private decimal quantitaToDo;
        public decimal QuantitaToDo
        {
            get => quantitaToDo;
            set
            {
                quantitaToDo = value;
                NotifyPropertyChanged("QuantitaToDo");
            }
        }
        public string? ResourceID { get; set; }
        public bool IsSummarize { get; set; }
        public bool ProductIsInfinite { get; set; }
        public bool IsUpdated { get; set; }
        public bool EReparto { get { return !string.IsNullOrEmpty(RepartoID); } }
        public bool EComponente { get; set; }
        public bool IsElaborated { get; set; }
        public DateTime? DataConsegna { get; set; }
        public TimeSpan EffectiveDuration { get; set; }

        private string? _UltimoTempoTipoID;
        public string? UltimoTempoTipoID
        {
            get { return _UltimoTempoTipoID; }
            set
            {
                _UltimoTempoTipoID = value;
                NotifyPropertyChanged("UltimoTempoTipoID");
            }
        }

        private DateTime? _UltimoTempoData;
        public DateTime? UltimoTempoData
        {
            get { return _UltimoTempoData; }
            set
            {
                _UltimoTempoData = value;
                NotifyPropertyChanged("UltimoTempoData");
            }
        }

        private string? _UltimoTempoOperatoreID;
        public string? UltimoTempoOperatoreID
        {
            get { return _UltimoTempoOperatoreID; }
            set
            {
                _UltimoTempoOperatoreID = value;
                NotifyPropertyChanged("UltimoTempoOperatoreID");
            }
        }

        private string? _UltimoTempoOperatoreDescrizione;
        public string? UltimoTempoOperatoreDescrizione
        {
            get { return _UltimoTempoOperatoreDescrizione; }
            set
            {
                _UltimoTempoOperatoreDescrizione = value;
                NotifyPropertyChanged("UltimoTempoOperatoreDescrizione");
            }
        }

        public int ClienteID { get; set; }
        public string? ClienteDescrizione { get; set; }
        public string? RepartoDescrizione { get; set; }
        public string? ArticoloDescrizione { get; set; }
        public string? UnitaDescrizione { get; set; }
        public string? RisorsaDescrizione { get; set; }

        // GANTT TASK
        public string? UniqueId { get; set; }
        public string? Description { get; set; }
        public DateTime Start { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsMilestone { get; set; }
        public string? Title { get; set; }
        public List<pro_ordine_composizione> Children { get; set; } = new();
        public List<pro_ordine_composizioneDependency> Dependencies { get; set; } = new();

        public ObservableCollection<tab_produzione_calendario_chiusura> Stops { get; set; } = new ObservableCollection<tab_produzione_calendario_chiusura>();
        public ObservableCollection<pro_ordine_composizione> DependantTasks { get; set; } = new ObservableCollection<pro_ordine_composizione>();
        public ObservableCollection<pro_ordine_composizione_tempo> Times { get; set; } = new ObservableCollection<pro_ordine_composizione_tempo>();
        public List<tab_produzione_calendario_chiusura> OverlappedSpots { get; set; } = new List<tab_produzione_calendario_chiusura>();
        public TimeSpan TotalOverlappedSpotsDuration
        {
            get
            {
                return new TimeSpan(OverlappedSpots.Sum(s => s.Duration.Ticks));
            }
        }

        public bool ERitardo
        {
            get
            {
                return this.DataConsegna < this.End;
            }
        }
        public TimeSpan Ritardo
        {
            get
            {
                if (this.DataConsegna.HasValue)
                {
                    return this.DataConsegna.Value.Subtract(this.End);
                }
                return new TimeSpan();
            }
        }

        #region TODO Workaround per non linkare Telerik
        public bool IsExpired { get; set; }
        public DateTime End { get; set; }
        #endregion

        public string ToolTipText
        {
            get
            {
                if (this.IsExpired)
                {
                    return string.Format("Overdue: {0}h", this.Ritardo.ToString(@"dd\:hh\:mm\:ss"));
                }
                return "On Time";
            }
        }
        public bool EEspanso { get; set; } = false;
        public bool ETerminato { get; set; } = false;
        public int PosizioneGantt { get; set; }

        private ObservableCollection<StockInfo>? availabilities;
        public ObservableCollection<StockInfo>? Availabilities { get => availabilities; set { availabilities = value; NotifyPropertyChanged("Availabilities"); } }

        #region Overrides
        public override string ToString()
        {
            return $"{ComponenteArticoloDescrizione} | Q: {Quantita} | QI: {QuantitaImpegnata} | QO: {QuantitaOriginale} | QTD: {QuantitaToDo}";
        }
        #endregion

        #region Ufp
        public string? PRSOCB { get; set; }
        public short? PRANNP { get; set; }
        public int? PRORDP { get; set; }
        public short? PRNSEQ { get; set; }

        public string? PRFASE { get; set; }
        public string? FaseDescrizione { get; set; }
        #endregion
    }
}
