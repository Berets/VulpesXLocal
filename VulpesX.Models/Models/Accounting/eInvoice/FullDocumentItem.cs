using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models.Accounting.eInvoice
{
    public class FullDocumentItem
    {
        public string? Tipo { get; set; }
        public string? Divisa { get; set; }
        public string? Data { get; set; }
        public string? Numero { get; set; }
        public string? BolloVirtuale { get; set; }
        public decimal? ImportoTotaleDocumento { get; set; }
        public decimal? Arrotondamento { get; set; }
        public List<string>? Causali { get; set; }
        public string? Art73 { get; set; }
        public List<FullDocumentItemRow>? RigheFattura { get; set; }
        public List<FullDocumentItemRecap>? DatiRiepilogo { get; set; }
        public List<FullDocumentItemPayment>? Pagamenti { get; set; }
        public List<FullDocumentItemDiscount>? ScontiMaggiorazioni { get; set; }

        // 2.1.2
        public List<FullDocumentItemBuying>? DatiOrdiniAcquisto { get; set; }

        // 2.1.3
        public List<FullDocumentItemBuying>? DatiContratto { get; set; }

        // 2.1.4
        public List<FullDocumentItemBuying>? DatiConvenzione { get; set; }

        // 2.1.5
        public List<FullDocumentItemBuying>? DatiRicezione { get; set; }

        // 2.1.7
        public List<int?>? DatiSALRiferimentoFase { get; set; }

        // 2.1.8
        public List<FullDocumentItemDDT>? DatiDDT { get; set; }

        // 2.1.9
        #region DatiTrasporto
        public string? DatiTrasportoDAVIDPaese { get; set; }
        public string? DatiTrasportoDAVIDCodice { get; set; }
        public string? DatiTrasportoDAVCodiceFiscale { get; set; }
        public string? DatiTrasportoDAVDenominazione { get; set; }
        public string? DatiTrasportoDAVCognome { get; set; }
        public string? DatiTrasportoDAVNome { get; set; }
        public string? DatiTrasportoDAVTitolo { get; set; }
        public string? DatiTrasportoDAVCodEORI { get; set; }
        public string? DatiTrasportoDAVNumeroLicenzaGuida { get; set; }
        public string? DatiTrasportoMezzoTrasporto { get; set; }
        public string? DatiTrasportoCausaleTrasporto { get; set; }
        public int? DatiTrasportoNumeroColli { get; set; }
        public string? DatiTrasportoDescrizione { get; set; }
        public string? DatiTrasportoUnitaMisuraPeso { get; set; }
        public decimal? DatiTrasportoPesoLordo { get; set; }
        public decimal? DatiTrasportoPesoNetto { get; set; }
        public string? DatiTrasportoDataOraRitiro { get; set; }
        public string? DatiTrasportoDataInizioTrasporto { get; set; }
        public string? DatiTrasportoTipoResa { get; set; }
        public string? DatiTrasportoResaIndirizzo { get; set; }
        public string? DatiTrasportoResaCivico { get; set; }
        public string? DatiTrasportoResaCAP { get; set; }
        public string? DatiTrasportoResaComune { get; set; }
        public string? DatiTrasportoResaProvincia { get; set; }
        public string? DatiTrasportoResaNazione { get; set; }
        public string? DatiTrasportoDataOraConsegna { get; set; }
        #endregion

        // 2.1.10
        public string? NumeroFatturaPrincipale { get; set; }
        public string? DataFatturaPrincipale { get; set; }

        // 2.3
        public string? DatiVeicoliData { get; set; }
        public string? DatiVeicoliTotalePercorso { get; set; }

        public List<FullDocumentItemBuying>? DatiFattureCollegate { get; set; }
        public List<FullDocumentItemPensionFund>? DatiCassaPrevidenziale { get; set; }
        public List<FullDocumentItemRitenuta>? DatiRitenuta { get; set; }
        public string? DatiBolloVirtuale { get; set; }
        public decimal? DatiBolloImporto { get; set; }
        #region Fattura semplificata
        public string? NumeroFR { get; set; }
        public string? DataFR { get; set; }
        public string? ElementiRettificati { get; set; }
        #endregion
    }

}
