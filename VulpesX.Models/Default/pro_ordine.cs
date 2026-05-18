namespace VulpesX.Models.Default;
 
public partial class pro_ordine : Base 
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
	private int? _ClienteID;
	public int? ClienteID { get => _ClienteID; set { if (_ClienteID != value) { _ClienteID = value; NotifyPropertyChanged();} } }
	private string? _ArticoloID;
	public string? ArticoloID { get => _ArticoloID; set { if (_ArticoloID != value) { _ArticoloID = value; NotifyPropertyChanged();} } }
	private string? _RevisioneID;
	public string? RevisioneID { get => _RevisioneID; set { if (_RevisioneID != value) { _RevisioneID = value; NotifyPropertyChanged();} } }
	private DateTime? _DataOrdine;
	public DateTime? DataOrdine { get => _DataOrdine; set { if (_DataOrdine != value) { _DataOrdine = value; NotifyPropertyChanged();} } }
	private DateTime? _DataConsegna;
	public DateTime? DataConsegna { get => _DataConsegna; set { if (_DataConsegna != value) { _DataConsegna = value; NotifyPropertyChanged();} } }
	private decimal? _Quantita;
	public decimal? Quantita { get => _Quantita; set { if (_Quantita != value) { _Quantita = value; NotifyPropertyChanged();} } }
	private string? _Stato;
	public string? Stato { get => _Stato; set { if (_Stato != value) { _Stato = value; NotifyPropertyChanged();} } }
	private string? _Commessa;
	public string? Commessa { get => _Commessa; set { if (_Commessa != value) { _Commessa = value; NotifyPropertyChanged();} } }
	private string? _CommessaInterna;
	public string? CommessaInterna { get => _CommessaInterna; set { if (_CommessaInterna != value) { _CommessaInterna = value; NotifyPropertyChanged();} } }
	private string? _Note;
	public string? Note { get => _Note; set { if (_Note != value) { _Note = value; NotifyPropertyChanged();} } }
	private int? _OrdineClienteAnno;
	public int? OrdineClienteAnno { get => _OrdineClienteAnno; set { if (_OrdineClienteAnno != value) { _OrdineClienteAnno = value; NotifyPropertyChanged();} } }
	private long? _OrdineClienteID;
	public long? OrdineClienteID { get => _OrdineClienteID; set { if (_OrdineClienteID != value) { _OrdineClienteID = value; NotifyPropertyChanged();} } }
	private long? _OrdineClienteRiga;
	public long? OrdineClienteRiga { get => _OrdineClienteRiga; set { if (_OrdineClienteRiga != value) { _OrdineClienteRiga = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}