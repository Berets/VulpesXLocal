using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class FATTT00F
    {
        public bool IsEnabled => FTNUFD <= 0;
        public bool IsReadOnly => !IsEnabled;

        public string DefinitiveID => FTNUFD > 0 && Causal != null ? $"{(!string.IsNullOrWhiteSpace(Causal.fatpre) ? $"{Causal.fatpre.Trim()}/" : null)}{FTNUFD}/{FTANNO}" : $"---";
        public string PrintFullID => FTNUFD > 0 && Causal != null ? $"{(!string.IsNullOrWhiteSpace(Causal.fatpre) ? $"{Causal.fatpre.Trim()}/" : null)}{FTNUFD}/{FTANNO}" : $"{FTNUOR}/{FTANNO}";
        public string PrintFilename => FTNUFD > 0 && Causal != null ? $"Fattura [{ftsoci}] {(!string.IsNullOrWhiteSpace(Causal.fatpre) ? $"{Causal.fatpre.Trim()}-" : null)}{FTNUFD}-{FTANNO}.pdf" : $"Fattura [{ftsoci}] {FTNUOR}-{FTANNO}.pdf";
        public string? PrintTemporaryText => FTNUFD > 0 ? null : $"> PROVVISORIA <";

        public ObservableCollection<FATTD00F>? Rows { get; set; }
        public CAUFAT00F? Causal { get; set; }
        public bool PrintAgentsInDetails { get; set; }
        public AGENTI? DefaultFirstAgent { get; set; }
        public AGENTI? DefaultSecondAgent { get; set; }
        public DESTINATARI? Recipient { get; set; }

        #region Computed
        public decimal ScontiCliente => Rows != null ? Math.Round(Rows.Sum(sum => sum.NetPrice) * (FTSCCL ?? 0) / 100, 2, MidpointRounding.AwayFromZero) : 0;
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
                foreach (var rate in Rows.GroupBy(g => new { g.FDASSF, g.FDALIV }))
                {
                    var amount = Rows.Where(w => w.FDASSF == rate.Key.FDASSF && w.FDALIV == rate.Key.FDALIV).Sum(sum => sum.NetPrice);
                    decimal rateValue = 0;
                    decimal.TryParse(rate.Key.FDALIV, out rateValue);
                    totalVAT += Math.Round(amount * rateValue / 100, 2, MidpointRounding.AwayFromZero);
                }
                return Math.Round(totalVAT, 2, MidpointRounding.AwayFromZero);
            }
        }
        public decimal Omaggi => Rows != null ? Math.Round(Rows.Where(w => w.FDTQTA == "O").Sum(sum => sum.NetPrice), 2, MidpointRounding.AwayFromZero) : 0;
        public string CustomerDiscountsText => FTSCCL.HasValue && FTSCCL.Value > 0 ? $"Sconti riservati al cliente ({(FTSCCL.HasValue && FTSCCL.Value > 0 ? $"{FTSCCL.Value.ToString("N2")} %" : null)})" : "Nessuno sconto riservato al cliente";
        public decimal GrandTotal => Math.Round(Imponibile - TotalGifts + TotalVAT, 2, MidpointRounding.AwayFromZero);
        public decimal GrandTotalWithGift => Math.Round(Imponibile + TotalVAT, 2, MidpointRounding.AwayFromZero);
        #endregion

        public ABE? Customer { get; set; }
        public string? CustomerFullDescriptionSearchable { get; set; }
        public string? FTTIPODescription => InvoiceTypes?.Where(w => w.ID == FTTIPO).FirstOrDefault()?.Description;
        public string? FTTIPODescriptionReport { get; set; }
        public ObservableCollection<GenericIDDescription>? InvoiceTypes => CommonsService.InvoiceTypes;

        public bool FTFLA3Bool
        {
            get
            {
                return FTFLA3 == "S";
            }
            set
            {
                if (value)
                    FTFLA3 = "S";
                else
                    FTFLA3 = "N";
            }
        }

        #region Gifts
        public bool HasGifts => Rows != null && Rows.Count > 0 ? Rows.Any(any => any.FDTQTA == "O") : false;
        public decimal TotalGifts
        {
            get
            {
                if (HasGifts)
                {
                    var result = Rows?.Where(w => w.FDTQTA == "O").Sum(sum => sum.NetPrice);
                    result = result - result * (FTSCCL ?? 0) / 100;
                    return Math.Round(result ?? 0, 2, MidpointRounding.AwayFromZero);
                }
                return 0;
            }
        }
        public decimal TotalGiftsVAT
        {
            get
            {
                if (Rows == null || Rows.Where(w => w.FDTQTA == "O").Count() == 0)
                    return 0;

                decimal totalVAT = 0;
                foreach (var rate in Rows.Where(w => w.FDTQTA == "O").GroupBy(g => new { g.FDASSF, g.FDALIV }))
                {
                    var amount = Rows.Where(w => w.FDTQTA == "O" && w.FDASSF == rate.Key.FDASSF && w.FDALIV == rate.Key.FDALIV).Sum(sum => sum.NetPrice);
                    decimal rateValue = 0;
                    decimal.TryParse(rate.Key.FDALIV, out rateValue);
                    totalVAT += amount * rateValue / 100;
                }
                return Math.Round(totalVAT, 2, MidpointRounding.AwayFromZero);
            }
        }
        public List<Tuple<string, string, string, string>>? GiftsRatesRecap { get; set; }
        #endregion

        public bool CanDeleteVisibility => FTNUFD <= 0 ? true : false;
        // autofattura status
        public FATTAUT? SelfInvoice { get; set; }
        public string SelfInvoiceStatusColor => SelfInvoice != null ? "G" : "O";
        public string SelfInvoiceStatusText => SelfInvoice != null ? $"Visualizza i dettagli dell'autofattura" : "Inserisci i dettagli dell'autofattura";
        public bool SelfInvoiceStatusVisibility => FTTIPO == "A" || FTTIPO == "C" ? true : false;
        // allegati
        public ObservableCollection<FATTAL00F>? Attachments { get; set; }
        public string InvoiceAttachmentsStatusColor => Attachments != null && Attachments.Count > 0 ? "G" : "O";
        public string InvoiceAttachmentsStatusText => Attachments != null && Attachments.Count > 0 ? $"Visualizza gli allegati a questa fattura" : "Clicca per aggiungere allegati a questa fattura";
        public string InvoiceAttachmentsText => Attachments?.Count.ToString() ?? "0";
        public string StatusDescription
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FTFLA1) && string.IsNullOrWhiteSpace(FTFLA2))
                {
                    return "Non stampata";
                }
                else
                {
                    if (FTFLA1 == "1" && FTFLA2 != "1")
                    {
                        return "Stampata definitiva";
                    }
                    else
                    {
                        if (FTFLA1 == "1" && FTFLA2 == "1")
                        {
                            return "Stampata e contabilizzata";
                        }
                    }
                }
                return "Non stampata";
            }
        }
        public string? Language { get; set; }
        #region eInvoice sent status
        private string? sdiSentStatus;
        public string? SDISentStatus
        {
            get => sdiSentStatus;
            set
            {
                sdiSentStatus = value;

                if (string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(ftsdiid) && ftsdiid != "#")
                    sdiSentStatus = "D";
                if (string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(ftsdiid) && ftsdiid == "#")
                    sdiSentStatus = "Z";

                switch (sdiSentStatus)
                {
                    case "D":
                        FEStatusText = FTDATFEL.HasValue ? $"Inviata in formato elettronico il {FTDATFEL.Value.ToString("dd/MM/yyyy HH:mm:ss")} con nome {FTNUMFEL} identificativo SDI {ftsdiid}" : null;
                        FEStatusColor = "G";
                        break;
                    case "R":
                        FEStatusText = $"Fattura respinta da Agenzia delle Entrate a causa di {SDIErrors?.Count} error{(SDIErrors?.Count == 1 ? "e" : "i")}\n\n{string.Join("\n", (SDIErrors != null) ? SDIErrors.Select(t => string.Format("{0} {1}", t.Item1, t.Item2)) : string.Empty)}";
                        FEStatusColor = "O";
                        break;
                    case "F":
                        FEStatusText = $"Fattura non recapitata con questa motivazione : {FailureDescription}";
                        FEStatusColor = "V";
                        break;
                    case "T":
                        FEStatusText = FTDATFEL.HasValue ? $"Fattura consegnata ad Agenzia delle Entrate (il {FTDATFEL.Value.ToString("dd/MM/yyyy HH:mm:ss")}), in attesa di elaborazione (puo' richiedere fino a 5 giorni)" : null;
                        FEStatusColor = "Y";
                        break;
                    case "Z":
                        FEStatusText = $"Fattura che non necessita invio perche' recapitata esternamente";
                        FEStatusColor = "M";
                        break;
                    default:
                        FEStatusText = $"Questa fattura non e' ancora stata inviata in formato elettronico";
                        FEStatusColor = "B";
                        break;
                }
                NotifyPropertyChanged("FEStatusColor");
                NotifyPropertyChanged("FEStatusCursor");
                NotifyPropertyChanged("FEStatusText");
            }
        }
        public string SDISentStatusDescription
        {
            get
            {
                switch (sdiSentStatus)
                {
                    case "D":
                        return $"Inviata e consegnata";
                    case "R":
                        return $"Respinta";
                    case "F":
                        return $"Non recapitabile";
                    case "T":
                        return $"In attesa conferma";
                    case "Z":
                        return $"Non necessita invio";
                    default:
                        return $"Non ancora inviata";
                }
            }
        }
        public string? FailureDescription { get; set; }
        public List<Tuple<string, string, string>>? SDIErrors { get; set; }
        public string? FEStatusColor { get; set; }
        //public Cursor FEStatusCursor { get; set; }
        public string? FEStatusText { get; set; }
        #endregion

        #region Info
        public string AddedText => added.HasValue ? added.Value.ToString() : "---";
        public string AddedUserText => !string.IsNullOrWhiteSpace(addedUserID) ? addedUserID : "---";
        public string UpdatedText => updated.HasValue ? updated.Value.ToString() : "---";
        public string UpdatedUserText => !string.IsNullOrWhiteSpace(updatedUserID) ? updatedUserID : "---";
        public string CanceledText => canceled.HasValue ? canceled.Value.ToString() : "---";
        public string CanceledUserText => !string.IsNullOrWhiteSpace(canceledUserID) ? canceledUserID : "---";
        #endregion

        public decimal? FTTRAS { get; set; }

        public string? FTCOAG { get; set; }
        public string? ftagecod2 { get; set; }
        public string? FTCLSO { get; set; }
        public string? fattmpag { get; set; }
    }
}
