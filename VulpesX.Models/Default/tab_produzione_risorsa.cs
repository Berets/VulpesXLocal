namespace VulpesX.Models.Default;
 
public partial class tab_produzione_risorsa : Base 
{
	private string _SocietaID = null!;
	public required string SocietaID { get => _SocietaID; set { if (_SocietaID != value) { _SocietaID = value; NotifyPropertyChanged();} } }
	private string _ID = null!;
	public required string ID { get => _ID; set { if (_ID != value) { _ID = value; NotifyPropertyChanged();} } }
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
	private string _Descrizione = null!;
	public required string Descrizione { get => _Descrizione; set { if (_Descrizione != value) { _Descrizione = value; NotifyPropertyChanged();} } }
	private long? _PiazzamentoDefault;
	public long? PiazzamentoDefault { get => _PiazzamentoDefault; set { if (_PiazzamentoDefault != value) { _PiazzamentoDefault = value; NotifyPropertyChanged();} } }
	private bool? _EInfinita;
	public bool? EInfinita { get => _EInfinita; set { if (_EInfinita != value) { _EInfinita = value; NotifyPropertyChanged();} } }
	private bool? _EPiazzamento;
	public bool? EPiazzamento { get => _EPiazzamento; set { if (_EPiazzamento != value) { _EPiazzamento = value; NotifyPropertyChanged();} } }
	private bool? _ECompleta;
	public bool? ECompleta { get => _ECompleta; set { if (_ECompleta != value) { _ECompleta = value; NotifyPropertyChanged();} } }
	private bool? _EVersamento;
	public bool? EVersamento { get => _EVersamento; set { if (_EVersamento != value) { _EVersamento = value; NotifyPropertyChanged();} } }
	private bool? _ETempoAlPezzo;
	public bool? ETempoAlPezzo { get => _ETempoAlPezzo; set { if (_ETempoAlPezzo != value) { _ETempoAlPezzo = value; NotifyPropertyChanged();} } }
	private bool? _EPiuOperatori;
	public bool? EPiuOperatori { get => _EPiuOperatori; set { if (_EPiuOperatori != value) { _EPiuOperatori = value; NotifyPropertyChanged();} } }
	private bool? _EPiuPezzi;
	public bool? EPiuPezzi { get => _EPiuPezzi; set { if (_EPiuPezzi != value) { _EPiuPezzi = value; NotifyPropertyChanged();} } }
	private bool? _EAutomatica;
	public bool? EAutomatica { get => _EAutomatica; set { if (_EAutomatica != value) { _EAutomatica = value; NotifyPropertyChanged();} } }
	private bool? _EFornitore;
	public bool? EFornitore { get => _EFornitore; set { if (_EFornitore != value) { _EFornitore = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}