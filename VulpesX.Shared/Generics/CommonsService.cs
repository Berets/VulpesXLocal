using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared.Generics
{
    public static class CommonsService
    {
        #region Standard types
        public static ObservableCollection<GenericIDDescription> StandardValueTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(null, "Nessuno") ,
            new GenericIDDescription("P", "Percentuale"),
            new GenericIDDescription("V", "Valore")
        };
        public static ObservableCollection<GenericIDDescription> PercentageValueTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription("P", "Percentuale"),
        };

        public static ObservableCollection<GenericIDDescription> StandardQuantityTypesWithStamp => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "V", Description = "Vendita" },
            new GenericIDDescription(){ ID = "T", Description = "Sconto merce" },
            new GenericIDDescription(){ ID = "N", Description = "No vendita" },
            new GenericIDDescription(){ ID = "B", Description = "Bollo" },
            new GenericIDDescription(){ ID = "O", Description = "Omaggio" }
        };
        public static ObservableCollection<GenericIDDescription> StandardQuantityTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "V", Description = "Vendita" },
            new GenericIDDescription(){ ID = "T", Description = "Sconto merce" },
            new GenericIDDescription(){ ID = "N", Description = "No vendita" },
            new GenericIDDescription(){ ID = "O", Description = "Omaggio" }
        };

        public static ObservableCollection<GenericIDDescription> StandardPriceTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "U", Description = "Unitario" },
            new GenericIDDescription(){ ID = "V", Description = "Valore" }
        };

        public static ObservableCollection<GenericIDDescription> StandardAccountingSigns => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "D", Description = "Dare" },
            new GenericIDDescription(){ ID = "A", Description = "Avere" }
        };

        public static ObservableCollection<GenericIDDescription> StandardAccountingSignsNone => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "D", Description = "Dare" },
            new GenericIDDescription(){ ID = "A", Description = "Avere" },
            new GenericIDDescription(){ ID = null, Description = "Nessuno" }
        };
        public static ObservableCollection<GenericIDDescription> BaseSigns => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "+", Description = "+" },
            new GenericIDDescription(){ ID = "-", Description = "-" }
        };
        public static ObservableCollection<GenericIDDescription> StandardIVASigns => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "+", Description = "+" },
            new GenericIDDescription(){ ID = "-", Description = "-" },
            new GenericIDDescription(){ ID = null, Description = "Nessuno" }
        };
        public static ObservableCollection<GenericIDDescription> StandardFulfillments => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "S", Description = "Saldo" },
            new GenericIDDescription(){ ID = "A", Description = "Acconto" }
        };
        public static ObservableCollection<GenericIDDescription> SingleEntityTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "C", Description = "Cliente" },
            new GenericIDDescription(){ ID = "F", Description = "Fornitore" }
        };
        public static ObservableCollection<GenericIDDescription> EntityTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "C", Description = "Clienti" },
            new GenericIDDescription(){ ID = "F", Description = "Fornitori" }
        };

        public static ObservableCollection<GenericIDDescription> RecipientTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "F", Description = "Destinatario fisso" },
            new GenericIDDescription(){ ID = "N", Description = "Destinatario occasionale" }
        };
        #endregion

        #region Accounting
        public static ObservableCollection<GenericIDDescription> SupportTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "D", Description = "Rimessa diretta" },
            new GenericIDDescription(){ ID = "R", Description = "Ri.Ba." },
            new GenericIDDescription(){ ID = "T", Description = "R.i.d." }
        };
        public static ObservableCollection<GenericIDDescription> SupportTypesUfp => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "T", Description = "Richiesta" },
            new GenericIDDescription(){ ID = "R", Description = "Ri.Ba." },
            new GenericIDDescription(){ ID = "N", Description = "Nessuno" }
        };

        public static ObservableCollection<GenericIDDescription> WalletStatus => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "N", Description = "Non estratto" },
            new GenericIDDescription(){ ID = "E", Description = "Estratto non contabilizzato" },
            new GenericIDDescription(){ ID = "C", Description = "Contabilizzato" },
            new GenericIDDescription(){ ID = "S", Description = "Scartato" },
            new GenericIDDescription(){ ID = "*", Description = "Tutti" }
        };
        public static ObservableCollection<GenericIDDescription> WalletStatusUfp => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "N", Description = "Non estratto" },
            new GenericIDDescription(){ ID = "E", Description = "Estratto non contabilizzato" },
            new GenericIDDescription(){ ID = "C", Description = "Contabilizzato" },
            new GenericIDDescription(){ ID = "*", Description = "Tutti" }
        };

        public static ObservableCollection<GenericIDDescription> IVABookTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "A", Description = "Acquisti" },
            new GenericIDDescription(){ ID = "V", Description = "Vendite" }
        };
        public static ObservableCollection<GenericIDDescription> IVABookTypesUfp => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "AI", Description = "Acquisti Italia" },
            new GenericIDDescription(){ ID = "AE", Description = "Acquisti Estero" },
            new GenericIDDescription(){ ID = "VI", Description = "Vendite Italia" },
            new GenericIDDescription(){ ID = "VE", Description = "Vendite Estero" },
        };
        public static ObservableCollection<GenericIDDescription> CostCentersTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "Q", Description = "Quantità" },
            new GenericIDDescription(){ ID = "V", Description = "Valore" }
        };
        public static ObservableCollection<GenericIDDescription> RateClassifications = new ObservableCollection<GenericIDDescription>() {
                    new GenericIDDescription { ID = "I", Description = "Imponibile + IVA" },
                    new GenericIDDescription { ID = "N", Description = "Non imponibile" },
                    new GenericIDDescription { ID = "E", Description = "Esente" },
                    new GenericIDDescription { ID = "S", Description = "Non soggetto" },
                    new GenericIDDescription { ID = null, Description = "Nessuna" }};
        public static ObservableCollection<GenericIDDescription> LIPEComputeTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "T", Description = "Testata" },
            new GenericIDDescription(){ ID = "D", Description = "Debito" },
            new GenericIDDescription(){ ID = "C", Description = "Credito" },
            new GenericIDDescription(){ ID = "E", Description = "Calcolo" }
        };
        public static ObservableCollection<GenericIDDescription> LIPELiquidationTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "M", Description = "Mensile" },
            new GenericIDDescription(){ ID = "T", Description = "Trimestrale" }
        };
        public static ObservableCollection<GenericIDDescription> LIPEQuarters => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "I", Description = "Primo trimestre" },
            new GenericIDDescription(){ ID = "II", Description = "Secondo trimestre" },
            new GenericIDDescription(){ ID = "III", Description = "Terzo trimestre" },
            new GenericIDDescription(){ ID = "IV", Description = "Quarto trimestre" }
        };

        public static ObservableCollection<GenericIDDescription> LIPETitles => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "1", Description = "Rappresentante legale, negoziale o di fatto, socio amministratore" },
            new GenericIDDescription(){ ID = "2", Description = "Rappresentante di minore, inabilitato o interdetto, ovvero curatore dell’eredità giacente, amministratore di eredità devoluta sotto condizione\r\nsospensiva o in favore di nascituro non ancora concepito, amministratore di sostegno per le persone con limitata capacità di agire" },
            new GenericIDDescription(){ ID = "3", Description = "Curatore fallimentare" },
            new GenericIDDescription(){ ID = "4", Description = "Commissario liquidatore (liquidazione coatta amministrativa ovvero amministrazione straordinaria)"},
            new GenericIDDescription(){ ID = "5", Description = "Commissario giudiziale (amministrazione controllata) ovvero custode giudiziario (custodia giudiziaria), ovvero amministratore giudiziario in\r\nqualità di rappresentante dei beni sequestrat" },
            new GenericIDDescription(){ ID = "6", Description = "Rappresentante fiscale di soggetto non residente" },
            new GenericIDDescription(){ ID = "7", Description = "Erede" },
            new GenericIDDescription(){ ID = "8", Description = "Liquidatore (liquidazione volontaria)" },
            new GenericIDDescription(){ ID = "9", Description = "Soggetto tenuto a presentare la dichiarazione ai fini IVA per conto del soggetto estinto a seguito di operazioni straordinarie o altre trasforma-\r\nzioni sostanziali soggettive (cessionario d’azienda, società beneficiaria, incorporante, conferitaria, ecc.); ovvero, ai fini delle imposte sui redditi\r\ne/o dell’IRAP, rappresentante della società beneficiaria (scissione) o della società risultante dalla fusione o incorporazione" },
            new GenericIDDescription(){ ID = "10", Description = "Rappresentante fiscale di soggetto non residente con le limitazioni di cui all’art. 44, comma 3, del D.L. n. 331/1993" },
            new GenericIDDescription(){ ID = "11", Description = "Soggetto esercente l’attività tutoria del minore o interdetto in relazione alla funzione istituzionale rivestita" },
            new GenericIDDescription(){ ID = "12", Description = "Liquidatore (liquidazione volontaria di ditta individuale - periodo ante messa in liquidazione)" },
            new GenericIDDescription(){ ID = "13", Description = "Amministratore di condominio" },
            new GenericIDDescription(){ ID = "14", Description = "Soggetto che sottoscrive la dichiarazione per conto di una pubblica amministrazione" },
            new GenericIDDescription(){ ID = "15", Description = "Commissario liquidatore di una pubblica amministrazione" }
        };
        public static ObservableCollection<GenericIDDescription> LIPEPresentations => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "1", Description = "Comunicazione predisposta dal contribuente" },
            new GenericIDDescription(){ ID = "2", Description = "Comunicazione predisposta da chi effettua l'invio" }
        };


        #endregion

        #region eInvoice
        public static ObservableCollection<GenericIDDescription> FEPaymentConditions => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "TP01", Description = "Pagamento a rate" },
            new GenericIDDescription(){ ID = "TP02", Description = "Pagamento completo" },
            new GenericIDDescription(){ ID = "TP03", Description = "Anticipo" }
        };
        public static ObservableCollection<GenericIDDescription> FECollectabilities => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "I", Description = "IVA ad esigibilità immediata" },
            new GenericIDDescription(){ ID = "D", Description = "IVA ad esigibilità differita" },
            new GenericIDDescription(){ ID = "S", Description = "Scissione dei pagamenti" }
        };
        public static ObservableCollection<GenericIDDescription> FESMTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "SC", Description = "Sconto" },
            new GenericIDDescription(){ ID = "MG", Description = "Maggiorazione" }
        };
        public static ObservableCollection<GenericIDDescription> ShareholderTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "SU", Description = "Socio unico" },
            new GenericIDDescription(){ ID = "SM", Description = "Più soci" }
        };
        public static ObservableCollection<GenericIDDescription> LiquidationStatuses => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "LS", Description = "In liquidazione" },
            new GenericIDDescription(){ ID = "LN", Description = "Non in liquidazione" }
        };
        #endregion

        #region CRM
        public static ObservableCollection<GenericIDDescription> OfferStatuses => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "A", Description = "In corso da inviare a firma" },
            new GenericIDDescription(){ ID = "I", Description = "In corso in attesa di firma tecnica" },
            new GenericIDDescription(){ ID = "T", Description = "In corso in attesa di firma commerciale" },
            new GenericIDDescription(){ ID = "F", Description = "In corso firmate" },
            new GenericIDDescription(){ ID = "C", Description = "Chiuse senza ordine" },
            new GenericIDDescription(){ ID = "O", Description = "Chiuse con ordine" },
            new GenericIDDescription(){ ID = "X", Description = "Annullate" },
            new GenericIDDescription(){ ID = "*", Description = "Tutte" }
            };
        public static ObservableCollection<GenericIDDescription> OrderStatuses => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "A", Description = "In corso da inviare a firma" },
            new GenericIDDescription(){ ID = "I", Description = "In corso in attesa di firma tecnica" },
            new GenericIDDescription(){ ID = "T", Description = "In corso in attesa di firma commerciale" },
            new GenericIDDescription(){ ID = "F", Description = "In corso firmati" },
            new GenericIDDescription(){ ID = "E", Description = "Evasi" },
            new GenericIDDescription(){ ID = "X", Description = "Annullati" },
            new GenericIDDescription(){ ID = "*", Description = "Tutti" }
            };
        public static ObservableCollection<GenericIDDescription> FeasabilityStatuses => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = null, Description = "Aperte" },
            new GenericIDDescription(){ ID = "A", Description = "Archiviate" },
            new GenericIDDescription(){ ID = "X", Description = "Annullate" },
            new GenericIDDescription(){ ID = "*", Description = "Tutte" }
            };
        public static ObservableCollection<GenericIDDescription> MarginTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "M", Description = "Margine" },
            new GenericIDDescription(){ ID = "U", Description = "Mark-up" }
            };
        public static ObservableCollection<GenericIntIDDescription> StampTypes => new ObservableCollection<GenericIntIDDescription>() {
            new GenericIntIDDescription(){ ID = 1, Description = "Bollo addebitato" },
            new GenericIntIDDescription(){ ID = 2, Description = "Bollo non addebitato" },
            new GenericIntIDDescription(){ ID = null, Description = "Non bollo" }
            };
        public static ObservableCollection<GenericIDDescription> PriceListFilters => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "V", Description = "Solo validi" },
            new GenericIDDescription(){ ID = "S", Description = "Solo scaduti" },
            new GenericIDDescription(){ ID = "*", Description = "Tutti" }
            };
        public static ObservableCollection<GenericIDDescription> InvoiceTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "F", Description = "Fattura" },
            new GenericIDDescription(){ ID = "N", Description = "Nota credito" },
            new GenericIDDescription(){ ID = "A", Description = "Autofattura" },
            new GenericIDDescription(){ ID = "C", Description = "Autonotacredito" },
            new GenericIDDescription(){ ID = "P", Description = "Parcella" },
            new GenericIDDescription(){ ID = "B", Description = "Nota debito" },
        };

        public static ObservableCollection<GenericIDDescription> InvoiceTypesUfp => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "F", Description = "Fattura" },
            new GenericIDDescription(){ ID = "N", Description = "Nota credito" },
            new GenericIDDescription(){ ID = "A", Description = "Nota addebito" },
            new GenericIDDescription(){ ID = "K", Description = "Autofattura" },
            new GenericIDDescription(){ ID = "C", Description = "Autonotacredito" },
            new GenericIDDescription(){ ID = "P", Description = "Proforma" },
        };

        public static ObservableCollection<GenericIDDescription> DDTStatuses => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = null, Description = "Non stampato" },
            new GenericIDDescription(){ ID = "R", Description = "Da fatturare" },
            new GenericIDDescription(){ ID = "F", Description = "Fatturato" },
            new GenericIDDescription(){ ID = "X", Description = "Annullato" },
            new GenericIDDescription(){ ID = "*", Description = "Tutti" }
        };

        public static ObservableCollection<GenericIDDescription> ANAFAT_PIECESProductionTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "P", Description = "Presidiata" },
            new GenericIDDescription(){ ID = "A", Description = "Non presidiata" }
        };

        public static ObservableCollection<GenericIDDescription> ActivityTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "S", Description = "Utente" },
            new GenericIDDescription(){ ID = "U", Description = "Supervisore" }
        };
        #endregion

        #region Production
        public static ObservableCollection<GenericIDDescription> ProductOrderTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "A", Description = "Da lanciare" },
            new GenericIDDescription(){ ID = "O", Description = "Lanciato" },
            new GenericIDDescription(){ ID = "S", Description = "Iniziato" },
            new GenericIDDescription(){ ID = "R", Description = "Pronto da lanciare (materiale ricevuto)" },
            new GenericIDDescription(){ ID = "W", Description = "Sospeso in attesa di materiale" },
            new GenericIDDescription(){ ID = "E", Description = "Terminato" },
            new GenericIDDescription(){ ID = "X", Description = "Annullati"},
            new GenericIDDescription(){ ID = "*", Description = "Tutti"}
        };
        #endregion

        #region SRM
        public static ObservableCollection<GenericIDDescription> RDAStatuses => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "P", Description = "In attesa di approvazione" },
            new GenericIDDescription(){ ID = "A", Description = "Approvate" },
            new GenericIDDescription(){ ID = "T", Description = "Trasformate" },
            new GenericIDDescription(){ ID = "X", Description = "Annullate" },
            new GenericIDDescription(){ ID = "*", Description = "Tutte" }
        };
        public static ObservableCollection<GenericIDDescription> PurchaseOrderStatuses => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "D", Description = "In corso in attesa di firma direzione" },
            new GenericIDDescription(){ ID = "M", Description = "In corso in attesa di firma commerciale" },
            new GenericIDDescription(){ ID = "F", Description = "In corso firmati" },
            new GenericIDDescription(){ ID = "C", Description = "Chiusi" },
            new GenericIDDescription(){ ID = "S", Description = "Inviati" },
            new GenericIDDescription(){ ID = "X", Description = "Annullate" },
            new GenericIDDescription(){ ID = "*", Description = "Tutte" }
            };
        #endregion

        #region Stores
        public static ObservableCollection<GenericIDDescription> StoreTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "I", Description = "Interno" },
            new GenericIDDescription(){ ID = "E", Description = "Esterno" }
        };
        public static ObservableCollection<GenericIDDescription> StoreCausalTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "+", Description = "Carico" },
            new GenericIDDescription(){ ID = "-", Description = "Scarico" }
        };
        #endregion

        #region Cultures
        public static CultureInfo OnlyYearCulture
        {
            get
            {
                var culture = new System.Globalization.CultureInfo("it-IT");
                culture.DateTimeFormat.ShortDatePattern = "yyyy";
                return culture;
            }
        }
        #endregion

        #region Standard filters
        public static ObservableCollection<GenericIDDescription> PrintedStatusTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "P", Description = "Stampati" },
            new GenericIDDescription(){ ID = "N", Description = "Non stampati" },
            new GenericIDDescription(){ ID = "*", Description = "Tutti" }
        };
        #endregion

        #region Accounting assets
        public static ObservableCollection<GenericIDDescription> AccountingAssetsComputeTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = null, Description = "Nessuno" },
            new GenericIDDescription(){ ID = "G", Description = "Giornaliero" },
            new GenericIDDescription(){ ID = "A", Description = "Annuale" }
        };
        public static ObservableCollection<GenericIDDescription> AccountingAssetsSuspensions => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = null, Description = "Calcolo ammortamento + rivalutazione" },
            new GenericIDDescription(){ ID = "R", Description = "Solo calcolo ammortamento" },
            new GenericIDDescription(){ ID = "L", Description = "Solo rivalutazione" },
            new GenericIDDescription(){ ID = "E", Description = "Nessun calcolo" },
            new GenericIDDescription(){ ID = "T", Description = "Fondo ammortamento = valore iniziale" }
        };
        public static ObservableCollection<GenericIDDescription> AccountingAssetsLaunchComputeTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "F", Description = "Fiscale" },
            new GenericIDDescription(){ ID = "C", Description = "Civilistico" }
        };
        #endregion

        #region Datetime
        public static ObservableCollection<GenericIntIDDescription> MonthsNamesWithNone => new ObservableCollection<GenericIntIDDescription>(){
            new GenericIntIDDescription() { ID = 0, Description = "Nessuno" },
            new GenericIntIDDescription() { ID = 1, Description = "Gennaio" },
            new GenericIntIDDescription() { ID = 2, Description = "Febbraio" },
            new GenericIntIDDescription() { ID = 3, Description = "Marzo" },
            new GenericIntIDDescription() { ID = 4, Description = "Aprile" },
            new GenericIntIDDescription() { ID = 5, Description = "Maggio" },
            new GenericIntIDDescription() { ID = 6, Description = "Giugno" },
            new GenericIntIDDescription() { ID = 7, Description = "Luglio" },
            new GenericIntIDDescription() { ID = 8, Description = "Agosto" },
            new GenericIntIDDescription() { ID = 9, Description = "Settembre" },
            new GenericIntIDDescription() { ID = 10, Description = "Ottobre" },
            new GenericIntIDDescription() { ID = 11, Description = "Novembre" },
            new GenericIntIDDescription() { ID = 12, Description = "Dicembre" }
        };
        #endregion
    }
}
