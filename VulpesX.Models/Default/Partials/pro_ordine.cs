 

namespace VulpesX.Models.Default
{
    public partial class pro_ordine
    {
        public string? ClienteDescrizione { get; set; }
        public string? ArticoloDescrizione { get; set; }
        public string? RiferimentoOrdineCliente { get; set; }
        public string FullCustomerDescription => $"{ClienteID} {ClienteDescrizione}";
        public string FullProductDescription => $"{ArticoloID} {ArticoloDescrizione}";
        public string? StatoDescrizione => CommonsService.ProductOrderTypes.Where(w => w.ID == Stato).FirstOrDefault()?.Description;
        public string? UM { get; set; }
        public string OrdineClienteFullID => $"{OrdineClienteAnno} - {OrdineClienteID} - {OrdineClienteRiga}";

        public long? DurataPrevista { get; set; }
        public long? DurataProduzione { get; set; }
        public long? DurataSospensione { get; set; }
        public long? DurataPiazzamento { get; set; }

        public TimeSpan DurataPrevistaSpan { get { return new TimeSpan(DurataPrevista ?? 0); } }
        public TimeSpan DurataProduzioneSpan { get { return new TimeSpan(DurataProduzione ?? 0); } }
        public TimeSpan DurataSospensioneSpan { get { return new TimeSpan(DurataSospensione ?? 0); } }
        public TimeSpan DurataPiazzamentoSpan { get { return new TimeSpan(DurataPiazzamento ?? 0); } }

        public bool CanDeleteVisibility => Stato == "A" || Stato == "O" ? true : false;

        #region Ufp
        public string? OPSOCI { get; set; }
        public short? OPANNP { get; set; }
        public int? OPNUOP { get; set; }
        #endregion
    }
}
