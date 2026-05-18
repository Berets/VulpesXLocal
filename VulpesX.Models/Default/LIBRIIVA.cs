namespace VulpesX.Models.Default;
 
public partial class LIBRIIVA : Base 
{
	private string _livcod = null!;
	public required string livcod { get => _livcod; set { if (_livcod != value) { _livcod = value; NotifyPropertyChanged();} } }
	private string _livdes = null!;
	public required string livdes { get => _livdes; set { if (_livdes != value) { _livdes = value; NotifyPropertyChanged();} } }
	private string? _livgci;
	public string? livgci { get => _livgci; set { if (_livgci != value) { _livgci = value; NotifyPropertyChanged();} } }
	private string? _livcci;
	public string? livcci { get => _livcci; set { if (_livcci != value) { _livcci = value; NotifyPropertyChanged();} } }
	private string? _livsci;
	public string? livsci { get => _livsci; set { if (_livsci != value) { _livsci = value; NotifyPropertyChanged();} } }
	private string? _livgce;
	public string? livgce { get => _livgce; set { if (_livgce != value) { _livgce = value; NotifyPropertyChanged();} } }
	private string? _livcce;
	public string? livcce { get => _livcce; set { if (_livcce != value) { _livcce = value; NotifyPropertyChanged();} } }
	private string? _livsce;
	public string? livsce { get => _livsce; set { if (_livsce != value) { _livsce = value; NotifyPropertyChanged();} } }
	private string? _livcgi;
	public string? livcgi { get => _livcgi; set { if (_livcgi != value) { _livcgi = value; NotifyPropertyChanged();} } }
	private string? _livtip;
	public string? livtip { get => _livtip; set { if (_livtip != value) { _livtip = value; NotifyPropertyChanged();} } }
	private string? _livaut;
	public string? livaut { get => _livaut; set { if (_livaut != value) { _livaut = value; NotifyPropertyChanged();} } }
	private string? _livcii;
	public string? livcii { get => _livcii; set { if (_livcii != value) { _livcii = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private DateTime? _livulst;
	public DateTime? livulst { get => _livulst; set { if (_livulst != value) { _livulst = value; NotifyPropertyChanged();} } }
	private DateTime? _added;
	public DateTime? added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private string? _addedUserID;
	public string? addedUserID { get => _addedUserID; set { if (_addedUserID != value) { _addedUserID = value; NotifyPropertyChanged();} } }
	private DateTime? _updated;
	public DateTime? updated { get => _updated; set { if (_updated != value) { _updated = value; NotifyPropertyChanged();} } }
	private string? _updatedUserID;
	public string? updatedUserID { get => _updatedUserID; set { if (_updatedUserID != value) { _updatedUserID = value; NotifyPropertyChanged();} } }
	private bool _livliq;
	public bool livliq { get => _livliq; set { if (_livliq != value) { _livliq = value; NotifyPropertyChanged();} } }
	private bool _livili;
	public bool livili { get => _livili; set { if (_livili != value) { _livili = value; NotifyPropertyChanged();} } }
	private bool _livven;
	public bool livven { get => _livven; set { if (_livven != value) { _livven = value; NotifyPropertyChanged();} } }
}