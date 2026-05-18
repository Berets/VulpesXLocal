using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models.Accounting.eInvoice
{
    public class FullDocument
    {
        public string? TipoFileXML { get; set; } // O = Ordinario , S = Semplificato
        public string? PaeseTrasmittente { get; set; }
        public string? PartitaIvaTrasmittente { get; set; }
        public string? ProgressivoInvio { get; set; }
        public string? FormatoTrasmissione { get; set; }
        public string? CodiceDestinatario { get; set; }
        public string? PECDestinatario { get; set; }
        #region [CedentePrestatore]
        public string? CedentePrestatorePaese { get; set; }
        public string? CedentePrestatorePartitaIVA { get; set; }
        public string? CedentePrestatoreCodiceFiscale { get; set; }
        public string? CedentePrestatoreDenominazione { get; set; }
        public string? CedentePrestatoreRegimeFiscale { get; set; }
        public string? CedentePrestatoreSedeIndirizzo { get; set; }
        public string? CedentePrestatoreSedeCap { get; set; }
        public string? CedentePrestatoreSedeNumeroCivico { get; set; }
        public string? CedentePrestatoreSedeComune { get; set; }
        public string? CedentePrestatoreSedeProvincia { get; set; }
        public string? CedentePrestatoreSedeNazione { get; set; }
        public string? CedentePrestatoreREAUfficio { get; set; }
        public string? CedentePrestatoreREANumero { get; set; }
        public decimal? CedentePrestatoreREACapitaleSociale { get; set; }
        public string? CedentePrestatoreREASocioUnico { get; set; }
        public string? CedentePrestatoreREAStatoLiquidazione { get; set; }
        public string? CedenteNome { get; set; }
        public string? CedenteCognome { get; set; }
        public string? CedenteTitolo { get; set; }
        public string? CedenteCodEORI { get; set; }
        public string? CedentePrestatoreRiferimentoAmministrazione { get; set; }
        #region Contatti
        public string? CedenteTelefono { get; set; }
        public string? CedenteFax { get; set; }
        public string? CedenteEmail { get; set; }
        #endregion
        #region Stabile organizzazione
        public string? CedenteSOIndirizzo { get; set; }
        public string? CedenteSONumeroCivico { get; set; }
        public string? CedenteSOCAP { get; set; }
        public string? CedenteSOComune { get; set; }
        public string? CedenteSOProvincia { get; set; }
        public string? CedenteSONazione { get; set; }
        #endregion
        #region Rappresentante fiscale
        public string? CedenteRFIDPaese { get; set; }
        public string? CedenteRFIDCodice { get; set; }
        public string? CedenteRFDenominazione { get; set; }
        public string? CedenteRFNome { get; set; }
        public string? CedenteRFCognome { get; set; }
        public string? CedenteRFCodiceFiscale { get; set; }
        public string? CedenteRFTitolo { get; set; }
        public string? CedenteRFCodEORI { get; set; }
        #endregion   
        #endregion
        #region [CessionarioCommittente]
        public string? CessionarioCommittentePaese { get; set; }
        public string? CessionarioCommittentePartitaIVA { get; set; }
        public string? CessionarioCommittenteCodiceFiscale { get; set; }
        public string? CessionarioCommittenteDenominazione { get; set; }
        public string? CessionarioCommittenteNome { get; set; }
        public string? CessionarioCommittenteCognome { get; set; }
        public string? CessionarioCommittenteTitolo { get; set; }
        public string? CessionarioCommittenteCodEORI { get; set; }
        #region Sede
        public string? CessionarioCommittenteSedeIndirizzo { get; set; }
        public string? CessionarioCommittenteSedeCap { get; set; }
        public string? CessionarioCommittenteSedeNumeroCivico { get; set; }
        public string? CessionarioCommittenteSedeComune { get; set; }
        public string? CessionarioCommittenteSedeProvincia { get; set; }
        public string? CessionarioCommittenteSedeNazione { get; set; }
        #endregion
        #region Stabile organizzazione
        public string? CessionarioCommittenteSOIndirizzo { get; set; }
        public string? CessionarioCommittenteSOCap { get; set; }
        public string? CessionarioCommittenteSONumeroCivico { get; set; }
        public string? CessionarioCommittenteSOComune { get; set; }
        public string? CessionarioCommittenteSOProvincia { get; set; }
        public string? CessionarioCommittenteSONazione { get; set; }
        #endregion
        #region Rappresentante fiscale
        public string? CessionarioRFIDPaese { get; set; }
        public string? CessionarioRFIDCodice { get; set; }
        public string? CessionarioRFDenominazione { get; set; }
        public string? CessionarioRFNome { get; set; }
        public string? CessionarioRFCognome { get; set; }
        #endregion
        #endregion
        public string? SoggettoEmittente { get; set; }
        public List<FullDocumentItem>? Fatture { get; set; }
    }
}
