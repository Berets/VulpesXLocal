namespace VulpesX.Models.Default;
 
public partial class ACCESOCE_TASKS : Base 
{
	private string _societa_id = null!;
	public required string societa_id { get => _societa_id; set { if (_societa_id != value) { _societa_id = value; NotifyPropertyChanged();} } }
	private long _task_id;
	public long task_id { get => _task_id; set { if (_task_id != value) { _task_id = value; NotifyPropertyChanged();} } }
	private long? _task_father_id;
	public long? task_father_id { get => _task_father_id; set { if (_task_father_id != value) { _task_father_id = value; NotifyPropertyChanged();} } }
	private string _utente_id = null!;
	public required string utente_id { get => _utente_id; set { if (_utente_id != value) { _utente_id = value; NotifyPropertyChanged();} } }
	private string? _utenti_id_notifica;
	public string? utenti_id_notifica { get => _utenti_id_notifica; set { if (_utenti_id_notifica != value) { _utenti_id_notifica = value; NotifyPropertyChanged();} } }
	private int? _cliente_id;
	public int? cliente_id { get => _cliente_id; set { if (_cliente_id != value) { _cliente_id = value; NotifyPropertyChanged();} } }
	private DateTime? _giorno;
	public DateTime? giorno { get => _giorno; set { if (_giorno != value) { _giorno = value; NotifyPropertyChanged();} } }
	private bool? _ha_orario;
	public bool? ha_orario { get => _ha_orario; set { if (_ha_orario != value) { _ha_orario = value; NotifyPropertyChanged();} } }
	private long? _inizio;
	public long? inizio { get => _inizio; set { if (_inizio != value) { _inizio = value; NotifyPropertyChanged();} } }
	private long? _fine;
	public long? fine { get => _fine; set { if (_fine != value) { _fine = value; NotifyPropertyChanged();} } }
	private string? _tipo;
	public string? tipo { get => _tipo; set { if (_tipo != value) { _tipo = value; NotifyPropertyChanged();} } }
	private string? _note;
	public string? note { get => _note; set { if (_note != value) { _note = value; NotifyPropertyChanged();} } }
	private bool? _e_completato;
	public bool? e_completato { get => _e_completato; set { if (_e_completato != value) { _e_completato = value; NotifyPropertyChanged();} } }
	private string? _note_completato;
	public string? note_completato { get => _note_completato; set { if (_note_completato != value) { _note_completato = value; NotifyPropertyChanged();} } }
	private string? _tags;
	public string? tags { get => _tags; set { if (_tags != value) { _tags = value; NotifyPropertyChanged();} } }
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
}