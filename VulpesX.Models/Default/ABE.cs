namespace VulpesX.Models.Default;
 
public partial class ABE : Base 
{
	private int _abecod;
	public int abecod { get => _abecod; set { if (_abecod != value) { _abecod = value; NotifyPropertyChanged();} } }
	private string? _abers1;
	public string? abers1 { get => _abers1; set { if (_abers1 != value) { _abers1 = value; NotifyPropertyChanged();} } }
	private string? _abers2;
	public string? abers2 { get => _abers2; set { if (_abers2 != value) { _abers2 = value; NotifyPropertyChanged();} } }
	private string _abeind = null!;
	public required string abeind { get => _abeind; set { if (_abeind != value) { _abeind = value; NotifyPropertyChanged();} } }
	private int? _abecap;
	public int? abecap { get => _abecap; set { if (_abecap != value) { _abecap = value; NotifyPropertyChanged();} } }
	private string? _abeloc;
	public string? abeloc { get => _abeloc; set { if (_abeloc != value) { _abeloc = value; NotifyPropertyChanged();} } }
	private string? _abepro;
	public string? abepro { get => _abepro; set { if (_abepro != value) { _abepro = value; NotifyPropertyChanged();} } }
	private string? _nazcod;
	public string? nazcod { get => _nazcod; set { if (_nazcod != value) { _nazcod = value; NotifyPropertyChanged();} } }
	private string? _abetpo;
	public string? abetpo { get => _abetpo; set { if (_abetpo != value) { _abetpo = value; NotifyPropertyChanged();} } }
	private string? _isocod;
	public string? isocod { get => _isocod; set { if (_isocod != value) { _isocod = value; NotifyPropertyChanged();} } }
	private string? _abepiv;
	public string? abepiv { get => _abepiv; set { if (_abepiv != value) { _abepiv = value; NotifyPropertyChanged();} } }
	private string? _abecfi;
	public string? abecfi { get => _abecfi; set { if (_abecfi != value) { _abecfi = value; NotifyPropertyChanged();} } }
	private string? _soctip;
	public string? soctip { get => _soctip; set { if (_soctip != value) { _soctip = value; NotifyPropertyChanged();} } }
	private DateTime? _abedna;
	public DateTime? abedna { get => _abedna; set { if (_abedna != value) { _abedna = value; NotifyPropertyChanged();} } }
	private string? _abelna;
	public string? abelna { get => _abelna; set { if (_abelna != value) { _abelna = value; NotifyPropertyChanged();} } }
	private string? _abepna;
	public string? abepna { get => _abepna; set { if (_abepna != value) { _abepna = value; NotifyPropertyChanged();} } }
	private string? _abesex;
	public string? abesex { get => _abesex; set { if (_abesex != value) { _abesex = value; NotifyPropertyChanged();} } }
	private int? _abeais;
	public int? abeais { get => _abeais; set { if (_abeais != value) { _abeais = value; NotifyPropertyChanged();} } }
	private string? _abeccc;
	public string? abeccc { get => _abeccc; set { if (_abeccc != value) { _abeccc = value; NotifyPropertyChanged();} } }
	private string? _abetri;
	public string? abetri { get => _abetri; set { if (_abetri != value) { _abetri = value; NotifyPropertyChanged();} } }
	private string? _abeinl;
	public string? abeinl { get => _abeinl; set { if (_abeinl != value) { _abeinl = value; NotifyPropertyChanged();} } }
	private int? _abecal;
	public int? abecal { get => _abecal; set { if (_abecal != value) { _abecal = value; NotifyPropertyChanged();} } }
	private string? _abelol;
	public string? abelol { get => _abelol; set { if (_abelol != value) { _abelol = value; NotifyPropertyChanged();} } }
	private string? _abeprl;
	public string? abeprl { get => _abeprl; set { if (_abeprl != value) { _abeprl = value; NotifyPropertyChanged();} } }
	private string? _abecfe;
	public string? abecfe { get => _abecfe; set { if (_abecfe != value) { _abecfe = value; NotifyPropertyChanged();} } }
	private string? _abesomcod;
	public string? abesomcod { get => _abesomcod; set { if (_abesomcod != value) { _abesomcod = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private DateTime? _added;
	public DateTime? added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private DateTime? _updated;
	public DateTime? updated { get => _updated; set { if (_updated != value) { _updated = value; NotifyPropertyChanged();} } }
	private DateTime? _canceled;
	public DateTime? canceled { get => _canceled; set { if (_canceled != value) { _canceled = value; NotifyPropertyChanged();} } }
	private string? _addedUserID;
	public string? addedUserID { get => _addedUserID; set { if (_addedUserID != value) { _addedUserID = value; NotifyPropertyChanged();} } }
	private string? _updatedUserID;
	public string? updatedUserID { get => _updatedUserID; set { if (_updatedUserID != value) { _updatedUserID = value; NotifyPropertyChanged();} } }
	private string? _canceledUserID;
	public string? canceledUserID { get => _canceledUserID; set { if (_canceledUserID != value) { _canceledUserID = value; NotifyPropertyChanged();} } }
	private string? _canceledNote;
	public string? canceledNote { get => _canceledNote; set { if (_canceledNote != value) { _canceledNote = value; NotifyPropertyChanged();} } }
	private bool _abfatfile;
	public bool abfatfile { get => _abfatfile; set { if (_abfatfile != value) { _abfatfile = value; NotifyPropertyChanged();} } }
	private int? _abfatfileid;
	public int? abfatfileid { get => _abfatfileid; set { if (_abfatfileid != value) { _abfatfileid = value; NotifyPropertyChanged();} } }
	private int? _abold;
	public int? abold { get => _abold; set { if (_abold != value) { _abold = value; NotifyPropertyChanged();} } }
}