namespace VulpesX.Models.Default;
 
public partial class tab_articolo : Base 
{
	private string _SocietaID = null!;
	public required string SocietaID { get => _SocietaID; set { if (_SocietaID != value) { _SocietaID = value; NotifyPropertyChanged();} } }
	private string _ID = null!;
	public required string ID { get => _ID; set { if (_ID != value) { _ID = value; NotifyPropertyChanged();} } }
	private string _Descrizione = null!;
	public required string Descrizione { get => _Descrizione; set { if (_Descrizione != value) { _Descrizione = value; NotifyPropertyChanged();} } }
	private string? _DescrizioneEstesa;
	public string? DescrizioneEstesa { get => _DescrizioneEstesa; set { if (_DescrizioneEstesa != value) { _DescrizioneEstesa = value; NotifyPropertyChanged();} } }
	private string _TipoID = null!;
	public required string TipoID { get => _TipoID; set { if (_TipoID != value) { _TipoID = value; NotifyPropertyChanged();} } }
	private string? _CategoriaID;
	public string? CategoriaID { get => _CategoriaID; set { if (_CategoriaID != value) { _CategoriaID = value; NotifyPropertyChanged();} } }
	private string? _UnitaID;
	public string? UnitaID { get => _UnitaID; set { if (_UnitaID != value) { _UnitaID = value; NotifyPropertyChanged();} } }
	private string? _UnitaIDAlt;
	public string? UnitaIDAlt { get => _UnitaIDAlt; set { if (_UnitaIDAlt != value) { _UnitaIDAlt = value; NotifyPropertyChanged();} } }
	private decimal? _QuantitaDefault;
	public decimal? QuantitaDefault { get => _QuantitaDefault; set { if (_QuantitaDefault != value) { _QuantitaDefault = value; NotifyPropertyChanged();} } }
	private string? _Note;
	public string? Note { get => _Note; set { if (_Note != value) { _Note = value; NotifyPropertyChanged();} } }
	private string? _GestionaleID;
	public string? GestionaleID { get => _GestionaleID; set { if (_GestionaleID != value) { _GestionaleID = value; NotifyPropertyChanged();} } }
	private DateTime? _LogAdded;
	public DateTime? LogAdded { get => _LogAdded; set { if (_LogAdded != value) { _LogAdded = value; NotifyPropertyChanged();} } }
	private DateTime? _LogUpdated;
	public DateTime? LogUpdated { get => _LogUpdated; set { if (_LogUpdated != value) { _LogUpdated = value; NotifyPropertyChanged();} } }
	private DateTime? _LogCanceled;
	public DateTime? LogCanceled { get => _LogCanceled; set { if (_LogCanceled != value) { _LogCanceled = value; NotifyPropertyChanged();} } }
	private string? _LogAddedUserID;
	public string? LogAddedUserID { get => _LogAddedUserID; set { if (_LogAddedUserID != value) { _LogAddedUserID = value; NotifyPropertyChanged();} } }
	private string? _LogUpdatedUserID;
	public string? LogUpdatedUserID { get => _LogUpdatedUserID; set { if (_LogUpdatedUserID != value) { _LogUpdatedUserID = value; NotifyPropertyChanged();} } }
	private string? _LogCanceledUserID;
	public string? LogCanceledUserID { get => _LogCanceledUserID; set { if (_LogCanceledUserID != value) { _LogCanceledUserID = value; NotifyPropertyChanged();} } }
	private string? _Barcode;
	public string? Barcode { get => _Barcode; set { if (_Barcode != value) { _Barcode = value; NotifyPropertyChanged();} } }
	private decimal? _conversion_factor;
	public decimal? conversion_factor { get => _conversion_factor; set { if (_conversion_factor != value) { _conversion_factor = value; NotifyPropertyChanged();} } }
	private decimal? _conversion_factor_alt;
	public decimal? conversion_factor_alt { get => _conversion_factor_alt; set { if (_conversion_factor_alt != value) { _conversion_factor_alt = value; NotifyPropertyChanged();} } }
	private decimal? _store_min_stock;
	public decimal? store_min_stock { get => _store_min_stock; set { if (_store_min_stock != value) { _store_min_stock = value; NotifyPropertyChanged();} } }
	private decimal? _store_restock_qty;
	public decimal? store_restock_qty { get => _store_restock_qty; set { if (_store_restock_qty != value) { _store_restock_qty = value; NotifyPropertyChanged();} } }
	private int? _default_supplier_id;
	public int? default_supplier_id { get => _default_supplier_id; set { if (_default_supplier_id != value) { _default_supplier_id = value; NotifyPropertyChanged();} } }
	private string? _asscod;
	public string? asscod { get => _asscod; set { if (_asscod != value) { _asscod = value; NotifyPropertyChanged();} } }
	private string? _assali;
	public string? assali { get => _assali; set { if (_assali != value) { _assali = value; NotifyPropertyChanged();} } }
	private bool? _is_customer_material;
	public bool? is_customer_material { get => _is_customer_material; set { if (_is_customer_material != value) { _is_customer_material = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private int? _ExpireDays;
	public int? ExpireDays { get => _ExpireDays; set { if (_ExpireDays != value) { _ExpireDays = value; NotifyPropertyChanged();} } }
	private string? _GroupID;
	public string? GroupID { get => _GroupID; set { if (_GroupID != value) { _GroupID = value; NotifyPropertyChanged();} } }
	private string? _AccountID;
	public string? AccountID { get => _AccountID; set { if (_AccountID != value) { _AccountID = value; NotifyPropertyChanged();} } }
	private string? _SubaccountID;
	public string? SubaccountID { get => _SubaccountID; set { if (_SubaccountID != value) { _SubaccountID = value; NotifyPropertyChanged();} } }
	private string? _LogCanceledReason;
	public string? LogCanceledReason { get => _LogCanceledReason; set { if (_LogCanceledReason != value) { _LogCanceledReason = value; NotifyPropertyChanged();} } }
	private decimal? _NetWeight;
	public decimal? NetWeight { get => _NetWeight; set { if (_NetWeight != value) { _NetWeight = value; NotifyPropertyChanged();} } }
	private decimal? _GrossWeight;
	public decimal? GrossWeight { get => _GrossWeight; set { if (_GrossWeight != value) { _GrossWeight = value; NotifyPropertyChanged();} } }
	private bool _HasLots;
	public bool HasLots { get => _HasLots; set { if (_HasLots != value) { _HasLots = value; NotifyPropertyChanged();} } }
	private string? _DefaultRevisionID;
	public string? DefaultRevisionID { get => _DefaultRevisionID; set { if (_DefaultRevisionID != value) { _DefaultRevisionID = value; NotifyPropertyChanged();} } }
	private bool _IsInfinite;
	public bool IsInfinite { get => _IsInfinite; set { if (_IsInfinite != value) { _IsInfinite = value; NotifyPropertyChanged();} } }
	private decimal? _store_warning_qty;
	public decimal? store_warning_qty { get => _store_warning_qty; set { if (_store_warning_qty != value) { _store_warning_qty = value; NotifyPropertyChanged();} } }
	private decimal? _packaging_qty;
	public decimal? packaging_qty { get => _packaging_qty; set { if (_packaging_qty != value) { _packaging_qty = value; NotifyPropertyChanged();} } }
	private string? _costcenter_id;
	public string? costcenter_id { get => _costcenter_id; set { if (_costcenter_id != value) { _costcenter_id = value; NotifyPropertyChanged();} } }
	private string? _cost_group_id;
	public string? cost_group_id { get => _cost_group_id; set { if (_cost_group_id != value) { _cost_group_id = value; NotifyPropertyChanged();} } }
	private string? _cost_account_id;
	public string? cost_account_id { get => _cost_account_id; set { if (_cost_account_id != value) { _cost_account_id = value; NotifyPropertyChanged();} } }
	private string? _cost_subaccount_id;
	public string? cost_subaccount_id { get => _cost_subaccount_id; set { if (_cost_subaccount_id != value) { _cost_subaccount_id = value; NotifyPropertyChanged();} } }
}