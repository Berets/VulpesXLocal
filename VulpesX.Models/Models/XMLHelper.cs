using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using VulpesX.Models.Models.Accounting.eInvoice;

namespace VulpesX.Models.Models
{
    public static class XMLHelper
    {
        public static XmlDocument? GetDocumentFromBytes(byte[] Data)
        {
            if (Data != null)
            {
                if (Data.LongLength > 0)
                {
                    // check if Base64
                    try
                    {
                        Data = Convert.FromBase64String(Encoding.UTF8.GetString(Data));
                    }
                    catch (FormatException)
                    { }

                    XmlDocument doc = new XmlDocument();
                    byte[] bom = Encoding.UTF8.GetPreamble();
                    if (Data[0] != 60 && (Data[0] != bom[0] || Data[1] != bom[1] || Data[2] != bom[2]))
                    {
                        SignedCms cms = new SignedCms();
                        cms.Decode(Data);
                        Data = cms.ContentInfo.Content;
                    }
                    // remove BOM
                    short startIndex = 0;
                    if (Data[0] == bom[0] && Data[1] == bom[1] && Data[2] == bom[2])
                        startIndex = 3;
                    doc.LoadXml(Encoding.UTF8.GetString(Data, startIndex, Data.Length - startIndex));
                    return doc;
                }
            }
            return null;
        }

        public static FullDocument GetDocumentFullInfo(XmlDocument Document)
        {
            FullDocument document = new FullDocument();
            document.Fatture = new List<FullDocumentItem>();
            XmlNamespaceManager nsManager = new XmlNamespaceManager(Document.NameTable);
            nsManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            nsManager.AddNamespace("p", "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2");
            nsManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            if (Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/DatiTrasmissione/IdTrasmittente/IdPaese", nsManager) != null)
            {
                #region Ordinaria
                document.TipoFileXML = "O";
                #region header [FatturaElettronicaHeader]
                document.PaeseTrasmittente = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/DatiTrasmissione/IdTrasmittente/IdPaese", nsManager)?.InnerText;
                document.PartitaIvaTrasmittente = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/DatiTrasmissione/IdTrasmittente/IdCodice", nsManager)?.InnerText;
                document.ProgressivoInvio = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/DatiTrasmissione/ProgressivoInvio", nsManager)?.InnerText;
                document.FormatoTrasmissione = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/DatiTrasmissione/FormatoTrasmissione", nsManager)?.InnerText;
                document.CodiceDestinatario = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/DatiTrasmissione/CodiceDestinatario", nsManager)?.InnerText;
                #region [CedentePrestatore]
                document.CedentePrestatoreCodiceFiscale = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/DatiAnagrafici/CodiceFiscale", nsManager)?.InnerText;
                document.CedentePrestatorePaese = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/DatiAnagrafici/IdFiscaleIVA/IdPaese", nsManager)?.InnerText;
                document.CedentePrestatorePartitaIVA = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/DatiAnagrafici/IdFiscaleIVA/IdCodice", nsManager)?.InnerText;
                document.CedentePrestatoreDenominazione = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/DatiAnagrafici/Anagrafica/Denominazione", nsManager)?.InnerText;
                document.CedenteNome = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/DatiAnagrafici/Anagrafica/Nome", nsManager)?.InnerText;
                document.CedenteCognome = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/DatiAnagrafici/Anagrafica/Cognome", nsManager)?.InnerText;
                document.CedenteTitolo = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/DatiAnagrafici/Anagrafica/Titolo", nsManager)?.InnerText;
                document.CedenteCodEORI = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/DatiAnagrafici/Anagrafica/CodEORI", nsManager)?.InnerText;
                document.CedentePrestatoreRegimeFiscale = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/DatiAnagrafici/RegimeFiscale", nsManager)?.InnerText;
                document.CedentePrestatoreSedeIndirizzo = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/Sede/Indirizzo", nsManager)?.InnerText;
                document.CedentePrestatoreSedeCap = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/Sede/CAP", nsManager)?.InnerText;
                document.CedentePrestatoreSedeNumeroCivico = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/Sede/NumeroCivico", nsManager)?.InnerText;
                document.CedentePrestatoreSedeComune = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/Sede/Comune", nsManager)?.InnerText;
                document.CedentePrestatoreSedeProvincia = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/Sede/Provincia", nsManager)?.InnerText;
                document.CedentePrestatoreSedeNazione = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/Sede/Nazione", nsManager)?.InnerText;
                document.CedentePrestatoreREAUfficio = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/IscrizioneREA/Ufficio", nsManager)?.InnerText;
                document.CedentePrestatoreREANumero = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/IscrizioneREA/NumeroREA", nsManager)?.InnerText;
                var reaCapital = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/IscrizioneREA/CapitaleSociale", nsManager);
                document.CedentePrestatoreREACapitaleSociale = reaCapital != null ? decimal.Parse(reaCapital.InnerText, new CultureInfo("en-US")) : default(decimal?);
                document.CedentePrestatoreREASocioUnico = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/IscrizioneREA/SocioUnico", nsManager)?.InnerText;
                document.CedentePrestatoreREAStatoLiquidazione = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/IscrizioneREA/StatoLiquidazione", nsManager)?.InnerText;
                document.CedentePrestatoreRiferimentoAmministrazione = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/RiferimentoAmministrazione", nsManager)?.InnerText;
                // Contatti
                document.CedenteTelefono = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/Contatti/Telefono", nsManager)?.InnerText;
                document.CedenteFax = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/Contatti/Fax", nsManager)?.InnerText;
                document.CedenteEmail = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/Contatti/Email", nsManager)?.InnerText;
                // StabileOrganizzazione
                document.CedenteSOIndirizzo = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/StabileOrganizzazione/Indirizzo", nsManager)?.InnerText;
                document.CedenteSONumeroCivico = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/StabileOrganizzazione/NumeroCivico", nsManager)?.InnerText;
                document.CedenteSOCAP = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/StabileOrganizzazione/CAP", nsManager)?.InnerText;
                document.CedenteSOComune = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/StabileOrganizzazione/Comune", nsManager)?.InnerText;
                document.CedenteSOProvincia = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/StabileOrganizzazione/Provincia", nsManager)?.InnerText;
                document.CedenteSONazione = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CedentePrestatore/StabileOrganizzazione/Nazione", nsManager)?.InnerText;
                // RappresentanteFiscale
                document.CedenteRFIDPaese = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/RappresentanteFiscale/DatiAnagrafici/IdFiscaleIVA/IdPaese", nsManager)?.InnerText;
                document.CedenteRFIDCodice = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/RappresentanteFiscale/DatiAnagrafici/IdFiscaleIVA/IdCodice", nsManager)?.InnerText;
                document.CedenteRFCodiceFiscale = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/RappresentanteFiscale/DatiAnagrafici/CodiceFiscale", nsManager)?.InnerText;
                document.CedenteRFDenominazione = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/RappresentanteFiscale/DatiAnagrafici/Anagrafica/Denominazione", nsManager)?.InnerText;
                document.CedenteRFNome = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/RappresentanteFiscale/DatiAnagrafici/Anagrafica/Nome", nsManager)?.InnerText;
                document.CedenteRFCognome = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/RappresentanteFiscale/DatiAnagrafici/Anagrafica/Cognome", nsManager)?.InnerText;
                document.CedenteRFTitolo = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/RappresentanteFiscale/DatiAnagrafici/Anagrafica/Titolo", nsManager)?.InnerText;
                document.CedenteRFCodEORI = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/RappresentanteFiscale/DatiAnagrafici/Anagrafica/CodEORI", nsManager)?.InnerText;
                #endregion
                #region [CessionarioCommittente]
                document.CessionarioCommittenteCodiceFiscale = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/DatiAnagrafici/CodiceFiscale", nsManager)?.InnerText;
                document.CessionarioCommittentePaese = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/DatiAnagrafici/IdFiscaleIVA/IdPaese", nsManager)?.InnerText;
                document.CessionarioCommittentePartitaIVA = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/DatiAnagrafici/IdFiscaleIVA/IdCodice", nsManager)?.InnerText;
                document.CessionarioCommittenteDenominazione = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/DatiAnagrafici/Anagrafica/Denominazione", nsManager)?.InnerText;
                document.CessionarioCommittenteNome = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/DatiAnagrafici/Anagrafica/Nome", nsManager)?.InnerText;
                document.CessionarioCommittenteCognome = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/DatiAnagrafici/Anagrafica/Cognome", nsManager)?.InnerText;
                document.CessionarioCommittenteTitolo = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/DatiAnagrafici/Anagrafica/Titolo", nsManager)?.InnerText;
                document.CessionarioCommittenteCodEORI = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/DatiAnagrafici/Anagrafica/CodEORI", nsManager)?.InnerText;
                // Sede
                document.CessionarioCommittenteSedeIndirizzo = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/Sede/Indirizzo", nsManager)?.InnerText;
                document.CessionarioCommittenteSedeCap = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/Sede/CAP", nsManager)?.InnerText;
                document.CessionarioCommittenteSedeNumeroCivico = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/Sede/NumeroCivico", nsManager)?.InnerText;
                document.CessionarioCommittenteSedeComune = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/Sede/Comune", nsManager)?.InnerText;
                document.CessionarioCommittenteSedeProvincia = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/Sede/Provincia", nsManager)?.InnerText;
                document.CessionarioCommittenteSedeNazione = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/Sede/Nazione", nsManager)?.InnerText;
                // StabileOrganizzazione
                document.CessionarioCommittenteSOIndirizzo = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/StabileOrganizzazione/Indirizzo", nsManager)?.InnerText;
                document.CessionarioCommittenteSOCap = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/StabileOrganizzazione/CAP", nsManager)?.InnerText;
                document.CessionarioCommittenteSONumeroCivico = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/StabileOrganizzazione/NumeroCivico", nsManager)?.InnerText;
                document.CessionarioCommittenteSOComune = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/StabileOrganizzazione/Comune", nsManager)?.InnerText;
                document.CessionarioCommittenteSOProvincia = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/StabileOrganizzazione/Provincia", nsManager)?.InnerText;
                document.CessionarioCommittenteSONazione = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/StabileOrganizzazione/Nazione", nsManager)?.InnerText;
                // RappresentanteFiscale
                document.CessionarioRFIDPaese = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/RappresentanteFiscale/IdFiscaleIVA/IdPaese", nsManager)?.InnerText;
                document.CessionarioRFIDCodice = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/RappresentanteFiscale/IdFiscaleIVA/IdCodice", nsManager)?.InnerText;
                document.CessionarioRFDenominazione = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/RappresentanteFiscale/Denominazione", nsManager)?.InnerText;
                document.CessionarioRFNome = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/RappresentanteFiscale/Nome", nsManager)?.InnerText;
                document.CessionarioRFCognome = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/CessionarioCommittente/RappresentanteFiscale/Cognome", nsManager)?.InnerText;
                #endregion
                var emitter = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/SoggettoEmittente", nsManager);
                document.SoggettoEmittente = emitter != null ? emitter.InnerText : null;
                if (document.CodiceDestinatario == "000000" || document.CodiceDestinatario == "0000000")
                    document.PECDestinatario = Document.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/DatiTrasmissione/PECDestinatario", nsManager)?.InnerText;
                #endregion
                #region invoices [FatturaElettronicaBody]
                var nodes = Document.SelectNodes("p:FatturaElettronica/FatturaElettronicaBody", nsManager);

                if (nodes != null)
                {
                    foreach (XmlNode node in nodes)
                    {
                        FullDocumentItem invoice = new FullDocumentItem();
                        invoice.Tipo = node.SelectSingleNode("DatiGenerali/DatiGeneraliDocumento/TipoDocumento", nsManager)?.InnerText;
                        invoice.Divisa = node.SelectSingleNode("DatiGenerali/DatiGeneraliDocumento/Divisa", nsManager)?.InnerText;
                        invoice.Data = !string.IsNullOrEmpty(node.SelectSingleNode("DatiGenerali/DatiGeneraliDocumento/Data", nsManager)?.InnerText) ?
                            DateTime.Parse(node.SelectSingleNode("DatiGenerali/DatiGeneraliDocumento/Data", nsManager)!.InnerText).ToString("d", CultureInfo.CreateSpecificCulture("it-IT")) : null;
                        invoice.Numero = node.SelectSingleNode("DatiGenerali/DatiGeneraliDocumento/Numero", nsManager)?.InnerText;
                        var totalImport = node.SelectSingleNode("DatiGenerali/DatiGeneraliDocumento/ImportoTotaleDocumento", nsManager);
                        invoice.ImportoTotaleDocumento = totalImport != null ? decimal.Parse(totalImport.InnerText, new CultureInfo("en-US")) : default(decimal?);
                        var genRounding = node.SelectSingleNode("DatiGenerali/DatiGeneraliDocumento/Arrotondamento", nsManager);
                        invoice.Arrotondamento = genRounding != null ? decimal.Parse(genRounding.InnerText, new CultureInfo("en-US")) : default(decimal?);
                        foreach (XmlNode ca in node.SelectNodes(".//DatiGenerali/DatiGeneraliDocumento/Causale", nsManager)?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>())
                        {
                            if (invoice.Causali == null)
                                invoice.Causali = new List<string>();
                            invoice.Causali.Add(ca.InnerText.Trim());
                        }
                        invoice.Art73 = node.SelectSingleNode("DatiGenerali/DatiGeneraliDocumento/Art73", nsManager)?.InnerText;
                        invoice.DatiBolloVirtuale = node.SelectSingleNode("DatiGenerali/DatiGeneraliDocumento/DatiBollo/BolloVirtuale", nsManager)?.InnerText;
                        var stamp = node.SelectSingleNode("DatiGenerali/DatiGeneraliDocumento/DatiBollo/ImportoBollo", nsManager);
                        invoice.DatiBolloImporto = stamp != null ? decimal.Parse(stamp.InnerText, new CultureInfo("en-US")) : default(decimal?);
                        #region [DatiOrdineAcquisto] 2.1.2
                        foreach (XmlNode bo in node.SelectNodes(".//DatiGenerali/DatiOrdineAcquisto", nsManager)?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>())
                        {
                            if (invoice.DatiOrdiniAcquisto == null)
                                invoice.DatiOrdiniAcquisto = new List<FullDocumentItemBuying>();
                            FullDocumentItemBuying newRow = new FullDocumentItemBuying();
                            newRow.IdDocumento = bo.SelectSingleNode("IdDocumento")?.InnerText;
                            newRow.CodiceCommessaConvenzione = bo.SelectSingleNode("CodiceCommessaConvenzione")?.InnerText;
                            newRow.CodiceCUP = bo.SelectSingleNode("CodiceCUP")?.InnerText;
                            newRow.CodiceCIG = bo.SelectSingleNode("CodiceCIG")?.InnerText;
                            newRow.NumItem = bo.SelectSingleNode("NumItem")?.InnerText;
                            var dataBO = bo.SelectSingleNode("Data", nsManager);
                            newRow.Data = dataBO != null ? (!string.IsNullOrWhiteSpace(dataBO.InnerText) ? (DateTime.Parse(dataBO.InnerText).ToString("d", CultureInfo.CreateSpecificCulture("it-IT"))) : null) : null;
                            foreach (XmlNode line in bo.SelectNodes(".//RiferimentoNumeroLinea", nsManager)?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>())
                            {
                                if (newRow.RiferimentoLinea == null)
                                    newRow.RiferimentoLinea = new List<long>();
                                newRow.RiferimentoLinea.Add(long.Parse(line.InnerText));
                            }
                            invoice.DatiOrdiniAcquisto.Add(newRow);
                        }
                        #endregion

                        #region [DatiContratto] 2.1.3
                        foreach (XmlNode bo in node.SelectNodes(".//DatiGenerali/DatiContratto", nsManager)?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>())
                        {
                            if (invoice.DatiContratto == null)
                                invoice.DatiContratto = new List<FullDocumentItemBuying>();
                            FullDocumentItemBuying newRow = new FullDocumentItemBuying();
                            newRow.IdDocumento = bo.SelectSingleNode("IdDocumento")?.InnerText;
                            newRow.CodiceCommessaConvenzione = bo.SelectSingleNode("CodiceCommessaConvenzione")?.InnerText;
                            newRow.CodiceCUP = bo.SelectSingleNode("CodiceCUP")?.InnerText;
                            newRow.CodiceCIG = bo.SelectSingleNode("CodiceCIG")?.InnerText;
                            newRow.NumItem = bo.SelectSingleNode("NumItem")?.InnerText;
                            var dataBO = bo.SelectSingleNode("Data", nsManager);
                            newRow.Data = dataBO != null ? (!string.IsNullOrWhiteSpace(dataBO.InnerText) ? (DateTime.Parse(dataBO.InnerText).ToString("d", CultureInfo.CreateSpecificCulture("it-IT"))) : null) : null;
                            foreach (XmlNode line in bo.SelectNodes(".//RiferimentoNumeroLinea", nsManager)?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>())
                            {
                                if (newRow.RiferimentoLinea == null)
                                    newRow.RiferimentoLinea = new List<long>();
                                newRow.RiferimentoLinea.Add(long.Parse(line.InnerText));
                            }
                            invoice.DatiContratto.Add(newRow);
                        }
                        #endregion

                        #region [DatiConvenzione] 2.1.4
                        foreach (XmlNode bo in node.SelectNodes(".//DatiGenerali/DatiConvenzione", nsManager)?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>())
                        {
                            if (invoice.DatiConvenzione == null)
                                invoice.DatiConvenzione = new List<FullDocumentItemBuying>();
                            FullDocumentItemBuying newRow = new FullDocumentItemBuying();
                            newRow.IdDocumento = bo.SelectSingleNode("IdDocumento")?.InnerText;
                            newRow.CodiceCommessaConvenzione = bo.SelectSingleNode("CodiceCommessaConvenzione")?.InnerText;
                            newRow.CodiceCUP = bo.SelectSingleNode("CodiceCUP")?.InnerText;
                            newRow.CodiceCIG = bo.SelectSingleNode("CodiceCIG")?.InnerText;
                            newRow.NumItem = bo.SelectSingleNode("NumItem")?.InnerText;
                            var dataBO = bo.SelectSingleNode("Data", nsManager);
                            newRow.Data = dataBO != null ? (!string.IsNullOrWhiteSpace(dataBO.InnerText) ? (DateTime.Parse(dataBO.InnerText).ToString("d", CultureInfo.CreateSpecificCulture("it-IT"))) : null) : null;
                            foreach (XmlNode line in bo.SelectNodes(".//RiferimentoNumeroLinea", nsManager)?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>())
                            {
                                if (newRow.RiferimentoLinea == null)
                                    newRow.RiferimentoLinea = new List<long>();
                                newRow.RiferimentoLinea.Add(long.Parse(line.InnerText));
                            }
                            invoice.DatiConvenzione.Add(newRow);
                        }
                        #endregion

                        #region [DatiRicezione] 2.1.5
                        foreach (XmlNode bo in node.SelectNodes(".//DatiGenerali/DatiRicezione", nsManager)?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>())
                        {
                            if (invoice.DatiRicezione == null)
                                invoice.DatiRicezione = new List<FullDocumentItemBuying>();
                            FullDocumentItemBuying newRow = new FullDocumentItemBuying();
                            newRow.IdDocumento = bo.SelectSingleNode("IdDocumento")?.InnerText;
                            newRow.CodiceCommessaConvenzione = bo.SelectSingleNode("CodiceCommessaConvenzione")?.InnerText;
                            newRow.CodiceCUP = bo.SelectSingleNode("CodiceCUP")?.InnerText;
                            newRow.CodiceCIG = bo.SelectSingleNode("CodiceCIG")?.InnerText;
                            newRow.NumItem = bo.SelectSingleNode("NumItem")?.InnerText;
                            var dataBO = bo.SelectSingleNode("Data", nsManager);
                            newRow.Data = dataBO != null ? (!string.IsNullOrWhiteSpace(dataBO.InnerText) ? (DateTime.Parse(dataBO.InnerText).ToString("d", CultureInfo.CreateSpecificCulture("it-IT"))) : null) : null;
                            foreach (XmlNode line in bo.SelectNodes(".//RiferimentoNumeroLinea", nsManager)?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>())
                            {
                                if (newRow.RiferimentoLinea == null)
                                    newRow.RiferimentoLinea = new List<long>();
                                newRow.RiferimentoLinea.Add(long.Parse(line.InnerText));
                            }
                            invoice.DatiRicezione.Add(newRow);
                        }
                        #endregion

                        #region [DatiSAL] 2.1.7
                        foreach (XmlNode bo in node.SelectNodes(".//DatiGenerali/DatiSAL", nsManager)?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>())
                        {
                            if (invoice.DatiSALRiferimentoFase == null)
                                invoice.DatiSALRiferimentoFase = new List<int?>();
                            var rifPhase = node.SelectSingleNode("RiferimentoFase", nsManager);
                            invoice.DatiSALRiferimentoFase.Add(rifPhase != null ? int.Parse(rifPhase.InnerText, new CultureInfo("en-US")) : default(int?));
                        }
                        #endregion

                        #region [DatiDDT] 2.1.8
                        foreach (XmlNode ddt in node.SelectNodes(".//DatiGenerali/DatiDDT", nsManager)?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>())
                        {
                            if (invoice.DatiDDT == null)
                                invoice.DatiDDT = new List<FullDocumentItemDDT>();
                            FullDocumentItemDDT newRow = new FullDocumentItemDDT();
                            newRow.Numero = ddt.SelectSingleNode("NumeroDDT")?.InnerText;
                            var dataDDT = ddt.SelectSingleNode("DataDDT", nsManager);
                            newRow.Data = dataDDT != null ? DateTime.Parse(dataDDT.InnerText).ToString("d", CultureInfo.CreateSpecificCulture("it-IT")) : null;
                            foreach (XmlNode line in ddt.SelectNodes(".//RiferimentoNumeroLinea", nsManager)?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>())
                            {
                                if (newRow.RiferimentoLinea == null)
                                    newRow.RiferimentoLinea = new List<long>();
                                newRow.RiferimentoLinea.Add(long.Parse(line.InnerText));
                            }
                            invoice.DatiDDT.Add(newRow);
                        }
                        #endregion

                        #region [DatiTrasporto] 2.1.9
                        invoice.DatiTrasportoDAVIDPaese = node.SelectSingleNode("DatiGenerali/DatiTrasporto/DatiAnagraficiVettore/IdFiscaleIVA/IdPaese", nsManager)?.InnerText;
                        invoice.DatiTrasportoDAVIDCodice = node.SelectSingleNode("DatiGenerali/DatiTrasporto/DatiAnagraficiVettore/IdFiscaleIVA/IdCodice", nsManager)?.InnerText;
                        invoice.DatiTrasportoDAVCodiceFiscale = node.SelectSingleNode("DatiGenerali/DatiTrasporto/DatiAnagraficiVettore/CodiceFiscale", nsManager)?.InnerText;
                        invoice.DatiTrasportoDAVDenominazione = node.SelectSingleNode("DatiGenerali/DatiTrasporto/DatiAnagraficiVettore/Anagrafica/Denominazione", nsManager)?.InnerText;
                        invoice.DatiTrasportoDAVNome = node.SelectSingleNode("DatiGenerali/DatiTrasporto/DatiAnagraficiVettore/Anagrafica/Nome", nsManager)?.InnerText;
                        invoice.DatiTrasportoDAVCognome = node.SelectSingleNode("DatiGenerali/DatiTrasporto/DatiAnagraficiVettore/Anagrafica/Cognome", nsManager)?.InnerText;
                        invoice.DatiTrasportoDAVTitolo = node.SelectSingleNode("DatiGenerali/DatiTrasporto/DatiAnagraficiVettore/Anagrafica/Titolo", nsManager)?.InnerText;
                        invoice.DatiTrasportoDAVCodEORI = node.SelectSingleNode("DatiGenerali/DatiTrasporto/DatiAnagraficiVettore/Anagrafica/CodEORI", nsManager)?.InnerText;
                        invoice.DatiTrasportoDAVNumeroLicenzaGuida = node.SelectSingleNode("DatiGenerali/DatiTrasporto/DatiAnagraficiVettore/NumeroLicenzaGuida", nsManager)?.InnerText;
                        invoice.DatiTrasportoMezzoTrasporto = node.SelectSingleNode("DatiGenerali/DatiTrasporto/MezzoTrasporto", nsManager)?.InnerText;
                        invoice.DatiTrasportoCausaleTrasporto = node.SelectSingleNode("DatiGenerali/DatiTrasporto/CausaleTrasporto", nsManager)?.InnerText;
                        var itemsCount = node.SelectSingleNode("DatiGenerali/DatiTrasporto/NumeroColli", nsManager);
                        invoice.DatiTrasportoNumeroColli = itemsCount != null ? int.Parse(itemsCount.InnerText, new CultureInfo("en-US")) : default(int?);
                        invoice.DatiTrasportoDescrizione = node.SelectSingleNode("DatiGenerali/DatiTrasporto/Descrizione", nsManager)?.InnerText;
                        invoice.DatiTrasportoUnitaMisuraPeso = node.SelectSingleNode("DatiGenerali/DatiTrasporto/UnitaMisuraPeso", nsManager)?.InnerText;
                        var gross = node.SelectSingleNode("DatiGenerali/DatiTrasporto/PesoLordo", nsManager);
                        invoice.DatiTrasportoPesoLordo = gross != null ? decimal.Parse(gross.InnerText, new CultureInfo("en-US")) : default(decimal?);
                        var net = node.SelectSingleNode("DatiGenerali/DatiTrasporto/PesoNetto", nsManager);
                        invoice.DatiTrasportoPesoNetto = net != null ? decimal.Parse(net.InnerText, new CultureInfo("en-US")) : default(decimal?);
                        var wdDate = node.SelectSingleNode("DatiGenerali/DatiTrasporto/DataOraRitiro", nsManager);
                        invoice.DatiTrasportoDataOraRitiro = wdDate != null ? DateTime.Parse(wdDate.InnerText).ToString("G", CultureInfo.CreateSpecificCulture("it-IT")) : null;
                        var startTranDate = node.SelectSingleNode("DatiGenerali/DatiTrasporto/DataInizioTrasporto", nsManager);
                        invoice.DatiTrasportoDataInizioTrasporto = startTranDate != null ? DateTime.Parse(startTranDate.InnerText).ToString("d", CultureInfo.CreateSpecificCulture("it-IT")) : null;
                        invoice.DatiTrasportoTipoResa = node.SelectSingleNode("DatiGenerali/DatiTrasporto/TipoResa", nsManager)?.InnerText;
                        invoice.DatiTrasportoResaIndirizzo = node.SelectSingleNode("DatiGenerali/DatiTrasporto/IndirizzoResa/Indirizzo", nsManager)?.InnerText;
                        invoice.DatiTrasportoResaCivico = node.SelectSingleNode("DatiGenerali/DatiTrasporto/IndirizzoResa/NumeroCivico", nsManager)?.InnerText;
                        invoice.DatiTrasportoResaCAP = node.SelectSingleNode("DatiGenerali/DatiTrasporto/IndirizzoResa/CAP", nsManager)?.InnerText;
                        invoice.DatiTrasportoResaComune = node.SelectSingleNode("DatiGenerali/DatiTrasporto/IndirizzoResa/Comune", nsManager)?.InnerText;
                        invoice.DatiTrasportoResaProvincia = node.SelectSingleNode("DatiGenerali/DatiTrasporto/IndirizzoResa/Provincia", nsManager)?.InnerText;
                        invoice.DatiTrasportoResaNazione = node.SelectSingleNode("DatiGenerali/DatiTrasporto/IndirizzoResa/Nazione", nsManager)?.InnerText;
                        var delivDate = node.SelectSingleNode("DatiGenerali/DatiTrasporto/DataOraConsegna", nsManager);
                        invoice.DatiTrasportoDataOraConsegna = delivDate != null ? DateTime.Parse(delivDate.InnerText).ToString("G", CultureInfo.CreateSpecificCulture("it-IT")) : null;
                        #endregion

                        #region [FatturaPrincipale] 2.1.10
                        invoice.NumeroFatturaPrincipale = node.SelectSingleNode("DatiGenerali/FatturaPrincipale/NumeroFatturaPrincipale", nsManager)?.InnerText;
                        var fpDate = node.SelectSingleNode("DatiGenerali/FatturaPrincipale/DataFatturaPrincipale", nsManager);
                        invoice.DataFatturaPrincipale = fpDate != null ? DateTime.Parse(fpDate.InnerText).ToString("d", CultureInfo.CreateSpecificCulture("it-IT")) : null;
                        #endregion

                        #region [DatiVeicoli] 2.3
                        var vehDate = node.SelectSingleNode("DatiVeicoli/Data", nsManager);
                        invoice.DatiVeicoliData = vehDate != null ? DateTime.Parse(vehDate.InnerText).ToString("d", CultureInfo.CreateSpecificCulture("it-IT")) : null;
                        invoice.DatiVeicoliTotalePercorso = node.SelectSingleNode("DatiVeicoli/TotalePercorso", nsManager)?.InnerText;
                        #endregion

                        #region [DatiFattureCollegate]
                        foreach (XmlNode bo in node.SelectNodes(".//DatiGenerali/DatiFattureCollegate", nsManager)?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>())
                        {
                            if (invoice.DatiFattureCollegate == null)
                                invoice.DatiFattureCollegate = new List<FullDocumentItemBuying>();
                            FullDocumentItemBuying newRow = new FullDocumentItemBuying();
                            newRow.IdDocumento = bo.SelectSingleNode("IdDocumento")?.InnerText;
                            newRow.CodiceCommessaConvenzione = bo.SelectSingleNode("CodiceCommessaConvenzione")?.InnerText;
                            newRow.CodiceCUP = bo.SelectSingleNode("CodiceCUP")?.InnerText;
                            newRow.CodiceCIG = bo.SelectSingleNode("CodiceCIG")?.InnerText;
                            newRow.NumItem = bo.SelectSingleNode("NumItem")?.InnerText;
                            var dataBO = bo.SelectSingleNode("Data", nsManager);
                            newRow.Data = dataBO != null ? DateTime.Parse(dataBO.InnerText).ToString("d", CultureInfo.CreateSpecificCulture("it-IT")) : null;
                            foreach (XmlNode line in bo.SelectNodes(".//RiferimentoNumeroLinea", nsManager)?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>())
                            {
                                if (newRow.RiferimentoLinea == null)
                                    newRow.RiferimentoLinea = new List<long>();
                                newRow.RiferimentoLinea.Add(long.Parse(line.InnerText));
                            }
                            invoice.DatiFattureCollegate.Add(newRow);
                        }
                        #endregion
                        #region invoice rows [DettaglioLinee]
                        foreach (XmlNode row in node.SelectNodes(".//DatiBeniServizi/DettaglioLinee", nsManager)?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>())
                        {
                            if (invoice.RigheFattura == null)
                                invoice.RigheFattura = new List<FullDocumentItemRow>();
                            FullDocumentItemRow newRow = new FullDocumentItemRow();
                            newRow.Linea = !string.IsNullOrEmpty(row.SelectSingleNode("NumeroLinea")?.InnerText) ? long.Parse(row.SelectSingleNode("NumeroLinea")!.InnerText) : 0;
                            newRow.TipoCessionePrestazione = row.SelectSingleNode("TipoCessionePrestazione")?.InnerText;
                            newRow.RiferimentoAmministrazione = row.SelectSingleNode("RiferimentoAmministrazione")?.InnerText;
                            newRow.Ritenuta = row.SelectSingleNode("Ritenuta")?.InnerText;
                            newRow.Descrizione = row.SelectSingleNode("Descrizione")?.InnerText;
                            var qty = row.SelectSingleNode("Quantita");
                            newRow.Quantita = qty != null ? decimal.Parse(qty.InnerText, new CultureInfo("en-US")) : default(decimal?);
                            newRow.PrezzoUnitario = (!string.IsNullOrEmpty(row.SelectSingleNode("PrezzoUnitario")?.InnerText)) ?
                                decimal.Parse(row.SelectSingleNode("PrezzoUnitario")!.InnerText, new CultureInfo("en-US")) : 0;
                            newRow.AliquotaIVA = (!string.IsNullOrEmpty(row.SelectSingleNode("AliquotaIVA")?.InnerText)) ?
                                decimal.Parse(row.SelectSingleNode("AliquotaIVA")!.InnerText, new CultureInfo("en-US")) : 0;
                            newRow.PrezzoTotale = (!string.IsNullOrEmpty(row.SelectSingleNode("PrezzoTotale")?.InnerText)) ?
                                decimal.Parse(row.SelectSingleNode("PrezzoTotale")!.InnerText, new CultureInfo("en-US")) : 0;
                            newRow.UnitaMisura = row.SelectSingleNode("UnitaMisura")?.InnerText;
                            newRow.Natura = row.SelectSingleNode("Natura")?.InnerText;
                            var startDate = row.SelectSingleNode("DataInizioPeriodo");
                            newRow.DataInizioPeriodo = startDate != null ? DateTime.Parse(startDate.InnerText).ToString("d", CultureInfo.CreateSpecificCulture("it-IT")) : null;
                            var endDate = row.SelectSingleNode("DataFinePeriodo");
                            newRow.DataFinePeriodo = endDate != null ? DateTime.Parse(endDate.InnerText).ToString("d", CultureInfo.CreateSpecificCulture("it-IT")) : null;
                            #region [CodiceArticolo]
                            foreach (XmlNode pid in row.SelectNodes(".//CodiceArticolo", nsManager)?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>())
                            {
                                if (newRow.CodiciArticolo == null)
                                    newRow.CodiciArticolo = new List<FullDocumentItemRowProduct>();
                                FullDocumentItemRowProduct newRowPid = new FullDocumentItemRowProduct();
                                newRowPid.CodiceTipo = pid.SelectSingleNode("CodiceTipo", nsManager)?.InnerText.Trim();
                                newRowPid.CodiceValore = pid.SelectSingleNode("CodiceValore", nsManager)?.InnerText.Trim();
                                newRow.CodiciArticolo.Add(newRowPid);
                            }
                            #endregion
                            #region [ScontoMaggiorazione]

                            foreach (XmlNode dis in row.SelectNodes(".//ScontoMaggiorazione", nsManager)?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>())
                            {
                                if (newRow.ScontiMaggiorazioni == null)
                                    newRow.ScontiMaggiorazioni = new List<FullDocumentItemDiscount>();
                                FullDocumentItemDiscount newRowDis = new FullDocumentItemDiscount();
                                newRowDis.Tipo = dis.SelectSingleNode("Tipo", nsManager)?.InnerText;
                                var disValue = dis.SelectSingleNode("Importo", nsManager);
                                newRowDis.Importo = disValue != null ? decimal.Parse(disValue.InnerText, new CultureInfo("en-US")) : default(decimal?);
                                var disPercentage = dis.SelectSingleNode("Percentuale", nsManager);
                                newRowDis.Importo = disPercentage != null ? decimal.Parse(disPercentage.InnerText, new CultureInfo("en-US")) : default(decimal?);
                                newRow.ScontiMaggiorazioni.Add(newRowDis);
                            }

                            #endregion
                            #region [AltriDatiGestionali]
                            var nodesOtherData = row.SelectNodes(".//AltriDatiGestionali", nsManager);

                            if (nodesOtherData != null)
                            {
                                foreach (XmlNode oth in nodesOtherData)
                                {
                                    if (newRow.AltriDatiGestionali == null)
                                        newRow.AltriDatiGestionali = new List<FullDocumentItemRowOther>();
                                    FullDocumentItemRowOther newRowOth = new FullDocumentItemRowOther();
                                    newRowOth.TipoDato = oth.SelectSingleNode("TipoDato", nsManager)?.InnerText;
                                    newRowOth.RiferimentoTesto = oth.SelectSingleNode("RiferimentoTesto", nsManager)?.InnerText;
                                    var othNum = oth.SelectSingleNode("RiferimentoNumero", nsManager);
                                    newRowOth.RiferimentoNumero = othNum != null ? decimal.Parse(othNum.InnerText, new CultureInfo("en-US")) : default(decimal?);
                                    var othDate = oth.SelectSingleNode("RiferimentoData", nsManager);
                                    newRowOth.RiferimentoData = othDate != null ? DateTime.Parse(othDate.InnerText).ToString("d", CultureInfo.CreateSpecificCulture("it-IT")) : null;
                                    newRow.AltriDatiGestionali.Add(newRowOth);
                                }
                            }
                            #endregion
                            invoice.RigheFattura.Add(newRow);
                        }
                        #endregion
                        #region [DatiRiepilogo]
                        var nodesSummary = node.SelectNodes(".//DatiBeniServizi/DatiRiepilogo", nsManager);

                        if (nodesSummary != null)
                        {
                            foreach (XmlNode row in nodesSummary)
                            {
                                if (invoice.DatiRiepilogo == null)
                                    invoice.DatiRiepilogo = new List<FullDocumentItemRecap>();
                                FullDocumentItemRecap newRow = new FullDocumentItemRecap();
                                newRow.Aliquota = !string.IsNullOrEmpty(row.SelectSingleNode("AliquotaIVA", nsManager)?.InnerText) ?
                                    decimal.Parse(row.SelectSingleNode("AliquotaIVA", nsManager)!.InnerText, new CultureInfo("en-US")) : 0;
                                var round = row.SelectSingleNode("Arrotondamento", nsManager);
                                newRow.Arrotondamento = round != null ? decimal.Parse(round.InnerText, new CultureInfo("en-US")) : default(decimal?);
                                newRow.Imponibile = !string.IsNullOrEmpty(row.SelectSingleNode("ImponibileImporto", nsManager)?.InnerText) ?
                                    decimal.Parse(row.SelectSingleNode("ImponibileImporto", nsManager)!.InnerText, new CultureInfo("en-US")) : 0;
                                newRow.Imposta = !string.IsNullOrEmpty(row.SelectSingleNode("Imposta", nsManager)?.InnerText) ?
                                    decimal.Parse(row.SelectSingleNode("Imposta", nsManager)!.InnerText, new CultureInfo("en-US")) : 0;
                                newRow.Esigibilita = row.SelectSingleNode("EsigibilitaIVA")?.InnerText;
                                newRow.RiferimentoNormativo = row.SelectSingleNode("RiferimentoNormativo")?.InnerText;
                                newRow.Natura = row.SelectSingleNode("Natura")?.InnerText;
                                invoice.DatiRiepilogo.Add(newRow);
                            }
                        }
                        #endregion
                        #region [DatiPagamento]
                        var nodesPayment = node.SelectNodes(".//DatiPagamento", nsManager);

                        if (nodesPayment != null)
                        {
                            foreach (XmlNode row in nodesPayment)
                            {
                                if (invoice.Pagamenti == null)
                                    invoice.Pagamenti = new List<FullDocumentItemPayment>();
                                FullDocumentItemPayment newRow = new FullDocumentItemPayment();
                                newRow.Condizioni = row.SelectSingleNode("CondizioniPagamento", nsManager)?.InnerText;

                                var nodesDetailPayment = row.SelectNodes(".//DettaglioPagamento", nsManager);

                                if (nodesDetailPayment != null)
                                {
                                    foreach (XmlNode detNode in nodesDetailPayment)
                                    {
                                        if (newRow.DettagliPagamento == null)
                                            newRow.DettagliPagamento = new List<FullDocumentItemPaymentDetail>();
                                        FullDocumentItemPaymentDetail newDetail = new FullDocumentItemPaymentDetail();
                                        newDetail.Beneficiario = detNode.SelectSingleNode("Beneficiario", nsManager)?.InnerText;
                                        newDetail.Modalita = detNode.SelectSingleNode("ModalitaPagamento", nsManager)?.InnerText;
                                        var rtpDate = detNode.SelectSingleNode("DataRiferimentoTerminiPagamento", nsManager);
                                        newDetail.DataRiferimentoTerminiPagamento = rtpDate != null ? DateTime.Parse(rtpDate.InnerText).ToString("d", CultureInfo.CreateSpecificCulture("it-IT")) : null;
                                        var paymentDays = detNode.SelectSingleNode("GiorniTerminiPagamento", nsManager);
                                        newDetail.GiorniTerminiPagamento = paymentDays != null ? int.Parse(paymentDays.InnerText, new CultureInfo("en-US")) : default(int?);
                                        var expireDate = detNode.SelectSingleNode("DataScadenzaPagamento", nsManager);
                                        newDetail.DataScadenza = expireDate != null ? DateTime.Parse(expireDate.InnerText).ToString("d", CultureInfo.CreateSpecificCulture("it-IT")) : null;
                                        var paymentValue = detNode.SelectSingleNode("ImportoPagamento", nsManager);
                                        newDetail.Importo = paymentValue != null ? decimal.Parse(paymentValue.InnerText, new CultureInfo("en-US")) : default(decimal?);
                                        newDetail.CodiceUfficioPostale = detNode.SelectSingleNode("CodUfficioPostale", nsManager)?.InnerText;
                                        newDetail.CognomeQuietanzante = detNode.SelectSingleNode("CognomeQuietanzante", nsManager)?.InnerText;
                                        newDetail.NomeQuietanzante = detNode.SelectSingleNode("NomeQuietanzante", nsManager)?.InnerText;
                                        newDetail.CFQuietanzante = detNode.SelectSingleNode("CFQuietanzante", nsManager)?.InnerText;
                                        newDetail.TitoloQuietanzante = detNode.SelectSingleNode("TitoloQuietanzante", nsManager)?.InnerText;
                                        newDetail.IstitutoFinanziario = detNode.SelectSingleNode("IstitutoFinanziario", nsManager)?.InnerText;
                                        newDetail.IBAN = detNode.SelectSingleNode("IBAN", nsManager)?.InnerText;
                                        newDetail.ABI = detNode.SelectSingleNode("ABI", nsManager)?.InnerText;
                                        newDetail.CAB = detNode.SelectSingleNode("CAB", nsManager)?.InnerText;
                                        newDetail.BIC = detNode.SelectSingleNode("BIC", nsManager)?.InnerText;
                                        var discount = detNode.SelectSingleNode("ScontoPagamentoAnticipato", nsManager);
                                        newDetail.ScontoPagamentoAnticipato = discount != null ? decimal.Parse(discount.InnerText, new CultureInfo("en-US")) : default(decimal?);
                                        var dlpaDate = detNode.SelectSingleNode("DataLimitePagamentoAnticipato", nsManager);
                                        newDetail.DataLimitePagamentoAnticipato = dlpaDate != null ? DateTime.Parse(dlpaDate.InnerText).ToString("d", CultureInfo.CreateSpecificCulture("it-IT")) : null;
                                        var penalty = detNode.SelectSingleNode("PenalitaPagamentiRitardati", nsManager);
                                        newDetail.PenalitaPagamentiRitardati = penalty != null ? decimal.Parse(penalty.InnerText, new CultureInfo("en-US")) : default(decimal?);
                                        var ddpDate = detNode.SelectSingleNode("DataDecorrenzaPenale", nsManager);
                                        newDetail.DataDecorrenzaPenale = ddpDate != null ? DateTime.Parse(ddpDate.InnerText).ToString("d", CultureInfo.CreateSpecificCulture("it-IT")) : null;
                                        newDetail.CodicePagamento = detNode.SelectSingleNode("CodicePagamento", nsManager)?.InnerText;
                                        newRow.DettagliPagamento.Add(newDetail);
                                    }
                                }
                                invoice.Pagamenti.Add(newRow);
                            }
                        }
                        #endregion
                        #region [ScontoMaggiorazione]
                        var nodesDiscount = node.SelectNodes(".//DatiGenerali/DatiGeneraliDocumento/ScontoMaggiorazione", nsManager);

                        if (nodesDiscount != null)
                        {
                            foreach (XmlNode row in nodesDiscount)
                            {
                                if (invoice.ScontiMaggiorazioni == null)
                                    invoice.ScontiMaggiorazioni = new List<FullDocumentItemDiscount>();
                                FullDocumentItemDiscount newRow = new FullDocumentItemDiscount();
                                newRow.Tipo = row.SelectSingleNode("Tipo", nsManager)?.InnerText;
                                var disValue = row.SelectSingleNode("Importo", nsManager);
                                newRow.Importo = disValue != null ? decimal.Parse(disValue.InnerText, new CultureInfo("en-US")) : default(decimal?);
                                var disPercentage = row.SelectSingleNode("Percentuale", nsManager);
                                newRow.Percentuale = disPercentage != null ? decimal.Parse(disPercentage.InnerText, new CultureInfo("en-US")) : default(decimal?);
                                invoice.ScontiMaggiorazioni.Add(newRow);
                            }
                        }
                        #endregion
                        #region [DatiRitenuta]
                        var nodesRetain = node.SelectNodes(".//DatiGenerali/DatiGeneraliDocumento/DatiRitenuta", nsManager);

                        if (nodesRetain != null)
                        {
                            foreach (XmlNode row in nodesRetain)
                            {
                                if (invoice.DatiRitenuta == null)
                                    invoice.DatiRitenuta = new List<FullDocumentItemRitenuta>();
                                var newRow = new FullDocumentItemRitenuta();
                                newRow.Tipo = row.SelectSingleNode("TipoRitenuta", nsManager)?.InnerText;
                                var importo = row.SelectSingleNode("ImportoRitenuta", nsManager);
                                newRow.Importo = importo != null ? decimal.Parse(importo.InnerText, new CultureInfo("en-US")) : 0;
                                var aliquota = row.SelectSingleNode("AliquotaRitenuta", nsManager);
                                newRow.Aliquota = aliquota != null ? decimal.Parse(aliquota.InnerText, new CultureInfo("en-US")) : 0;
                                newRow.CausalePagamento = row.SelectSingleNode("CausalePagamento", nsManager)?.InnerText;
                                invoice.DatiRitenuta.Add(newRow);
                            }
                        }
                        #endregion
                        #region [DatiCassaPrevidenziale]
                        var nodesPrevidential = node.SelectNodes(".//DatiGenerali/DatiGeneraliDocumento/DatiCassaPrevidenziale", nsManager);

                        if (nodesPrevidential != null)
                        {
                            foreach (XmlNode row in nodesPrevidential)
                            {
                                if (invoice.DatiCassaPrevidenziale == null)
                                    invoice.DatiCassaPrevidenziale = new List<FullDocumentItemPensionFund>();
                                FullDocumentItemPensionFund newRow = new FullDocumentItemPensionFund();
                                newRow.TipoCassa = row.SelectSingleNode("TipoCassa", nsManager)?.InnerText;
                                var alCassa = row.SelectSingleNode("AlCassa", nsManager);
                                newRow.AlCassa = alCassa != null ? decimal.Parse(alCassa.InnerText, new CultureInfo("en-US")) : default(decimal?);
                                var impoCC = row.SelectSingleNode("ImportoContributoCassa", nsManager);
                                newRow.ImportoContributoCassa = impoCC != null ? decimal.Parse(impoCC.InnerText, new CultureInfo("en-US")) : default(decimal?);
                                var impoCa = row.SelectSingleNode("ImponibileCassa", nsManager);
                                newRow.ImponibileCassa = impoCa != null ? decimal.Parse(impoCa.InnerText, new CultureInfo("en-US")) : default(decimal?);
                                var aliqIVA = row.SelectSingleNode("AliquotaIVA", nsManager);
                                newRow.AliquotaIVA = aliqIVA != null ? decimal.Parse(aliqIVA.InnerText, new CultureInfo("en-US")) : default(decimal?);
                                newRow.Ritenuta = row.SelectSingleNode("Ritenuta", nsManager)?.InnerText;
                                newRow.Natura = row.SelectSingleNode("Natura", nsManager)?.InnerText;
                                newRow.RiferimentoAmministrazione = row.SelectSingleNode("RiferimentoAmministrazione", nsManager)?.InnerText;
                                invoice.DatiCassaPrevidenziale.Add(newRow);
                            }
                        }
                        #endregion
                        document.Fatture.Add(invoice);
                    }
                }
                #endregion
                #endregion
            }
            else
            {
                #region Semplificata
                document.TipoFileXML = "S";
                nsManager.AddNamespace("ps", "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.0");
                #region header [FatturaElettronicaHeader]
                document.PaeseTrasmittente = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/DatiTrasmissione/IdTrasmittente/IdPaese", nsManager)?.InnerText;
                document.PartitaIvaTrasmittente = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/DatiTrasmissione/IdTrasmittente/IdCodice", nsManager)?.InnerText;
                document.ProgressivoInvio = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/DatiTrasmissione/ProgressivoInvio", nsManager)?.InnerText;
                document.FormatoTrasmissione = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/DatiTrasmissione/FormatoTrasmissione", nsManager)?.InnerText;
                document.CodiceDestinatario = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/DatiTrasmissione/CodiceDestinatario", nsManager)?.InnerText;
                #region [CedentePrestatore]
                document.CedentePrestatoreCodiceFiscale = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/CodiceFiscale", nsManager)?.InnerText;
                document.CedentePrestatorePaese = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/IdFiscaleIVA/IdPaese", nsManager)?.InnerText;
                document.CedentePrestatorePartitaIVA = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/IdFiscaleIVA/IdCodice", nsManager)?.InnerText;
                document.CedentePrestatoreDenominazione = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/Denominazione", nsManager)?.InnerText;
                document.CedenteNome = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/Nome", nsManager)?.InnerText;
                document.CedenteCognome = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/Cognome", nsManager)?.InnerText;
                document.CedentePrestatoreRegimeFiscale = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/RegimeFiscale", nsManager)?.InnerText;
                document.CedentePrestatoreSedeIndirizzo = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/Sede/Indirizzo", nsManager)?.InnerText;
                document.CedentePrestatoreSedeCap = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/Sede/CAP", nsManager)?.InnerText;
                document.CedentePrestatoreSedeNumeroCivico = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/Sede/NumeroCivico", nsManager)?.InnerText;
                document.CedentePrestatoreSedeComune = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/Sede/Comune", nsManager)?.InnerText;
                document.CedentePrestatoreSedeProvincia = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/Sede/Provincia", nsManager)?.InnerText;
                document.CedentePrestatoreSedeNazione = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/Sede/Nazione", nsManager)?.InnerText;
                document.CedentePrestatoreREAUfficio = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/IscrizioneREA/Ufficio", nsManager)?.InnerText;
                document.CedentePrestatoreREANumero = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/IscrizioneREA/NumeroREA", nsManager)?.InnerText;
                var reaCapital = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/IscrizioneREA/CapitaleSociale", nsManager);
                document.CedentePrestatoreREACapitaleSociale = reaCapital != null ? decimal.Parse(reaCapital.InnerText, new CultureInfo("en-US")) : default(decimal?);
                document.CedentePrestatoreREASocioUnico = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/IscrizioneREA/SocioUnico", nsManager)?.InnerText;
                document.CedentePrestatoreREAStatoLiquidazione = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/IscrizioneREA/StatoLiquidazione", nsManager)?.InnerText;
                // StabileOrganizzazione
                document.CedenteSOIndirizzo = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/StabileOrganizzazione/Indirizzo", nsManager)?.InnerText;
                document.CedenteSONumeroCivico = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/StabileOrganizzazione/NumeroCivico", nsManager)?.InnerText;
                document.CedenteSOCAP = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/StabileOrganizzazione/CAP", nsManager)?.InnerText;
                document.CedenteSOComune = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/StabileOrganizzazione/Comune", nsManager)?.InnerText;
                document.CedenteSOProvincia = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/StabileOrganizzazione/Provincia", nsManager)?.InnerText;
                document.CedenteSONazione = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/StabileOrganizzazione/Nazione", nsManager)?.InnerText;
                // RappresentanteFiscale
                document.CedenteRFIDPaese = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/RappresentanteFiscale/IdFiscaleIVA/IdPaese", nsManager)?.InnerText;
                document.CedenteRFIDCodice = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/RappresentanteFiscale/IdFiscaleIVA/IdCodice", nsManager)?.InnerText;
                document.CedenteRFDenominazione = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/RappresentanteFiscale/Denominazione", nsManager)?.InnerText;
                document.CedenteRFNome = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/RappresentanteFiscale/Nome", nsManager)?.InnerText;
                document.CedenteRFCognome = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CedentePrestatore/RappresentanteFiscale/Cognome", nsManager)?.InnerText;
                #endregion
                #region [CessionarioCommittente]
                document.CessionarioCommittenteCodiceFiscale = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/IdentificativiFiscali/CodiceFiscale", nsManager)?.InnerText;
                document.CessionarioCommittentePaese = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/IdentificativiFiscali/IdFiscaleIVA/IdPaese", nsManager)?.InnerText;
                document.CessionarioCommittentePartitaIVA = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/IdentificativiFiscali/IdFiscaleIVA/IdCodice", nsManager)?.InnerText;

                document.CessionarioCommittenteDenominazione = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/AltriDatiIdentificativi/Denominazione", nsManager)?.InnerText;
                document.CessionarioCommittenteNome = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/AltriDatiIdentificativi/Nome", nsManager)?.InnerText;
                document.CessionarioCommittenteCognome = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/AltriDatiIdentificativi/Cognome", nsManager)?.InnerText;
                // Sede
                document.CessionarioCommittenteSedeIndirizzo = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/AltriDatiIdentificativi/Sede/Indirizzo", nsManager)?.InnerText;
                document.CessionarioCommittenteSedeCap = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/AltriDatiIdentificativi/Sede/CAP", nsManager)?.InnerText;
                document.CessionarioCommittenteSedeNumeroCivico = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/AltriDatiIdentificativi/Sede/NumeroCivico", nsManager)?.InnerText;
                document.CessionarioCommittenteSedeComune = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/AltriDatiIdentificativi/Sede/Comune", nsManager)?.InnerText;
                document.CessionarioCommittenteSedeProvincia = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/AltriDatiIdentificativi/Sede/Provincia", nsManager)?.InnerText;
                document.CessionarioCommittenteSedeNazione = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/AltriDatiIdentificativi/Sede/Nazione", nsManager)?.InnerText;
                // Sede
                document.CessionarioCommittenteSOIndirizzo = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/AltriDatiIdentificativi/StabileOrganizzazione/Indirizzo", nsManager)?.InnerText;
                document.CessionarioCommittenteSOCap = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/AltriDatiIdentificativi/StabileOrganizzazione/CAP", nsManager)?.InnerText;
                document.CessionarioCommittenteSONumeroCivico = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/AltriDatiIdentificativi/StabileOrganizzazione/NumeroCivico", nsManager)?.InnerText;
                document.CessionarioCommittenteSOComune = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/AltriDatiIdentificativi/StabileOrganizzazione/Comune", nsManager)?.InnerText;
                document.CessionarioCommittenteSOProvincia = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/AltriDatiIdentificativi/StabileOrganizzazione/Provincia", nsManager)?.InnerText;
                document.CessionarioCommittenteSONazione = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/AltriDatiIdentificativi/StabileOrganizzazione/Nazione", nsManager)?.InnerText;
                // RappresentanteFiscale
                document.CessionarioRFIDPaese = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/AltriDatiIdentificativi/RappresentanteFiscale/IdFiscaleIVA/IdPaese", nsManager)?.InnerText;
                document.CessionarioRFIDCodice = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/AltriDatiIdentificativi/RappresentanteFiscale/IdFiscaleIVA/IdCodice", nsManager)?.InnerText;
                document.CessionarioRFDenominazione = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/AltriDatiIdentificativi/RappresentanteFiscale/Denominazione", nsManager)?.InnerText;
                document.CessionarioRFNome = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/AltriDatiIdentificativi/RappresentanteFiscale/Nome", nsManager)?.InnerText;
                document.CessionarioRFCognome = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/CessionarioCommittente/AltriDatiIdentificativi/RappresentanteFiscale/Cognome", nsManager)?.InnerText;
                #endregion
                var emitter = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/SoggettoEmittente", nsManager);
                document.SoggettoEmittente = emitter != null ? emitter.InnerText : null;
                if (document.CodiceDestinatario == "000000" || document.CodiceDestinatario == "0000000")
                    document.PECDestinatario = Document.SelectSingleNode("ps:FatturaElettronicaSemplificata/FatturaElettronicaHeader/DatiTrasmissione/PECDestinatario", nsManager)?.InnerText;
                #endregion
                #region invoices [FatturaElettronicaBody]
                var nodes = Document.SelectNodes("ps:FatturaElettronicaSemplificata/FatturaElettronicaBody", nsManager);
                if (nodes != null)
                {
                    foreach (XmlNode node in nodes)
                    {
                        FullDocumentItem invoice = new FullDocumentItem();
                        // DatiGenerali
                        invoice.Tipo = node.SelectSingleNode("DatiGenerali/DatiGeneraliDocumento/TipoDocumento", nsManager)?.InnerText;
                        invoice.Divisa = node.SelectSingleNode("DatiGenerali/DatiGeneraliDocumento/Divisa", nsManager)?.InnerText;
                        invoice.Data = (!string.IsNullOrEmpty(node.SelectSingleNode("DatiGenerali/DatiGeneraliDocumento/Data", nsManager)?.InnerText)) ?
                            DateTime.Parse(node.SelectSingleNode("DatiGenerali/DatiGeneraliDocumento/Data", nsManager)!.InnerText).ToString("d", CultureInfo.CreateSpecificCulture("it-IT")) : null;
                        invoice.Numero = node.SelectSingleNode("DatiGenerali/DatiGeneraliDocumento/Numero", nsManager)?.InnerText;
                        invoice.BolloVirtuale = node.SelectSingleNode("DatiGenerali/DatiGeneraliDocumento/BolloVirtuale", nsManager)?.InnerText;
                        // DatiFatturaRettificata
                        invoice.NumeroFR = node.SelectSingleNode("DatiGenerali/DatiFatturaRettificata/NumeroFR", nsManager)?.InnerText;
                        var dataFR = node.SelectSingleNode("DatiGenerali/DatiFatturaRettificata/DataFR", nsManager);
                        invoice.DataFR = dataFR != null ? DateTime.Parse(dataFR.InnerText).ToString("d", CultureInfo.CreateSpecificCulture("it-IT")) : null;
                        invoice.ElementiRettificati = node.SelectSingleNode("DatiGenerali/DatiFatturaRettificata/ElementiRettificati", nsManager)?.InnerText;
                        #region DatiBeniServizi
                        var nodesGoods = node.SelectNodes(".//DatiBeniServizi", nsManager);

                        if (nodesGoods != null)
                        {
                            foreach (XmlNode row in nodesGoods)
                            {
                                FullDocumentItemRow newRow = new FullDocumentItemRow();
                                newRow.Descrizione = row.SelectSingleNode("Descrizione")?.InnerText;
                                var rowVal = row.SelectSingleNode("Importo", nsManager);
                                newRow.Importo = rowVal != null ? decimal.Parse(rowVal.InnerText, new CultureInfo("en-US")) : default(decimal?);
                                var impo = row.SelectSingleNode("DatiIVA/Imposta", nsManager);
                                newRow.DatiIVAImposta = impo != null ? decimal.Parse(impo.InnerText, new CultureInfo("en-US")) : default(decimal?);
                                var aliq = row.SelectSingleNode("DatiIVA/Aliquota", nsManager);
                                newRow.DatiIVAAliquota = aliq != null ? decimal.Parse(aliq.InnerText, new CultureInfo("en-US")) : default(decimal?);
                                newRow.Natura = row.SelectSingleNode("Natura")?.InnerText;
                                newRow.RiferimentoNormativo = row.SelectSingleNode("RiferimentoNormativo")?.InnerText;
                                if (invoice.RigheFattura == null)
                                    invoice.RigheFattura = new List<FullDocumentItemRow>();
                                invoice.RigheFattura.Add(newRow);
                            }
                        }
                        #endregion
                        document.Fatture.Add(invoice);
                    }
                }
                #endregion
                #endregion
            }
            return document;
        }

        public static List<DocumentAttachment> GetAttachments(XmlDocument Document)
        {
            XmlNamespaceManager nsManager = new XmlNamespaceManager(Document.NameTable);
            nsManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            nsManager.AddNamespace("p", "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2");
            nsManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            List<DocumentAttachment> retValue = new List<DocumentAttachment>();
            var nodes = Document.SelectNodes("p:FatturaElettronica/FatturaElettronicaBody/Allegati", nsManager);

            if (nodes != null)
            {
                foreach (XmlNode att in nodes)
                {
                    string? attachment = att.SelectSingleNode("Attachment", nsManager)?.InnerText;
                    byte[]? data = null;

                    if (!string.IsNullOrEmpty(attachment))
                        data = Convert.FromBase64String(attachment);

                    DocumentAttachment newAttachment = new DocumentAttachment()
                    {
                        Name = att.SelectSingleNode("NomeAttachment", nsManager)?.InnerText,
                        Compression = att.SelectSingleNode("AlgoritmoCompressione", nsManager)?.InnerText,
                        Format = att.SelectSingleNode("FormatoAttachment", nsManager)?.InnerText,
                        Description = att.SelectSingleNode("DescrizioneAttachment", nsManager)?.InnerText,
                        Data = data,
                        Size = data?.Length ?? 0
                    };
                    retValue.Add(newAttachment);
                }
            }

            return retValue;
        }
    }
}
