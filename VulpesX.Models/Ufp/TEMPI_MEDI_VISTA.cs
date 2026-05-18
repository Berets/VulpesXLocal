using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class TEMPI_MEDI_VISTA : Base
    {
        private string _Societa = null!;
        public required string Societa { get => _Societa; set { if (_Societa != value) { _Societa = value; NotifyPropertyChanged(); } } }

        private int _TempoID;
        public int TempoID { get => _TempoID; set { if (_TempoID != value) { _TempoID = value; NotifyPropertyChanged(); } } }

        private string? _TempoSequenza;
        public string? TempoSequenza { get => _TempoSequenza; set { if (_TempoSequenza != value) { _TempoSequenza = value; NotifyPropertyChanged(); } } }

        private DateTime _TempoData;
        public DateTime TempoData { get => _TempoData; set { if (_TempoData != value) { _TempoData = value; NotifyPropertyChanged(); } } }

        private string? _TempoTipo;
        public string? TempoTipo { get => _TempoTipo; set { if (_TempoTipo != value) { _TempoTipo = value; NotifyPropertyChanged(); } } }

        private decimal _TempoQuantitaVersata;
        public decimal TempoQuantitaVersata { get => _TempoQuantitaVersata; set { if (_TempoQuantitaVersata != value) { _TempoQuantitaVersata = value; NotifyPropertyChanged(); } } }

        private decimal _TempoQuantitaScartata;
        public decimal TempoQuantitaScartata { get => _TempoQuantitaScartata; set { if (_TempoQuantitaScartata != value) { _TempoQuantitaScartata = value; NotifyPropertyChanged(); } } }

        private string? _OrdineCommessa;
        public string? OrdineCommessa { get => _OrdineCommessa; set { if (_OrdineCommessa != value) { _OrdineCommessa = value; NotifyPropertyChanged(); } } }

        private decimal _OrdineQuantita;
        public decimal OrdineQuantita { get => _OrdineQuantita; set { if (_OrdineQuantita != value) { _OrdineQuantita = value; NotifyPropertyChanged(); } } }

        private string? _RepartoID;
        public string? RepartoID { get => _RepartoID; set { if (_RepartoID != value) { _RepartoID = value; NotifyPropertyChanged(); } } }

        private string? _RepartoDescrizione;
        public string? RepartoDescrizione { get => _RepartoDescrizione; set { if (_RepartoDescrizione != value) { _RepartoDescrizione = value; NotifyPropertyChanged(); } } }

        private string? _RepartoFull;
        public string? RepartoFull { get => _RepartoFull; set { if (_RepartoFull != value) { _RepartoFull = value; NotifyPropertyChanged(); } } }

        private string? _FaseID;
        public string? FaseID { get => _FaseID; set { if (_FaseID != value) { _FaseID = value; NotifyPropertyChanged(); } } }

        private string? _FaseDescrizione;
        public string? FaseDescrizione { get => _FaseDescrizione; set { if (_FaseDescrizione != value) { _FaseDescrizione = value; NotifyPropertyChanged(); } } }

        private string? _FaseFull;
        public string? FaseFull { get => _FaseFull; set { if (_FaseFull != value) { _FaseFull = value; NotifyPropertyChanged(); } } }

        private decimal _Produzione;
        public decimal Produzione { get => _Produzione; set { if (_Produzione != value) { _Produzione = value; NotifyPropertyChanged(); } } }

        private decimal _Piazzamento;
        public decimal Piazzamento { get => _Piazzamento; set { if (_Piazzamento != value) { _Piazzamento = value; NotifyPropertyChanged(); } } }

        private decimal _TempoPiazzamentoPezzo;
        public decimal TempoPiazzamentoPezzo { get => _TempoPiazzamentoPezzo; set { if (_TempoPiazzamentoPezzo != value) { _TempoPiazzamentoPezzo = value; NotifyPropertyChanged(); } } }

        private decimal _TempoProduzionePezzo;
        public decimal TempoProduzionePezzo { get => _TempoProduzionePezzo; set { if (_TempoProduzionePezzo != value) { _TempoProduzionePezzo = value; NotifyPropertyChanged(); } } }
    }
}
