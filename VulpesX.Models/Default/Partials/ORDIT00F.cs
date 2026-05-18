using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class ORDIT00F
    {
        public int ProductionOrdersNeeded { get; set; }
        public int ProductionOrdersGenerated { get; set; }
        public int ActiveAlerts { get; set; }
        public int TotalAlerts { get; set; }
        public bool NeedProduction => ProductionOrdersNeeded > ProductionOrdersGenerated;
        public ABE? Customer { get; set; }
        public string? CustomerFullDescriptionSearchable { get; set; }
        public DESTINATARI? Recipient { get; set; }
        public string? RecipientFullDescriptionSearchable { get; set; }
        public TAB_CRM_CAUORD? Causal { get; set; }
        public VETTORI? FirstCarrier { get; set; }
        public VETTORI? SecondCarrier { get; set; }
        public SPEDIZIONE? Shipping { get; set; }
        public CONSEGNA? Delivery { get; set; }
        public AGENTI? DefaultFirstAgent { get; set; }
        public AGENTI? DefaultSecondAgent { get; set; }
        public bool PrintAgentsInDetails { get; set; }
        public string PrintFullID => $"{OTANNO}/{OTNUOR}";
        public string PrintFilename => $"Ordine [{otsoci}] {OTANNO}-{OTNUOR}.pdf";
        public string? flgchiDescription => CommonsService.OrderStatuses.Where(w => w.ID == flgchi).FirstOrDefault()?.Description;
        public string? RecipientText => Recipient != null ? $"{Recipient.codesti.ToString("N0")} {Recipient.ragisoc} - {Recipient.DEINDI} {(Recipient.DECAP.HasValue ? Recipient.DECAP.Value.ToString("N0").PadLeft(5, '0') : null)}, {Recipient.deloc} ({Recipient.depro})" : null;
        public ObservableCollection<ORDID00F>? Rows { get; set; }
        public ObservableCollection<GenericStringDecimal>? UMsRecap { get; set; }
        public List<Tuple<string, string, string, string>>? RatesRecap { get; set; }
        public List<Tuple<string, string, string, string>>? RatesRecap2 { get; set; }
        public ObservableCollection<FATTD00F>? Invoices { get; set; }
        public int InvoicesCount { get; set; }
        public ObservableCollection<BOLLD00F>? DDTs { get; set; }
        public int DDTCount { get; set; }
        public string? Language { get; set; }
        #region Computed
        public decimal ScontiCliente => Rows != null ? Math.Round(Rows.Sum(sum => sum.NetPrice) * (OTSCCL ?? 0) / 100, 2) : 0;
        public decimal Imponibile => Rows != null ? Math.Round(Rows.Sum(sum => sum.NetPrice) - ScontiCliente, 2, MidpointRounding.AwayFromZero) : 0;
        public decimal Total => Rows != null ? Math.Round(Rows.Sum(sum => sum.AmountDisplay), 2, MidpointRounding.AwayFromZero) : 0;
        public decimal Sconti => Rows != null ? Math.Round(Rows.Sum(sum => sum.Discount), 2, MidpointRounding.AwayFromZero) : 0;
        public decimal Maggiorazioni => Rows != null ? Math.Round(Rows.Sum(sum => sum.Surcharge), 2, MidpointRounding.AwayFromZero) : 0;
        public decimal TotalVAT
        {
            get
            {
                if (Rows == null || Rows.Count == 0)
                    return 0;

                decimal totalVAT = 0;
                foreach (var rate in Rows.GroupBy(g => new { g.ODASSF, g.ODALIV }))
                {
                    var amount = Rows.Where(w => w.ODASSF == rate.Key.ODASSF && w.ODALIV == rate.Key.ODALIV).Sum(sum => sum.NetPrice);
                    decimal rateValue = 0;
                    decimal.TryParse(rate.Key.ODALIV, out rateValue);
                    totalVAT += amount * rateValue / 100;
                }
                return Math.Round(totalVAT, 2, MidpointRounding.AwayFromZero);
            }
        }
        public decimal Omaggi => Rows != null ? Math.Round(Rows.Where(w => w.ODTQTA == "O").Sum(sum => sum.NetPrice), 2, MidpointRounding.AwayFromZero) : 0;
        public string CustomerDiscountsText => OTSCCL.HasValue && OTSCCL.Value > 0 ? $"Sconti riservati al cliente ({(OTSCCL.HasValue && OTSCCL.Value > 0 ? $"{OTSCCL.Value.ToString("N2")} %" : null)})" : "Nessuno sconto riservato al cliente";
        #endregion

        public bool CanDelete => canceled == null && (!Rows?.Where(w => w.ODQTAEV != null)?.Any() ?? false) && flgchi != "E" && flgchi != "X";
        public bool CanDeleteVisibility => CanDelete ? true : false;

        public bool CanPrint => canceled != null ? false :
                                OTINFI.HasValue && !string.IsNullOrWhiteSpace(OTINFIUSR) &&
                                OTFITE.HasValue && !string.IsNullOrWhiteSpace(OTFITEUSR) &&
                                OTFICO.HasValue && !string.IsNullOrWhiteSpace(OTFICOUSR);

        #region Gifts
        public bool HasGifts => Rows != null && Rows.Count > 0 ? Rows.Any(any => any.ODTQTA == "O") : false;
        public decimal TotalGifts
        {
            get
            {
                if (HasGifts)
                {
                    var result = Rows?.Where(w => w.ODTQTA == "O").Sum(sum => sum.NetPrice);
                    result = result - result * (OTSCCL ?? 0) / 100;
                    return Math.Round(result ?? 0, 2, MidpointRounding.AwayFromZero);
                }
                return 0;
            }
        }
        public decimal TotalGiftsVAT
        {
            get
            {
                if (Rows == null || Rows.Where(w => w.ODTQTA == "O").Count() == 0)
                    return 0;

                decimal totalVAT = 0;
                foreach (var rate in Rows.Where(w => w.ODTQTA == "O").GroupBy(g => new { g.ODASSF, g.ODALIV }))
                {
                    var amount = Rows.Where(w => w.ODTQTA == "O" && w.ODASSF == rate.Key.ODASSF && w.ODALIV == rate.Key.ODALIV).Sum(sum => sum.NetPrice);
                    decimal rateValue = 0;
                    decimal.TryParse(rate.Key.ODALIV, out rateValue);
                    totalVAT += amount * rateValue / 100;
                }
                return Math.Round(totalVAT, 2, MidpointRounding.AwayFromZero);
            }
        }
        public List<Tuple<string, string, string, string>>? GiftsRatesRecap { get; set; }
        #endregion

        public string CanPrintColor => CanPrint ? "G" : "Y";
        public string PrintToolTip => CanPrint ? "Apri il PDF dell'ordine" : "Mancano delle firme per procedere alla stampa dell'ordine, verrà stampato come bozza";
        public bool CanSend => canceled != null ? false :
                                OTINFI.HasValue && !string.IsNullOrWhiteSpace(OTINFIUSR) &&
                                OTFITE.HasValue && !string.IsNullOrWhiteSpace(OTFITEUSR) &&
                                OTFICO.HasValue && !string.IsNullOrWhiteSpace(OTFICOUSR);
        public bool CanTasksVisibility => canceled == null ? true : false;
        public bool CanProductionTasksVisibility => canceled == null && NeedProduction ? true : false;
        public string CanSendColor => CanSend ? "G" : "O";
        public string SendToolTip => CanSend ? "Invia l'ordine tramite email" : "Mancano delle firme per procedere all'invio dell'ordine";
        public string SignToolTip
        {
            get
            {
                if (!OTINFI.HasValue && string.IsNullOrWhiteSpace(OTINFIUSR) &&
                    !OTFITE.HasValue && string.IsNullOrWhiteSpace(OTFITEUSR) &&
                    !OTFICO.HasValue && string.IsNullOrWhiteSpace(OTFICOUSR))
                {
                    return "Clicca per inviare l'ordine alla firma tecnica";
                }
                else
                {
                    if (OTINFI.HasValue && !string.IsNullOrWhiteSpace(OTINFIUSR) &&
                    !OTFITE.HasValue && string.IsNullOrWhiteSpace(OTFITEUSR) &&
                    !OTFICO.HasValue && string.IsNullOrWhiteSpace(OTFICOUSR))
                    {
                        return "Clicca per apporre la firma tecnica e mandare l'ordine alla firma commerciale";
                    }
                    else
                    {
                        if (OTINFI.HasValue && !string.IsNullOrWhiteSpace(OTINFIUSR) &&
                            OTFITE.HasValue && !string.IsNullOrWhiteSpace(OTFITEUSR) &&
                            !OTFICO.HasValue && string.IsNullOrWhiteSpace(OTFICOUSR))
                        {
                            return "Clicca per apporre la firma commerciale e confermare l'ordine";
                        }
                        else
                        {
                            if (OTINFI.HasValue && !string.IsNullOrWhiteSpace(OTINFIUSR) &&
                                OTFITE.HasValue && !string.IsNullOrWhiteSpace(OTFITEUSR) &&
                                OTFICO.HasValue && !string.IsNullOrWhiteSpace(OTFICOUSR))
                            {
                                return "L'ordine ha ricevuto tutte le firme necessarie";
                            }
                            else
                            { return "L'ordine non e' valido"; }
                        }
                    }
                }
            }
        }
        public string SignColor
        {
            get
            {
                if (!OTINFI.HasValue && string.IsNullOrWhiteSpace(OTINFIUSR) &&
                    !OTFITE.HasValue && string.IsNullOrWhiteSpace(OTFITEUSR) &&
                    !OTFICO.HasValue && string.IsNullOrWhiteSpace(OTFICOUSR))
                {
                    return "G";
                }
                else
                {
                    if (OTINFI.HasValue && !string.IsNullOrWhiteSpace(OTINFIUSR) &&
                    !OTFITE.HasValue && string.IsNullOrWhiteSpace(OTFITEUSR) &&
                    !OTFICO.HasValue && string.IsNullOrWhiteSpace(OTFICOUSR))
                    {
                        return "P";
                    }
                    else
                    {
                        if (OTINFI.HasValue && !string.IsNullOrWhiteSpace(OTINFIUSR) &&
                            OTFITE.HasValue && !string.IsNullOrWhiteSpace(OTFITEUSR) &&
                            !OTFICO.HasValue && string.IsNullOrWhiteSpace(OTFICOUSR))
                        {
                            return "G";
                        }
                        else
                        {
                            if (OTINFI.HasValue && !string.IsNullOrWhiteSpace(OTINFIUSR) &&
                                OTFITE.HasValue && !string.IsNullOrWhiteSpace(OTFITEUSR) &&
                                OTFICO.HasValue && !string.IsNullOrWhiteSpace(OTFICOUSR))
                            {
                                return "B";
                            }
                            else
                            {
                                return "O";
                            }
                        }
                    }
                }
            }
        }
        //public Cursor SignCursor
        //{
        //    get
        //    {
        //        if (OTINFI.HasValue && !string.IsNullOrWhiteSpace(OTINFIUSR) &&
        //            OTFITE.HasValue && !string.IsNullOrWhiteSpace(OTFITEUSR) &&
        //            OTFICO.HasValue && !string.IsNullOrWhiteSpace(OTFICOUSR))
        //        {
        //            return null;
        //        }
        //        else
        //        { return Cursors.Hand; }
        //    }
        //}
        // alerts 
        public string AlertToolTip
        {
            get
            {
                if (TotalAlerts == 0)
                    return $"Non ci sono promemoria su questo ordine";
                else
                    return $"Ci sono {TotalAlerts} promemoria su questo ordine, dei quali {ActiveAlerts} attivi";
            }
        }
        public string AlertColor
        {
            get
            {
                if (TotalAlerts == 0)
                {
                    return "W";
                }
                else
                {
                    if (ActiveAlerts > 0)
                    {
                        return "G";
                    }
                    else
                    {
                        return "B";
                    }
                }
            }
        }
        // production 
        public string ProductionToolTip
        {
            get
            {
                if (flgchi == "F")
                {
                    if (NeedProduction)
                        return "Clicca per generare gli ordini di produzione";
                    else
                        return "Questo ordine non contiene articoli che necessitino di produzione";
                }
                else
                {
                    return "Questo ordine non puo' generare ordini di produzione perche' già generati oppure perchè chiuso, non firmato o annullato";
                }
            }
        }
        public string ProductionColor
        {
            get
            {
                if (flgchi == "F")
                {
                    return "G";
                }
                else
                {
                    return "O";
                }
            }
        }
        //public Cursor ProductionCursor
        //{
        //    get
        //    {
        //        if (flgchi == "F")
        //        {
        //            return Cursors.Hand;
        //        }
        //        else
        //        { return null; }
        //    }
        //}
        // DDT
        public string DDTStatusColor => DDTCount > 0 ? "G" : "O";
        public string DDTStatusText => DDTCount > 0 ? $"Visualizza i DDT emessi per questo ordine" : "Nessun DDT emesso per questo ordine";
        public string DDTText => DDTCount.ToString() ?? "0";
        // invoices
        public string InvoicesStatusColor => InvoicesCount > 0 ? "G" : "O";
        public string InvoicesStatusText => InvoicesCount > 0 ? $"Visualizza le fatture emesse per questo ordine" : "Nessuna fattura emessa per questo ordine";
        public string InvoicesText => InvoicesCount.ToString() ?? "0";
        // allegati
        public ObservableCollection<ORDITAL00F>? Attachments { get; set; }
        public string OrderAttachmentsStatusColor => Attachments != null && Attachments.Count > 0 ? "G" : "O";
        public string OrderAttachmentsStatusText => Attachments != null && Attachments.Count > 0 ? $"Visualizza gli allegati a questo ordine" : "Clicca per aggiungere allegati a questo ordine";
        public string OrderAttachmentsText => Attachments?.Count.ToString() ?? "0";

        #region Info
        public string AddedText => added.HasValue ? added.Value.ToString() : "---";
        public string AddedUserText => !string.IsNullOrWhiteSpace(addedUserID) ? addedUserID : "---";
        public string UpdatedText => updated.HasValue ? updated.Value.ToString() : "---";
        public string UpdatedUserText => !string.IsNullOrWhiteSpace(updatedUserID) ? updatedUserID : "---";
        public string CanceledText => canceled.HasValue ? canceled.Value.ToString() : "---";
        public string CanceledUserText => !string.IsNullOrWhiteSpace(canceledUserID) ? canceledUserID : "---";
        #endregion
    }
}
