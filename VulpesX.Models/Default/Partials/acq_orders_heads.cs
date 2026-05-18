using System.Collections.ObjectModel;
using VulpesX.Models.Default.Partials;


namespace VulpesX.Models.Default;

public partial class acq_orders_heads
{
    public event EventHandler? SupplierChanged;
    protected void OnSupplierChanged(EventArgs e)
    {
        EventHandler? handler = SupplierChanged;
        if (handler != null)
            handler(this, e);
    }
    public event EventHandler? PaymentChanged;
    protected void OnPaymentChanged(EventArgs e)
    {
        EventHandler? handler = PaymentChanged;
        if (handler != null)
            handler(this, e);
    }

    public ObservableCollection<acq_orders_rows>? Rows { get; set; }
    public ObservableCollection<GenericStringDecimal>? UMsRecap { get; set; }
    // first 3
    public List<Tuple<string, string, string, string>>? RatesRecap { get; set; }
    // others
    public List<Tuple<string, string, string, string>>? RatesRecap2 { get; set; }

    public string PrintFilename => $"Ordine di acquisto [{company_id}] {id}.pdf";

    public string SendUserText => !string.IsNullOrWhiteSpace(send_user) ? send_user : "---";
    public bool ClosedVisibility => closed.HasValue ? true : false;
    public string? Language { get; set; }

    #region Computed
    public decimal TotalAmount => Rows != null && Rows.Count > 0 ? Rows.Sum(sum => sum.NetAmount) : 0;
    public bool CanDelete => !closed.HasValue && !canceled.HasValue;
    public bool CanDeleteVisibility => CanDelete ? true : false;

    public bool CanClose => !closed.HasValue && !canceled.HasValue && sent.HasValue;
    public bool CanCloseVisibility => CanClose ? true : false;
    #endregion

    #region Supplier
    public ABE? SupplierGrid { get; set; }
    private ABE? supplier;
    public ABE? Supplier
    {
        get => supplier;
        set
        {
            if (supplier?.abecod != value?.abecod)
            {
                supplier_id = value?.abecod ?? 0;
                supplier = value;
                NotifyPropertyChanged("Supplier");
                OnSupplierChanged(EventArgs.Empty);
            }
        }
    }

    private ObservableCollection<ABE>? suppliers;
    public ObservableCollection<ABE>? Suppliers
    {
        get => suppliers;
        set
        {
            suppliers = value;
            if (supplier_id > 0)
                Supplier = suppliers?.Where(w => w.abecod == supplier_id).FirstOrDefault();
            else
                Supplier = null;
            NotifyPropertyChanged("Suppliers");
        }
    }
    #endregion

    #region Payment
    public PAGFOR? PaymentGrid { get; set; }
    private PAGFOR? payment;
    public PAGFOR? Payment
    {
        get => payment;
        set
        {
            if (payment?.pfocod != value?.pfocod)
            {
                payment_id = value?.pfocod;
                payment = value;
                NotifyPropertyChanged("Payment");
                OnPaymentChanged(EventArgs.Empty);
            }
        }
    }

    private ObservableCollection<PAGFOR>? payments;
    public ObservableCollection<PAGFOR>? Payments
    {
        get => payments;
        set
        {
            payments = value;
            if (!string.IsNullOrWhiteSpace(payment_id))
                Payment = payments?.Where(w => w.pfocod == payment_id).FirstOrDefault();
            else
                Payment = null;
            NotifyPropertyChanged("Payments");
        }
    }
    #endregion

    #region Shipment
    private SPEDIZIONE? shipment;
    public SPEDIZIONE? Shipment
    {
        get => shipment;
        set
        {
            if (shipment?.specod != value?.specod)
            {
                shipping_id = value?.specod;
                shipment = value;
                NotifyPropertyChanged("Shipment");
            }
        }
    }

    private ObservableCollection<SPEDIZIONE>? shipments;
    public ObservableCollection<SPEDIZIONE>? Shipments
    {
        get => shipments;
        set
        {
            shipments = value;
            if (!string.IsNullOrWhiteSpace(shipping_id))
                Shipment = shipments?.Where(w => w.specod == shipping_id).FirstOrDefault();
            else
                Shipment = null;
            NotifyPropertyChanged("Shipments");
        }
    }
    #endregion

    #region Delivery
    private CONSEGNA? delivery;
    public CONSEGNA? Delivery
    {
        get => delivery;
        set
        {
            if (delivery?.concod != value?.concod)
            {
                delivery_id = value?.concod;
                delivery = value;
                NotifyPropertyChanged("Delivery");
            }
        }
    }

    private ObservableCollection<CONSEGNA>? deliveries;
    public ObservableCollection<CONSEGNA>? Deliveries
    {
        get => deliveries;
        set
        {
            deliveries = value;
            if (!string.IsNullOrWhiteSpace(delivery_id))
                Delivery = deliveries?.Where(w => w.concod == delivery_id).FirstOrDefault();
            else
                Delivery = null;
            NotifyPropertyChanged("Deliveries");
        }
    }
    #endregion

    #region Bank
    private BankItem? bank;
    public BankItem? Bank
    {
        get => bank;
        set
        {
            if (value != null)
            {
                if (bank?.ABI != value?.ABI && bank?.CAB != value?.CAB)
                {
                    bank_abi = value?.ABI;
                    bank_cab = value?.CAB;
                    if (!string.IsNullOrWhiteSpace(value?.Account))
                        bank_account = value?.Account;
                }
            }
            else
            {
                bank_abi = null;
                bank_cab = null;
                bank_account = null;
            }
            bank = value;
            NotifyPropertyChanged("Bank");
        }
    }
    private ObservableCollection<BankItem>? banks;
    public ObservableCollection<BankItem>? Banks
    {
        get => banks;
        set
        {
            banks = value;
            if (bank_abi.HasValue && bank_cab.HasValue && banks != null && banks.Count > 0)
            {
                if (banks.Count > 1000)
                    Bank = banks?.Where(w => w.ABI == bank_abi && w.CAB == bank_cab).FirstOrDefault();
                else
                    Bank = banks?.Where(w => w.ABI == bank_abi && w.CAB == bank_cab && w.Account == bank_account).FirstOrDefault();
            }
            else
            {
                Bank = null;
            }
            NotifyPropertyChanged("Banks");
        }
    }
    #endregion

    #region Grid icons tasks
    public bool CanPrint => canceled.HasValue ? false :
                                    commercial_signed.HasValue && !string.IsNullOrWhiteSpace(commercial_signer) &&
                                    management_signed.HasValue && !string.IsNullOrWhiteSpace(management_signer);
    public string CanPrintColor => CanPrint ? "G" : "Y";
    public string PrintToolTip => CanPrint ? "Apri il PDF dell'ordine di acquisto" : "Mancano delle firme per procedere alla stampa dell'ordine di acquisto, verrà stampato come bozza";
    public bool CanSend => canceled.HasValue || closed.HasValue ? false :
                                    commercial_signed.HasValue && !string.IsNullOrWhiteSpace(commercial_signer) &&
                                    management_signed.HasValue && !string.IsNullOrWhiteSpace(management_signer);
    public bool CanTasksVisibility => !canceled.HasValue ? true : false;
    public string CanSendColor => CanSend ? "G" : "O";

    public string SendToolTip => CanSend ? "Invia l'ordine di acquisto tramite email" : closed.HasValue ? "Impossibile inviare un ordine chiuso" : "Mancano delle firme per procedere all'invio dell'ordine di acquisto";
    public string SignToolTip
    {
        get
        {
            if (canceled.HasValue)
                return "Ordine di acquisto annullato";
            if (closed.HasValue)
                return "Ordine di acquisto chiuso";
            if (supplier_id == 0)
                return "Ordine senza fornitore";
            if (string.IsNullOrWhiteSpace(payment_id))
                return "Ordine senza pagamento";
            if (TotalAmount == 0)
                return "Ordine senza righe o con righe senza prezzi";
            if (!commercial_signed.HasValue && string.IsNullOrWhiteSpace(commercial_signer) &&
                !management_signed.HasValue && string.IsNullOrWhiteSpace(management_signer))
            {
                return "Clicca per apporre la firma commerciale e mandare alla firma direzionale";
            }
            else
            {
                if (commercial_signed.HasValue && !string.IsNullOrWhiteSpace(commercial_signer) &&
                    !management_signed.HasValue && string.IsNullOrWhiteSpace(management_signer))
                {
                    return "Clicca per apporre la firma direzionale e confermare l'ordine di acquisto";
                }
                else
                {
                    return "L'ordine di acquisto presenta tutte le firme necessarie";
                }
            }
        }
    }
    public string SignColor
    {
        get
        {
            if (supplier_id == 0 || string.IsNullOrWhiteSpace(payment_id) || TotalAmount == 0)
                return "O";
            if (!commercial_signed.HasValue && string.IsNullOrWhiteSpace(commercial_signer) &&
                !management_signed.HasValue && string.IsNullOrWhiteSpace(management_signer))
            {
                return  "G";
            }
            else
            {
                if (commercial_signed.HasValue && !string.IsNullOrWhiteSpace(commercial_signer) &&
                    !management_signed.HasValue && string.IsNullOrWhiteSpace(management_signer))
                {
                    return  "Y";
                }
                else
                {
                    if (commercial_signed.HasValue && !string.IsNullOrWhiteSpace(commercial_signer) &&
                        management_signed.HasValue && !string.IsNullOrWhiteSpace(management_signer))
                    {
                        return "W";
                    }
                    else
                    {
                        return "O";
                    }
                }
            }
        }
    }
    //public Cursor SignCursor
    //{
    //    get
    //    {
    //        if (commercial_signed.HasValue && !string.IsNullOrWhiteSpace(commercial_signer) &&
    //            management_signed.HasValue && !string.IsNullOrWhiteSpace(management_signer) || closed.HasValue || canceled.HasValue ||
    //            supplier_id == 0 || string.IsNullOrWhiteSpace(payment_id) || TotalAmount == 0)
    //        {
    //            return null;
    //        }
    //        else
    //        { return Cursors.Hand; }
    //    }
    //}
    #endregion

    #region Attachments
    // allegati
    public ObservableCollection<acq_orders_heads_attachments>? Attachments { get; set; }
    public string OfferAttachmentsStatusColor => Attachments != null && Attachments.Count > 0 ? "G": "O";
    public string OfferAttachmentsStatusText => Attachments != null && Attachments.Count > 0 ? $"Visualizza gli allegati a questo ordine di acquisto" : "Clicca per aggiungere allegati a questo ordine di acquisto";
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
