namespace VulpesX.Models.Default;
 
public partial class ORDITALE : Base 
{
	private string _otsoci = null!;
	public required string otsoci { get => _otsoci; set { if (_otsoci != value) { _otsoci = value; NotifyPropertyChanged();} } }
	private int _OTANNO;
	public int OTANNO { get => _OTANNO; set { if (_OTANNO != value) { _OTANNO = value; NotifyPropertyChanged();} } }
	private int _OTNUOR;
	public int OTNUOR { get => _OTNUOR; set { if (_OTNUOR != value) { _OTNUOR = value; NotifyPropertyChanged();} } }
	private int _OTAID;
	public int OTAID { get => _OTAID; set { if (_OTAID != value) { _OTAID = value; NotifyPropertyChanged();} } }
	private string _OTADESC = null!;
	public required string OTADESC { get => _OTADESC; set { if (_OTADESC != value) { _OTADESC = value; NotifyPropertyChanged();} } }
	private bool _OTAATTI;
	public bool OTAATTI { get => _OTAATTI; set { if (_OTAATTI != value) { _OTAATTI = value; NotifyPropertyChanged();} } }
	private DateTime? _added;
	public DateTime? added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private DateTime? _updated;
	public DateTime? updated { get => _updated; set { if (_updated != value) { _updated = value; NotifyPropertyChanged();} } }
	private string? _addedUserID;
	public string? addedUserID { get => _addedUserID; set { if (_addedUserID != value) { _addedUserID = value; NotifyPropertyChanged();} } }
	private string? _updatedUserID;
	public string? updatedUserID { get => _updatedUserID; set { if (_updatedUserID != value) { _updatedUserID = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}