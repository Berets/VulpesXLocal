using System.Collections.ObjectModel;
using System.ComponentModel;
 

namespace VulpesX.Models.Default
{
    public partial class OFFET00F
    {
        public OFFET00F()
        {
            PropertyChanged += OFFET00F_PropertyChanged;
        }

        private void OFFET00F_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "OFTPELO" || e.PropertyName == "OFTPENE")
                NotifyPropertyChanged("Tare");
        }

        public ABE? Customer { get; set; }
        public DESTINATARI? Recipient { get; set; }
        public TAB_CRM_CAUOFF? Causal { get; set; }
        public VETTORI? FirstCarrier { get; set; }
        public VETTORI? SecondCarrier { get; set; }
        public SPEDIZIONE? Shipping { get; set; }
        public CONSEGNA? Delivery { get; set; }
        public AGENTI? DefaultFirstAgent { get; set; }
        public AGENTI? DefaultSecondAgent { get; set; }
        public bool PrintAgentsInDetails { get; set; }
        public bool PrintObject { get; set; }
        public string? Language { get; set; }

        #region Computed
        public decimal ScontiCliente => Rows != null ? Math.Round(Rows.Sum(sum => sum.NetPrice) * (OFTSCCL ?? 0) / 100, 2, MidpointRounding.AwayFromZero) : 0;
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
                foreach (var rate in Rows.GroupBy(g => new { g.OFDASSF, g.OFDALIV }))
                {
                    var amount = Rows.Where(w => w.OFDASSF == rate.Key.OFDASSF && w.OFDALIV == rate.Key.OFDALIV).Sum(sum => sum.NetPrice);
                    decimal rateValue = 0;
                    decimal.TryParse(rate.Key.OFDALIV, out rateValue);
                    totalVAT += amount * rateValue / 100;
                }
                return Math.Round(totalVAT, 2, MidpointRounding.AwayFromZero);
            }
        }
        public decimal Omaggi => Rows != null ? Math.Round(Rows.Where(w => w.OFDTQTA == "O").Sum(sum => sum.NetPrice), 2, MidpointRounding.AwayFromZero) : 0;
        public string CustomerDiscountsText => OFTSCCL.HasValue && OFTSCCL.Value > 0 ? $"Sconti riservati al cliente ({(OFTSCCL.HasValue && OFTSCCL.Value > 0 ? $"{OFTSCCL.Value.ToString("N2")} %" : null)})" : "Nessuno sconto riservato al cliente";
        #endregion

        public string PrintFullID => $"{OFTANNO}/{OFTNUOR}";
        public string PrintFilename => $"Offerta [{oftsoci}] {OFTANNO}-{OFTNUOR}.pdf";
        public string? oflgchiDescription => CommonsService.OfferStatuses.Where(w => w.ID == oflgchi).FirstOrDefault()?.Description;
        public string? RecipientText => Recipient != null ? $"{Recipient.codesti.ToString("N0")} {Recipient.ragisoc} - {Recipient.DEINDI} {(Recipient.DECAP.HasValue ? Recipient.DECAP.Value.ToString("N0").PadLeft(5, '0') : null)}, {Recipient.deloc} ({Recipient.depro})" : null;
        public ObservableCollection<OFFED00F>? Rows { get; set; }

        #region Gifts
        public bool HasGifts => Rows != null && Rows.Count > 0 ? Rows.Any(any => any.OFDTQTA == "O") : false;
        public decimal TotalGifts
        {
            get
            {
                if (HasGifts)
                {
                    var result = Rows?.Where(w => w.OFDTQTA == "O").Sum(sum => sum.NetPrice);
                    result = result - result * (OFTSCCL ?? 0) / 100;
                    return Math.Round(result ?? 0, 2, MidpointRounding.AwayFromZero);
                }
                return 0;
            }
        }
        public decimal TotalGiftsVAT
        {
            get
            {
                if (Rows == null || Rows.Where(w => w.OFDTQTA == "O").Count() == 0)
                    return 0;

                decimal totalVAT = 0;
                foreach (var rate in Rows.Where(w => w.OFDTQTA == "O").GroupBy(g => new { g.OFDASSF, g.OFDALIV }))
                {
                    var amount = Rows.Where(w => w.OFDTQTA == "O" && w.OFDASSF == rate.Key.OFDASSF && w.OFDALIV == rate.Key.OFDALIV).Sum(sum => sum.NetPrice);
                    decimal rateValue = 0;
                    decimal.TryParse(rate.Key.OFDALIV, out rateValue);
                    totalVAT += amount * rateValue / 100;
                }
                return Math.Round(totalVAT, 2, MidpointRounding.AwayFromZero);
            }
        }
        public List<Tuple<string, string, string, string>>? GiftsRatesRecap { get; set; }
        #endregion

        public ObservableCollection<GenericStringDecimal>? UMsRecap { get; set; }
        public List<Tuple<string, string, string, string>>? RatesRecap { get; set; }
        public List<Tuple<string, string, string, string>>? RatesRecap2 { get; set; }

        #region Grid tasks properties
        public string? EntityBackground => (Customer != null) ? Customer.abecfe != "P" ? "T" : "B" : null;
        public string? EntityTooltip => (Customer != null) ? Customer.abecfe != "P" ? "Offerta a cliente" : "Offerta a prospect" : null;
        public bool CanDelete => canceled == null && (!Rows?.Where(w => w.OFDSTA != null || w.OFDQTAEV != null)?.Any() ?? false) && oflgchi != "O" && oflgchi != "C";
        public bool CanDeleteVisibility => CanDelete ? true : false;
        public bool CanPrint => canceled != null ? false :
                                OFTINFI.HasValue && !string.IsNullOrWhiteSpace(OFTINFIUSR) &&
                                OFTFITE.HasValue && !string.IsNullOrWhiteSpace(OFTFITEUSR) &&
                                OFTFICO.HasValue && !string.IsNullOrWhiteSpace(OFTFICOUSR);
        public string CanPrintColor => CanPrint ? "G" : "Y";
        public string PrintToolTip => CanPrint ? "Apri il PDF dell'offerta" : "Mancano delle firme per procedere alla stampa dell'offerta, verrà stampata come bozza";
        public bool CanSend => canceled != null ? false :
                                OFTINFI.HasValue && !string.IsNullOrWhiteSpace(OFTINFIUSR) &&
                                OFTFITE.HasValue && !string.IsNullOrWhiteSpace(OFTFITEUSR) &&
                                OFTFICO.HasValue && !string.IsNullOrWhiteSpace(OFTFICOUSR);
        public bool CanTasksVisibility => canceled == null ? true : false;
        public string CanSendColor => CanSend ? "G" : "O";
        public string SendToolTip => CanSend ? "Invia l'offerta tramite email" : "Mancano delle firme per procedere all'invio dell'offerta";
        public string SignToolTip
        {
            get
            {
                if (!OFTINFI.HasValue && string.IsNullOrWhiteSpace(OFTINFIUSR) &&
                    !OFTFITE.HasValue && string.IsNullOrWhiteSpace(OFTFITEUSR) &&
                    !OFTFICO.HasValue && string.IsNullOrWhiteSpace(OFTFICOUSR))
                {
                    return "Clicca per inviare l'offerta alla firma tecnica";
                }
                else
                {
                    if (OFTINFI.HasValue && !string.IsNullOrWhiteSpace(OFTINFIUSR) &&
                    !OFTFITE.HasValue && string.IsNullOrWhiteSpace(OFTFITEUSR) &&
                    !OFTFICO.HasValue && string.IsNullOrWhiteSpace(OFTFICOUSR))
                    {
                        return "Clicca per apporre la firma tecnica e mandare l'offerta alla firma commerciale";
                    }
                    else
                    {
                        if (OFTINFI.HasValue && !string.IsNullOrWhiteSpace(OFTINFIUSR) &&
                            OFTFITE.HasValue && !string.IsNullOrWhiteSpace(OFTFITEUSR) &&
                            !OFTFICO.HasValue && string.IsNullOrWhiteSpace(OFTFICOUSR))
                        {
                            return "Clicca per apporre la firma commerciale e confermare l'ordine";
                        }
                        else
                        {
                            if (OFTINFI.HasValue && !string.IsNullOrWhiteSpace(OFTINFIUSR) &&
                                OFTFITE.HasValue && !string.IsNullOrWhiteSpace(OFTFITEUSR) &&
                                OFTFICO.HasValue && !string.IsNullOrWhiteSpace(OFTFICOUSR))
                            {
                                return "L'offerta ha ricevuto tutte le firme necessarie";
                            }
                            else
                            { return "L'offerta non e' valida"; }
                        }
                    }
                }
            }
        }
        public string SignColor
        {
            get
            {
                if (!OFTINFI.HasValue && string.IsNullOrWhiteSpace(OFTINFIUSR) &&
                    !OFTFITE.HasValue && string.IsNullOrWhiteSpace(OFTFITEUSR) &&
                    !OFTFICO.HasValue && string.IsNullOrWhiteSpace(OFTFICOUSR))
                {
                    return "G";
                }
                else
                {
                    if (OFTINFI.HasValue && !string.IsNullOrWhiteSpace(OFTINFIUSR) &&
                    !OFTFITE.HasValue && string.IsNullOrWhiteSpace(OFTFITEUSR) &&
                    !OFTFICO.HasValue && string.IsNullOrWhiteSpace(OFTFICOUSR))
                    {
                        return "P";
                    }
                    else
                    {
                        if (OFTINFI.HasValue && !string.IsNullOrWhiteSpace(OFTINFIUSR) &&
                            OFTFITE.HasValue && !string.IsNullOrWhiteSpace(OFTFITEUSR) &&
                            !OFTFICO.HasValue && string.IsNullOrWhiteSpace(OFTFICOUSR))
                        {
                            return "G";
                        }
                        else
                        {
                            if (OFTINFI.HasValue && !string.IsNullOrWhiteSpace(OFTINFIUSR) &&
                                OFTFITE.HasValue && !string.IsNullOrWhiteSpace(OFTFITEUSR) &&
                                OFTFICO.HasValue && !string.IsNullOrWhiteSpace(OFTFICOUSR))
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
        //        if (OFTINFI.HasValue && !string.IsNullOrWhiteSpace(OFTINFIUSR) &&
        //            OFTFITE.HasValue && !string.IsNullOrWhiteSpace(OFTFITEUSR) &&
        //            OFTFICO.HasValue && !string.IsNullOrWhiteSpace(OFTFICOUSR))
        //        {
        //            return null;
        //        }
        //        else
        //        { return Cursors.Hand; }
        //    }
        //}
        #endregion
        #region Attachments
        public ObservableCollection<OFFETAL00F>? Attachments { get; set; }
        public string OfferAttachmentsStatusColor => Attachments != null && Attachments.Count > 0 ? "G" : "O";
        public string OfferAttachmentsStatusText => Attachments != null && Attachments.Count > 0 ? $"Visualizza gli allegati a questa offerta" : "Clicca per aggiungere allegati a questa offerta";
        public string OfferAttachmentsText => Attachments?.Count.ToString() ?? "0";
        #endregion
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
