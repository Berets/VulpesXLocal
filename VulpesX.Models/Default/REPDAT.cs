namespace VulpesX.Models.Default;
 
public partial class REPDAT : Base 
{
	private string _pdsoci = null!;
	public required string pdsoci { get => _pdsoci; set { if (_pdsoci != value) { _pdsoci = value; NotifyPropertyChanged();} } }
	private int _pdanno;
	public int pdanno { get => _pdanno; set { if (_pdanno != value) { _pdanno = value; NotifyPropertyChanged();} } }
	private int _pdnuop;
	public int pdnuop { get => _pdnuop; set { if (_pdnuop != value) { _pdnuop = value; NotifyPropertyChanged();} } }
	private int _pdnseq;
	public int pdnseq { get => _pdnseq; set { if (_pdnseq != value) { _pdnseq = value; NotifyPropertyChanged();} } }
	private DateTime _pddata;
	public DateTime pddata { get => _pddata; set { if (_pddata != value) { _pddata = value; NotifyPropertyChanged();} } }
	private string? _pdsore;
	public string? pdsore { get => _pdsore; set { if (_pdsore != value) { _pdsore = value; NotifyPropertyChanged();} } }
	private string? _pdrepa;
	public string? pdrepa { get => _pdrepa; set { if (_pdrepa != value) { _pdrepa = value; NotifyPropertyChanged();} } }
	private string? _pdsofa;
	public string? pdsofa { get => _pdsofa; set { if (_pdsofa != value) { _pdsofa = value; NotifyPropertyChanged();} } }
	private string? _pdfase;
	public string? pdfase { get => _pdfase; set { if (_pdfase != value) { _pdfase = value; NotifyPropertyChanged();} } }
	private string? _pdsoma;
	public string? pdsoma { get => _pdsoma; set { if (_pdsoma != value) { _pdsoma = value; NotifyPropertyChanged();} } }
	private string? _pdmacc;
	public string? pdmacc { get => _pdmacc; set { if (_pdmacc != value) { _pdmacc = value; NotifyPropertyChanged();} } }
	private decimal? _pdtemp;
	public decimal? pdtemp { get => _pdtemp; set { if (_pdtemp != value) { _pdtemp = value; NotifyPropertyChanged();} } }
}