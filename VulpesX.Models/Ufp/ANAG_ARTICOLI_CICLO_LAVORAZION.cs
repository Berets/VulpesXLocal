namespace VulpesX.Models.Ufp;
using VulpesX.Shared;
public partial class ANAG_ARTICOLI_CICLO_LAVORAZION : Base 
{
	private string _ARTCOD = null!;
	public required string ARTCOD { get => _ARTCOD; set { if (_ARTCOD != value) { _ARTCOD = value; NotifyPropertyChanged();} } }
	private string _artciccod = null!;
	public required string artciccod { get => _artciccod; set { if (_artciccod != value) { _artciccod = value; NotifyPropertyChanged();} } }
	private int _artcicseq;
	public int artcicseq { get => _artcicseq; set { if (_artcicseq != value) { _artcicseq = value; NotifyPropertyChanged();} } }
	private string _artcicfas = null!;
	public required string artcicfas { get => _artcicfas; set { if (_artcicfas != value) { _artcicfas = value; NotifyPropertyChanged();} } }
	private string _artcicrep = null!;
	public required string artcicrep { get => _artcicrep; set { if (_artcicrep != value) { _artcicrep = value; NotifyPropertyChanged();} } }
	private string _artcicfassoc = null!;
	public required string artcicfassoc { get => _artcicfassoc; set { if (_artcicfassoc != value) { _artcicfassoc = value; NotifyPropertyChanged();} } }
	private string _artcicrepsoc = null!;
	public required string artcicrepsoc { get => _artcicrepsoc; set { if (_artcicrepsoc != value) { _artcicrepsoc = value; NotifyPropertyChanged();} } }
	private string? _artmacsoc;
	public string? artmacsoc { get => _artmacsoc; set { if (_artmacsoc != value) { _artmacsoc = value; NotifyPropertyChanged();} } }
	private string? _artmaccod;
	public string? artmaccod { get => _artmaccod; set { if (_artmaccod != value) { _artmaccod = value; NotifyPropertyChanged();} } }
	private int _artcicats;
	public int artcicats { get => _artcicats; set { if (_artcicats != value) { _artcicats = value; NotifyPropertyChanged();} } }
	private int _artcicatm;
	public int artcicatm { get => _artcicatm; set { if (_artcicatm != value) { _artcicatm = value; NotifyPropertyChanged();} } }
	private int _artcicath;
	public int artcicath { get => _artcicath; set { if (_artcicath != value) { _artcicath = value; NotifyPropertyChanged();} } }
	private int _artcicmas;
	public int artcicmas { get => _artcicmas; set { if (_artcicmas != value) { _artcicmas = value; NotifyPropertyChanged();} } }
	private int _artcicmam;
	public int artcicmam { get => _artcicmam; set { if (_artcicmam != value) { _artcicmam = value; NotifyPropertyChanged();} } }
	private int _artcicmah;
	public int artcicmah { get => _artcicmah; set { if (_artcicmah != value) { _artcicmah = value; NotifyPropertyChanged();} } }
	private int _artcicmds;
	public int artcicmds { get => _artcicmds; set { if (_artcicmds != value) { _artcicmds = value; NotifyPropertyChanged();} } }
	private int _artcicmdm;
	public int artcicmdm { get => _artcicmdm; set { if (_artcicmdm != value) { _artcicmdm = value; NotifyPropertyChanged();} } }
	private int _artcicmdh;
	public int artcicmdh { get => _artcicmdh; set { if (_artcicmdh != value) { _artcicmdh = value; NotifyPropertyChanged();} } }
	private string? _artcictfa;
	public string? artcictfa { get => _artcictfa; set { if (_artcictfa != value) { _artcictfa = value; NotifyPropertyChanged();} } }
}