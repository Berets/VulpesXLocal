using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class ACC_EINVOICE_HEADS
    {
        public bool fattstampaBool { get => fattstampa == "S"; set => fattstampa = value ? "S" : null; }
        public bool accountedBool => accounted.HasValue;
        public ABE? Supplier { get; set; }
        public ABE? Customer { get; set; }
        public CAUCONT? Causal { get; set; }
        public FE_RFIDOC? TaxRegime { get; set; }
        public FE_TIPODOC? DocumentType { get; set; }
        public ISO? ISO { get; set; }
        public string CustomerReceivedFullDescription => $"{fattclipiva} - {fattclirags}";
        public ObservableCollection<ABE>? Suppliers { get; set; }

        #region Attachments
        //public Cursor InvoiceAttachmentsCursor => fattalle.HasValue && fattalle > 0 ? Cursors.Hand : Cursors.Help;
        public string InvoiceAttachmentsStatusColor => fattalle.HasValue && fattalle > 0 ? "V" : "O";
        public string InvoiceAttachmentsStatusText => fattalle.HasValue && fattalle > 0 ? $"Visualizza gli allegati a questa fattura" : "Nessun allegato presente in questa fattura";
        public string InvoiceAttachmentsText => fattalle.ToString() ?? "0";
        #endregion

        #region Related entities
        public ObservableCollection<ACC_EINVOICE_ROWS>? Rows { get; set; }
        public ObservableCollection<ACC_EINVOICE_SM>? SMs { get; set; }
        public ObservableCollection<ACC_EINVOICE_VAT>? VATs { get; set; }
        public ObservableCollection<ACC_EINVOICE_EXPIRES>? Expires { get; set; }
        public ObservableCollection<ACC_EINVOICE_DDT>? DDTs { get; set; }
        public ObservableCollection<ACC_EINVOICE_PO>? POs { get; set; }
        public ObservableCollection<ACC_EINVOICE_LINKED>? Linkeds { get; set; }
        public ObservableCollection<ACC_EINVOICE_CP>? CPs { get; set; }
        public ObservableCollection<ACC_EINVOICE_RIT>? RITs { get; set; }
        #endregion

        #region Info
        public string AddedText => added.HasValue ? added.Value.ToString() : "---";
        public string AddedUserText => !string.IsNullOrWhiteSpace(addedUserID) ? addedUserID : "---";
        public string UpdatedText => updated.HasValue ? updated.Value.ToString() : "---";
        public string UpdatedUserText => !string.IsNullOrWhiteSpace(updatedUserID) ? updatedUserID : "---";
        public string CanceledText => canceled.HasValue ? canceled.Value.ToString() : "---";
        public string CanceledUserText => !string.IsNullOrWhiteSpace(canceledUserID) ? canceledUserID : "---";
        #endregion

        public string? AccountingText
        {
            get { return (fattannoreg.HasValue && fattnumreg.HasValue) ? $"{fattannoreg}/{fattnumreg}" : "Non contabilizzata"; }
        }

        public bool ViewAccountingEnabled
        {
            get { return (fattannoreg.HasValue && fattnumreg.HasValue) ? true : false; }
        }
    }
}
