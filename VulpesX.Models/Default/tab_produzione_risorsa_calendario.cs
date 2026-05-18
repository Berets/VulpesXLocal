namespace VulpesX.Models.Default;
 
public partial class tab_produzione_risorsa_calendario : Base 
{
	private string _SocietaID = null!;
	public required string SocietaID { get => _SocietaID; set { if (_SocietaID != value) { _SocietaID = value; NotifyPropertyChanged();} } }
	private string _RisorsaID = null!;
	public required string RisorsaID { get => _RisorsaID; set { if (_RisorsaID != value) { _RisorsaID = value; NotifyPropertyChanged();} } }
	private DateTime _Giorno;
	public DateTime Giorno { get => _Giorno; set { if (_Giorno != value) { _Giorno = value; NotifyPropertyChanged();} } }
	private DateTime _Dalle;
	public DateTime Dalle { get => _Dalle; set { if (_Dalle != value) { _Dalle = value; NotifyPropertyChanged();} } }
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
	private DateTime _Alle;
	public DateTime Alle { get => _Alle; set { if (_Alle != value) { _Alle = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private string? _Tipo;
	public string? Tipo { get => _Tipo; set { if (_Tipo != value) { _Tipo = value; NotifyPropertyChanged();} } }
}