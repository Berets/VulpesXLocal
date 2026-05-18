namespace VulpesX.Models.Default;
 
public partial class CLICONNO : Base 
{
	private string _ccnsoc = null!;
	public required string ccnsoc { get => _ccnsoc; set { if (_ccnsoc != value) { _ccnsoc = value; NotifyPropertyChanged();} } }
	private int _ccncod;
	public int ccncod { get => _ccncod; set { if (_ccncod != value) { _ccncod = value; NotifyPropertyChanged();} } }
	private int _ccriga;
	public int ccriga { get => _ccriga; set { if (_ccriga != value) { _ccriga = value; NotifyPropertyChanged();} } }
	private string? _ccnot1;
	public string? ccnot1 { get => _ccnot1; set { if (_ccnot1 != value) { _ccnot1 = value; NotifyPropertyChanged();} } }
	private string? _ccnot2;
	public string? ccnot2 { get => _ccnot2; set { if (_ccnot2 != value) { _ccnot2 = value; NotifyPropertyChanged();} } }
	private string? _ccnot3;
	public string? ccnot3 { get => _ccnot3; set { if (_ccnot3 != value) { _ccnot3 = value; NotifyPropertyChanged();} } }
	private string? _ccnot4;
	public string? ccnot4 { get => _ccnot4; set { if (_ccnot4 != value) { _ccnot4 = value; NotifyPropertyChanged();} } }
	private string? _ccnot5;
	public string? ccnot5 { get => _ccnot5; set { if (_ccnot5 != value) { _ccnot5 = value; NotifyPropertyChanged();} } }
	private DateTime? _ccdata;
	public DateTime? ccdata { get => _ccdata; set { if (_ccdata != value) { _ccdata = value; NotifyPropertyChanged();} } }
	private DateTime? _ccrico;
	public DateTime? ccrico { get => _ccrico; set { if (_ccrico != value) { _ccrico = value; NotifyPropertyChanged();} } }
	private string? _ccticc;
	public string? ccticc { get => _ccticc; set { if (_ccticc != value) { _ccticc = value; NotifyPropertyChanged();} } }
	private string? _ccute2;
	public string? ccute2 { get => _ccute2; set { if (_ccute2 != value) { _ccute2 = value; NotifyPropertyChanged();} } }
	private string? _ccnote;
	public string? ccnote { get => _ccnote; set { if (_ccnote != value) { _ccnote = value; NotifyPropertyChanged();} } }
	private string? _ccdes;
	public string? ccdes { get => _ccdes; set { if (_ccdes != value) { _ccdes = value; NotifyPropertyChanged();} } }
	private string? _ccazm;
	public string? ccazm { get => _ccazm; set { if (_ccazm != value) { _ccazm = value; NotifyPropertyChanged();} } }
}