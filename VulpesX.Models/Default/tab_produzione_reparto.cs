namespace VulpesX.Models.Default;
 
public partial class tab_produzione_reparto : Base 
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
	private long? _TempoDefault;
	public long? TempoDefault { get => _TempoDefault; set { if (_TempoDefault != value) { _TempoDefault = value; NotifyPropertyChanged();} } }
	private bool? _EPrelievo;
	public bool? EPrelievo { get => _EPrelievo; set { if (_EPrelievo != value) { _EPrelievo = value; NotifyPropertyChanged();} } }
	private bool? _EAvanzamentoParziale;
	public bool? EAvanzamentoParziale { get => _EAvanzamentoParziale; set { if (_EAvanzamentoParziale != value) { _EAvanzamentoParziale = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private bool _FineCiclo;
	public bool FineCiclo { get => _FineCiclo; set { if (_FineCiclo != value) { _FineCiclo = value; NotifyPropertyChanged();} } }
}