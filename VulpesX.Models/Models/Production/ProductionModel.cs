using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Ufp;

namespace VulpesX.Models.Models.Production
{
    public class ProductionModel
    {
        public class ProductionUfpModel : Base
        {
            public required string TipoID { get; set; }

            public required string SocietaID { get; set; }

            private bool _IsSelected;
            public bool IsSelected { get { return _IsSelected; } set { _IsSelected = value; NotifyPropertyChanged(); } }

            public int Sequenza { get; set; }
            public string? RepartoID { get; set; }
            public string? RepartoDescrizione { get; set; }
            public string? RepartoFull { get { return $"{RepartoID?.TrimEnd()} - {RepartoDescrizione?.TrimEnd()}"; } }
            public string? FaseID { get; set; }
            public string? FaseDescrizione { get; set; }
            public string? FaseFull { get { return $"{FaseID?.TrimEnd()} - {FaseDescrizione?.TrimEnd()}"; } }
            public bool FaseCNC { get; set; }
            public string FaseCNCString { get { return FaseCNC ? "Si" : "No"; } }
            public string FaseCNCColor { get { return FaseCNC ? "G" : "R"; } }

            public decimal? TempoPiazzamento { get; set; }
            public decimal? TempoProduzione { get; set; }

            public ObservableCollection<TEMPI_MEDI_VISTA>? TempiMedi { get; set; }
            public int TempiMediPiazzamentoCount { get { return (TempiMedi ?? new ObservableCollection<TEMPI_MEDI_VISTA>()).Where(o=>o.TempoPiazzamentoPezzo > 0).Count(); } }
            public int TempiMediProduzioneCount { get { return (TempiMedi ?? new ObservableCollection<TEMPI_MEDI_VISTA>()).Where(o => o.TempoProduzionePezzo > 0).Count(); } }
        }

        public class ProductionTimeUfpModel
        {
            public required string SocietaID { get; set; }
            public short Anno { get; set; }
            public int ID { get; set; }
            public short Sequenza { get; set; }
            public short Progressivo { get; set; }
            public DateTime Data { get; set; }
            public string? TipoID { get; set; }
            public string? TipoDescrizione { get; set; }

            public decimal? QuantitaOrdine { get; set; }
            public decimal? QuantitaVersata { get; set; }
            public decimal? QuantitaScartata { get; set; }

            public string? RepartoID { get; set; }
            public string? RepartoDescrizione { get; set; }
            public string? RepartoFull { get { return $"{RepartoID?.TrimEnd()} - {RepartoDescrizione?.TrimEnd()}"; } }
            public string? FaseID { get; set; }
            public string? FaseDescrizione { get; set; }
            public string? FaseFull { get { return $"{FaseID?.TrimEnd()} - {FaseDescrizione?.TrimEnd()}"; } }
            public string? RisorsaID { get; set; }
            public string? RisorsaDescrizione { get; set; }
            public string? RisorsaFull { get { return $"{RisorsaID?.TrimEnd()} - {RisorsaDescrizione?.TrimEnd()}"; } }
            public string? OperatoreID { get; set; }
            public string? OperatoreDescrizione { get; set; }
            public string? OperatoreFull { get { return $"{OperatoreID?.TrimEnd()} - {OperatoreDescrizione?.TrimEnd()}"; } }

            public string? SequenzaFull { get { return $"{Sequenza} - {RepartoFull} - {FaseFull}"; } }

            public bool IsTotal { get; set; }

            public bool EProcessata { get; set; }
            public TimeSpan? Durata { get; set; }

            public TimeSpan? TotalePiazzamento { get; set; }
            public TimeSpan? TotaleProduzione { get; set; }

            public TimeSpan? TempoProduzionePezzo { get; set; }
        }
    }
}
