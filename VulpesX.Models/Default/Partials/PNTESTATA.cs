using System.Collections.ObjectModel;
using VulpesX.Models.Default.Partials;

namespace VulpesX.Models.Default
{
    public partial class PNTESTATA
    {
        public string? FullID => N1DARE.HasValue ? $"{N1DARE.Value.ToString("d")} {(N1REGI > 0 ? N1REGI : "---")}" : null;
        public string? PrintFullID => $"{N1ANNO}/{N1REGI}";
        public string? PrintDate => N1DARE.HasValue ? N1DARE.Value.ToString("d") : null;
        public string? N1REGIText => N1REGI.ToString();
        public string? N1DAREText => N1DARE.HasValue ? N1DARE.Value.ToString("dd/MM/yyyy") : null;
        public string? N1CLFOText => N1CLFO.HasValue ? N1CLFO.Value.ToString() : null;
        public string PrintFilename => $"Registrazione [{N1SOCI}] {N1ANNO}-{N1REGI}.pdf";
        public string? PrintTemporaryText => !N1TmpPNBool ? null : $"> TEMPORANEA <";
        public decimal Amount { get; set; }
        public decimal OtherAmount { get; set; }
        public CAUCONT? AccountingCausal { get; set; }
        public ABE? BasicRegistry { get; set; }
        public string? EntityFullDescription { get; set; }
        public bool N1TmpPNBool
        {
            get
            {
                return N1TmpPN == "S";
            }

            set
            {
                if (value)
                    N1TmpPN = "S";
                else
                    N1TmpPN = "N";
            }
        }
        public bool ForceProtocol { get; set; }
        public ObservableCollection<PNRIGHE>? Rows { get; set; }
        public ObservableCollection<PNIVA>? VATRows { get; set; }
        public ObservableCollection<PrintRegViewModel>? PrintDetailRows { get; set; }

        public decimal? TotalDare => Rows?.Where(w => w.N1SEGN == "D").Sum(sum => sum.N1IMEU);
        public decimal? TotalAvere => Rows?.Where(w => w.N1SEGN == "A").Sum(sum => sum.N1IMEU);
        public string SbilancioText
        {
            get
            {
                var dare = Math.Round(TotalDare ?? 0, 2);
                var avere = Math.Round(TotalAvere ?? 0, 2);

                if (dare > avere)
                {
                    return $"{dare - avere:N2} D";
                }
                else
                {
                    if (avere > dare)
                        return $"{avere - dare:N2} A";
                    else
                        return $"{0:N2} -";
                }
            }
        }

        public bool NotBalanced
        {
            get
            {
                var amount = Math.Round(Amount, 2);
                var otherAmount = Math.Round(OtherAmount, 2);

                return amount != otherAmount;
            }
        }

        public bool NotBalancedVisibility => Amount == OtherAmount ? false : true;
        public string? NotBalancedTooltip => Amount == OtherAmount ? null : $"Questa registrazione e' sbilanciata per {(Amount > OtherAmount ? (Amount - OtherAmount).ToString("N2") : (OtherAmount - Amount).ToString("N2"))} {(Amount > OtherAmount ? "D" : "A")}";

        public bool DuplicateVisibility => AccountingCausal != null && AccountingCausal.caugen == "S" && AccountingCausal.cauiva == "N" && AccountingCausal.caucli == "N" && AccountingCausal.caufor == "N" && AccountingCausal.cauter == "N" && AccountingCausal.cauint == "N";

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
