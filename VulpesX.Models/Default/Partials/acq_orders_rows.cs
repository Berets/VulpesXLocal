using System.Collections.ObjectModel;
using System.ComponentModel;
using VulpesX.Shared.Utilities;


namespace VulpesX.Models.Default;

public partial class acq_orders_rows
{
    #region Dynamic refresh
    public acq_orders_rows()
    {
        PropertyChanged += acq_orders_rows_PropertyChanged;
    }

    private void acq_orders_rows_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "price" || e.PropertyName == "price_type" || e.PropertyName == "quantity" ||
            e.PropertyName == "discount" || e.PropertyName == "discount_type")
        {
            NotifyPropertyChanged("Amount");
            NotifyPropertyChanged("NetAmount");
        }
    }
    #endregion

    public event EventHandler? ProductOrQuantityValueChanged;
    protected void OnProductOrQuantityValueChanged(EventArgs e)
    {
        EventHandler? handler = ProductOrQuantityValueChanged;
        if (handler != null)
            handler(this, e);
    }

    public DateTime OrderDate { get; set; }
    public int SupplierID { get; set; }
    public ObservableCollection<acq_orders_rows_rdas>? RDAs { get; set; }
    public ObservableCollection<acq_orders_rows_jobs>? Jobs { get; set; }
    public ObservableCollection<acq_orders_rows_customer_orders>? CustomerOrders { get; set; }

    public ObservableCollection<GenericIDDescription> DiscountTypes => CommonsService.StandardValueTypes;
    public ObservableCollection<GenericIDDescription> PriceTypes => CommonsService.StandardPriceTypes;
    public string? PriceTypeText => PriceTypes.Where(w => w.ID == price_type).FirstOrDefault()?.Description;
    public ObservableCollection<GenericIDDescription>? UMs { get; set; }
    public ObservableCollection<GenericIDDescription>? UMsCache { get; set; }
    public ObservableCollection<tab_articolo_tipo>? ProductTypes { get; set; }
    #region QuantityValue
    public decimal? QuantityValue
    {
        get => quantity;
        set
        {
            quantity = value;
            NotifyPropertyChanged("QuantityValue");
            OnProductOrQuantityValueChanged(EventArgs.Empty);
        }
    }
    #endregion

    #region Print
    public string? SupplierCode { get; set; }
    #endregion

    #region Computed
    public decimal Amount
    {
        get
        {
            if (price_type == "U")
            {
                return (price ?? 0) * (quantity ?? 0);
            }
            else
            {
                if (price_type == "V")
                {
                    return price ?? 0;
                }
            }
            return 0;
        }
    }
    public decimal NetAmount => AccountingHelper.ComputePrice(Amount, discount, discount_type, null, null, null, null, null, null).NetPrice;
    public decimal VAT
    {
        get
        {
            decimal rate = 0;
            decimal.TryParse(tax_rate, out rate);
            return NetAmount * rate / 100;
        }
    }
    public string DiscountText => $"{(discount.HasValue && discount.Value > 0 ? discount.Value.ToString("N2") + (discount_type == "V" ? "€" : "%") : " ")}";
    #endregion

    #region Product
    private tab_articolo? product;
    public tab_articolo? Product
    {
        get => product;
        set
        {
            if (product?.ID != value?.ID && UMsCache != null)
            {
                // load UMs
                UMs = new ObservableCollection<GenericIDDescription>(UMsCache.Where(w => w.ID == value?.UnitaID || w.ID == value?.UnitaIDAlt));
                unit_id = product != null || string.IsNullOrWhiteSpace(unit_id) ? value?.UnitaID : unit_id;
                if (value != null)
                {
                    // get default rate
                    if (string.IsNullOrWhiteSpace(tax_code) && string.IsNullOrWhiteSpace(tax_rate))
                        Rate = rates?.Where(w => w.asscod == value.asscod && w.assali == value.assali).FirstOrDefault();
                    // get default store
                    if (string.IsNullOrWhiteSpace(store_id))
                    {
                        Store = stores?.Where(w => w.id == ProductTypes?.Where(w => w.ID == value?.TipoID).FirstOrDefault()?.DefaultMagazzinoID).FirstOrDefault();
                    }
                }
                product_id = value?.ID;
                product = value;
                NotifyPropertyChanged("Product");
                NotifyPropertyChanged("unit_id");
                NotifyPropertyChanged("UMs");
                OnProductOrQuantityValueChanged(EventArgs.Empty);
            }
        }
    }

    private ObservableCollection<tab_articolo>? products;
    public ObservableCollection<tab_articolo>? Products
    {
        get => products;
        set
        {
            products = value;
            if (!string.IsNullOrWhiteSpace(product_id))
                Product = products?.Where(w => w.ID == product_id).FirstOrDefault();
            else
                Product = null;
            NotifyPropertyChanged("Products");
        }
    }
    #endregion

    #region Store
    private store_stores? store;
    public store_stores? Store
    {
        get => store;
        set
        {
            if (store?.id != value?.id)
            {
                store_id = value?.id;
                store = value;
                NotifyPropertyChanged("Store");
            }
        }
    }

    private ObservableCollection<store_stores>? stores;
    public ObservableCollection<store_stores>? Stores
    {
        get => stores;
        set
        {
            stores = value;
            if (!string.IsNullOrWhiteSpace(store_id))
                Store = stores?.Where(w => w.id == store_id).FirstOrDefault();
            else
                Store = null;
            NotifyPropertyChanged("Stores");
        }
    }
    #endregion

    #region Rate
    private ASSOGGETAMENTI? rate;
    public ASSOGGETAMENTI? Rate
    {
        get => rate;
        set
        {
            if (rate?.asscod != value?.asscod || rate?.assali != value?.assali)
            {
                tax_code = (value != null) ? value.asscod : string.Empty;
                tax_rate = (value != null) ? value.assali : string.Empty;
                rate = value;
                NotifyPropertyChanged("Rate");
            }
        }
    }

    private ObservableCollection<ASSOGGETAMENTI>? rates;
    public ObservableCollection<ASSOGGETAMENTI>? Rates
    {
        get => rates;
        set
        {
            rates = value;
            if (!string.IsNullOrWhiteSpace(tax_code) && !string.IsNullOrWhiteSpace(tax_rate))
                Rate = rates?.Where(w => w.asscod == tax_code && w.assali == tax_rate).FirstOrDefault();
            else
                Rate = null;
            NotifyPropertyChanged("Rates");
        }
    }
    #endregion
}
