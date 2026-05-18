using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Shared.Generics;

namespace VulpesX.Shared
{
    public static class Constants
    {
        public const string APP_NAME = "Vulpes X";

        public const string THEME = "D";
        public const string THEME_LIGHT = "L";
        public const string THEME_DARK = "D";

        public const string VIEW_FULL = "F";
        public const string VIEW_SCREEN = "S";

        //DOMAINS
        public const string UFP_DOMAIN = "@ufp.it";

        public static string NO_LOT_ID = "# Lotto unico";

        public const string CONCURRENCY_VIOLATION = "I dati sono stati modificati da un altro utente, ricaricare il record e riprovare";
        public const string INSERT_VIOLATION = "Errore durante l'inserimento del dato";
        public const string CONNECTION_CREATION_ERROR = "Errore durante la creazione della connessione al database";

        public static string[] ReservedIDs = { "MMA", "PNG", "ENG", "FAP", "FEL", "ORC", "OFF", "ORP", "BOL", "RDA", "ANF", "LOT", "GLO", "BUY", "GOR", "DPP" };
        public static string[] ReservedIDsTSQL = { "'MMA'", "'PNG'", "'ENG'", "'FAP'", "'FEL'", "'ORC'", "'OFF'", "'ORP'", "'BOL'", "'RDA'", "'ANF'", "'LOT'", "'GLO'", "'BUY'", "'GOR'", "'DPP'" };
        public static GenericIDDescription STORE_MOVEMENTS => new GenericIDDescription("MMA", "Movimenti di magazzino");
        public static GenericIDDescription PN => new GenericIDDescription("PNG", "Prima nota");
        public static GenericIDDescription STORE_ENGAGES => new GenericIDDescription("ENG", "Impegni materiale");
        public static GenericIDDescription INVOICE_TEMP => new GenericIDDescription("FAP", "Fatture numerazione provvisoria");
        public static GenericIDDescription E_INVOICE_GENERAL => new GenericIDDescription("FEL", "Fatturazione elettronica");
        public static GenericIDDescription CUSTOMER_ORDERS => new GenericIDDescription("ORC", "Ordini clienti");
        public static GenericIDDescription CUSTOMER_OFFERS => new GenericIDDescription("OFF", "Offerte clienti");
        public static GenericIDDescription PRODUCTION_ORDERS => new GenericIDDescription("ORP", "Ordini produzione");
        public static GenericIDDescription DDT_TEMP => new GenericIDDescription("BOL", "DDT numerazione provvisoria");
        public static GenericIDDescription RDA => new GenericIDDescription("RDA", "Numerazione automatica RDA");
        public static GenericIDDescription FEASABILITY => new GenericIDDescription("ANF", "Numerazione automatica AF");
        public static GenericIDDescription LOTS => new GenericIDDescription("LOT", "Numerazione automatica lotti di produzione");
        public static GenericIDDescription GOODS_LOTS => new GenericIDDescription("GLO", "Numerazione automatica lotti di entrata merce");
        public static GenericIDDescription BUY_ORDERS => new GenericIDDescription("BUY", "Numerazione automatica ordini di acquisto");
        public static GenericIDDescription GOODS_RECEIPTS => new GenericIDDescription("GOR", "Numerazione automatica entrata merci");
        public static GenericIDDescription WALLET_DIST => new GenericIDDescription("DPP", "Numerazione distinte portafogli");
        public static GenericIDDescription PAYMENT_SENDER => new GenericIDDescription("MAP", "Mandati di pagamento");


        public const string _TEMPOID_NONINIZIATO = "0";
        public const string _TEMPOID_INIZIO = "1";
        public const string _TEMPOID_SOSPENSIONE = "2";
        public const string _TEMPOID_RIPRESA = "3";
        public const string _TEMPOID_FINE = "5";
        public const string _TEMPOID_VERSAMENTO = "4";
        public const string _TEMPOID_INIZIOPIAZZAMENTO = "6";
        public const string _TEMPOID_FINEPIAZZAMENTO = "7";
        public const string _TEMPOID_COMPLETA = "X";

        public const string _CALENDARIO_OPEN = "O";
        public const string _CALENDARIO_CLOSE = "C";

        public static DateTime _GX_MIN_DATE = new DateTime(1753, 1, 1);

        #region Production history operations
        public static string PRODUCTION_START_LAUNCH => "Start launch";
        public static string PRODUCTION_LAUNCH => "Launch";
        public static string PRODUCTION_LAUNCH_NOPROD => "Launch without production";
        public static string PRODUCTION_LAUNCH_WAIT => "Launch waited";
        public static string PRODUCTION_LAUNCH_OK => "Launch OK";
        public static string PRODUCTION_START_HALFWORK => "Started halfwork selection";
        public static string PRODUCTION_START_RAWS => "Started raws selection";
        public static string PRODUCTION_CANCELED_LAUNCH => "Launch canceled";
        public static string PRODUCTION_CLEAR_ENGAGES => "Cleared all engages";
        public static string PRODUCTION_UNLOCK => "Unlock";
        public static string PRODUCTION_MANUAL_CHANGE_STATUS => "Manual status changed";
        #endregion

        #region Modules
        public static string MODULE_CRM => "CRM";
        public static string MODULE_SRM => "SRM";
        public static string MODULE_SHIPPING => "Shipping";
        public static string MODULE_ACCOUNTING => "Accounting";
        public static string MODULE_PRODUCTION => "Production";
        public static string MODULE_STORE => "Store";
        #endregion

        #region Report types
        public static string REPORT_TYPE_INVOICE => "Invoice";
        public static string REPORT_TYPE_DDT => "DDT";
        public static string REPORT_TYPE_OFFER_COVER => "OfferCover";
        public static string REPORT_TYPE_OFFER => "Offer";
        public static string REPORT_TYPE_ORDER => "Order";
        public static string REPORT_TYPE_ACCOUNTING_RECORD => "AccountingRecord";
        public static string REPORT_TYPE_ACCOUNTING_EC => "EC";
        public static string REPORT_TYPE_ACCOUNTING_BALANCE_SIMULATION => "BalanceSimulation";
        public static string REPORT_TYPE_ACCOUNTING_BALANCE_SIMULATION_SUB => "SubBalanceSimulation";
        public static string REPORT_TYPE_ACCOUNTING_IVABOOK => "IVABook";
        public static string REPORT_TYPE_ACCOUNTING_IVADETAILS => "IVADetails";
        public static string REPORT_TYPE_ACCOUNTING_IVARECAP => "IVARecap";
        public static string REPORT_TYPE_ACCOUNTING_EXPIRES => "Expires";
        public static string REPORT_TYPE_ACCOUNTING_EXPIRES_DATE => "ExpiresDate";
        public static string REPORT_TYPE_ACCOUNTING_EXPIRES_PAYMENT => "ExpiresPayment";
        public static string REPORT_TYPE_ACCOUNTING_MASTRINI => "Mastrini";
        public static string REPORT_TYPE_ACCOUNTING_IVARECAP_YEARLY => "IVARecapYearly";
        public static string REPORT_TYPE_ACCOUNTING_IVARECAP_DETAILS => "IVARecapDetails";
        public static string REPORT_TYPE_ACCOUNTING_IVARECAP_DETAILS_SUBIVA => "SubIVARecapDetails";
        public static string REPORT_TYPE_PRODUCTION_BADGE_OPERATORE => "BadgeOperatore";
        public static string REPORT_TYPE_PRODUCTION_ODP => "ODP";
        public static string REPORT_TYPE_PRODUCTION_BOX => "BOX";
        public static string REPORT_TYPE_BUY_ORDER => "PurchaseOrder";
        public static string REPORT_TYPE_LOT_LABEL => "LotLabel";
        public static string REPORT_TYPE_PRODUCT_LABEL => "ProductLabel";
        public static string REPORT_TYPE_FINAL_LOT_LABEL => "FinalLotLabel";
        public static string REPORT_TYPE_STORE_STOCKS => "StocksList";
        #endregion

        #region Stylesheet types
        public static string XSL_TYPE_INVOICE_ASSO => "Asso.xsl";
        public static string XSL_TYPE_INVOICE_ADE_PR => "PR.xsl";
        public static string XSL_TYPE_INVOICE_ADE_PA => "PA.xsl";
        #endregion

      
    }
}
