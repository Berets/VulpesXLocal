namespace VulpesX.Models.Default;
 
public partial class DWH_Query : Base 
{
	private string _SocietaID = null!;
	public required string SocietaID { get => _SocietaID; set { if (_SocietaID != value) { _SocietaID = value; NotifyPropertyChanged();} } }
	private Guid _ID;
	public Guid ID { get => _ID; set { if (_ID != value) { _ID = value; NotifyPropertyChanged();} } }
	private string? _Titolo;
	public string? Titolo { get => _Titolo; set { if (_Titolo != value) { _Titolo = value; NotifyPropertyChanged();} } }
	private string? _Query;
	public string? Query { get => _Query; set { if (_Query != value) { _Query = value; NotifyPropertyChanged();} } }
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
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private bool? _EStoredProcedure;
	public bool? EStoredProcedure { get => _EStoredProcedure; set { if (_EStoredProcedure != value) { _EStoredProcedure = value; NotifyPropertyChanged();} } }
	private string? _StoredProcedureName;
	public string? StoredProcedureName { get => _StoredProcedureName; set { if (_StoredProcedureName != value) { _StoredProcedureName = value; NotifyPropertyChanged();} } }
    private string? _description;
    public string? description { get => _description; set { if (_description != value) { _description = value; NotifyPropertyChanged(); } } }
}