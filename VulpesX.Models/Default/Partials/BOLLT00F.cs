using System.Collections.ObjectModel;
using System.ComponentModel;
 

namespace VulpesX.Models.Default
{
    public partial class BOLLT00F
    {
        public BOLLT00F()
        {
            this.PropertyChanged += BOLLT00F_PropertyChanged;
        }

        private void BOLLT00F_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "BTPESO" || e.PropertyName == "BTPES2")
                NotifyPropertyChanged("Tare");
        }
        public string? BTBOLLText => BTBOLL.ToString();
        public string? BTDATPText => BTDATP.HasValue ? BTDATP.Value.ToString("dd/MM/yyyy") : null;
        public string? BTDATAText => BTDATA.HasValue ? BTDATA.Value.ToString("dd/MM/yyyy") : null;
        public string? BTDASPText => BTDASP.HasValue ? BTDASP.Value.ToString("dd/MM/yyyy HH:mm:ss") : null;
        public bool CanDelete => BTNUBD == 0 && !canceled.HasValue;
        public bool CanDeleteVisibility => CanDelete ? true : false;
        public bool CanPrintPreviewVisibility => CanDelete ? true : false;
        public string? StatusDescription => CommonsService.DDTStatuses.Where(w => w.ID == BTSTATO).FirstOrDefault()?.Description;
        public AGENTI? DefaultFirstAgent { get; set; }
        public AGENTI? DefaultSecondAgent { get; set; }
        public bool PrintAgentsInDetails { get; set; }
        public ABE? Customer { get; set; }
        public int CustomerID { get; set; }
        public string? CustomerDescription { get; set; }
        public string? CustomerFullDescriptionSearchable { get; set; }

        public DESTINATARI? Recipient { get; set; }
        public int RecipientID { get; set; }
        public string? RecipientDescription { get; set; }
        public string? RecipientFullDescriptionSearchable { get; set; }
        public CAUSBOLL? Causal { get; set; }
        public AREE? Area { get; set; }
        public SPEDIZIONE? Shipment { get; set; } // porto
        public CONSEGNA? Delivery { get; set; } // tipo trasporto
        public VETTORI? FirstCarrier { get; set; }
        public VETTORI? SecondCarrier { get; set; }

        #region Computed
        public decimal ScontiCliente => Rows != null ? Math.Round(Rows.Sum(sum => sum.NetPrice) * (BTSCCL ?? 0) / 100, 2) : 0;
        public decimal Tare => (BTPESO ?? 0) - (BTPES2 ?? 0);
        public string CustomerDiscountsText => BTSCCL.HasValue && BTSCCL.Value > 0 || BTSCCL.HasValue && BTSCCL.Value > 0 ? $"Sconti riservati al cliente ({(BTSCCL.HasValue && BTSCCL.Value > 0 ? $"{BTSCCL.Value.ToString("N2")} %" : null)})" : "Nessuno sconto riservato al cliente";
        public decimal Imponibile => Rows != null ? Math.Round(Rows.Sum(sum => sum.NetPrice) - ScontiCliente, 2, MidpointRounding.AwayFromZero) : 0;
        #endregion

        public string DefinitiveID => BTNUBD > 0 && Causal != null ? $"{(!string.IsNullOrWhiteSpace(Causal.bolpre) ? $"{Causal.bolpre.Trim()}/" : null)}{BTNUBD}/{BTANNO}" : $"---";
        public string PrintFullID => BTNUBD > 0 && Causal != null ? $"{(!string.IsNullOrWhiteSpace(Causal.bolpre) ? $"{Causal.bolpre.Trim()}/" : null)}{BTNUBD}/{BTANNO}" : $"{BTBOLL}/{BTANNO}";
        public string PrintFilename => BTNUBD > 0 && Causal != null ? $"DDT [{bolsoc}] {(!string.IsNullOrWhiteSpace(Causal.bolpre) ? $"{Causal.bolpre.Trim()}-" : null)}{BTNUBD}-{BTANNO}.pdf" : $"Bozza DDT [{bolsoc}] {BTBOLL}-{BTANNO}.pdf";
        public string? PrintTemporaryText => BTNUBD > 0 ? null : $"> DOCUMENTO PROVVISORIO <";
        public string? PrintDate => BTNUBD > 0 && BTDATA.HasValue ? $"{BTDATA.Value.Date.ToShortDateString()}" : BTDATP.HasValue ? $"{BTDATP.Value.Date.ToShortDateString()}" : null;

        public ObservableCollection<BOLLD00F>? Rows { get; set; }
        public string BTCOLLText => (BTCOLL ?? 0) > 0 && BTCOLL.HasValue ? BTCOLL.Value.ToString() : "0";
        public string? Language { get; set; }
        #region Gifts
        public bool HasGifts => Rows != null && Rows.Count > 0 ? Rows.Any(any => any.BOTQTA == "O") : false;
        public decimal TotalGifts
        {
            get
            {
                if (HasGifts)
                {
                    var result = Rows?.Where(w => w.BOTQTA == "O").Sum(sum => sum.NetPrice);
                    result = result - result * (BTSCCL ?? 0) / 100;

                    return Math.Round(result ?? 0, 2, MidpointRounding.AwayFromZero);
                }
                return 0;
            }
        }
        public decimal TotalGiftsVAT
        {
            get
            {
                if (Rows == null || Rows.Where(w => w.BOTQTA == "O").Count() == 0)
                    return 0;

                decimal totalVAT = 0;
                foreach (var rate in Rows.Where(w => w.BOTQTA == "O").GroupBy(g => new { g.boasso, g.boaliq }))
                {
                    var amount = Rows.Where(w => w.BOTQTA == "O" && w.boasso == rate.Key.boasso && w.boaliq == rate.Key.boaliq).Sum(sum => sum.NetPrice);
                    decimal rateValue = 0;
                    decimal.TryParse(rate.Key.boaliq, out rateValue);
                    totalVAT += amount * rateValue / 100;
                }
                return Math.Round(totalVAT, 2, MidpointRounding.AwayFromZero);
            }
        }
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
