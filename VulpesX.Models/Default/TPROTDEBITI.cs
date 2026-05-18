namespace VulpesX.Models.Default;
 
public partial class TPROTDEBITI : Base 
{
	private string _prodebsoc = null!;
	public required string prodebsoc { get => _prodebsoc; set { if (_prodebsoc != value) { _prodebsoc = value; NotifyPropertyChanged();} } }
	private int _prodebanno;
	public int prodebanno { get => _prodebanno; set { if (_prodebanno != value) { _prodebanno = value; NotifyPropertyChanged();} } }
	private int _prodebprog;
	public int prodebprog { get => _prodebprog; set { if (_prodebprog != value) { _prodebprog = value; NotifyPropertyChanged();} } }
	private DateTime? _prodebdatarr;
	public DateTime? prodebdatarr { get => _prodebdatarr; set { if (_prodebdatarr != value) { _prodebdatarr = value; NotifyPropertyChanged();} } }
	private DateTime? _prodebdatdoc;
	public DateTime? prodebdatdoc { get => _prodebdatdoc; set { if (_prodebdatdoc != value) { _prodebdatdoc = value; NotifyPropertyChanged();} } }
	private string? _prodebnumdoc;
	public string? prodebnumdoc { get => _prodebnumdoc; set { if (_prodebnumdoc != value) { _prodebnumdoc = value; NotifyPropertyChanged();} } }
	private string? _prodebnote;
	public string? prodebnote { get => _prodebnote; set { if (_prodebnote != value) { _prodebnote = value; NotifyPropertyChanged();} } }
	private string? _prodebiban;
	public string? prodebiban { get => _prodebiban; set { if (_prodebiban != value) { _prodebiban = value; NotifyPropertyChanged();} } }
	private string? _prodebtippag;
	public string? prodebtippag { get => _prodebtippag; set { if (_prodebtippag != value) { _prodebtippag = value; NotifyPropertyChanged();} } }
	private int? _prodebnumreg;
	public int? prodebnumreg { get => _prodebnumreg; set { if (_prodebnumreg != value) { _prodebnumreg = value; NotifyPropertyChanged();} } }
	private int? _prodebannoreg;
	public int? prodebannoreg { get => _prodebannoreg; set { if (_prodebannoreg != value) { _prodebannoreg = value; NotifyPropertyChanged();} } }
	private decimal? _prodebimppag;
	public decimal? prodebimppag { get => _prodebimppag; set { if (_prodebimppag != value) { _prodebimppag = value; NotifyPropertyChanged();} } }
	private decimal? _prodebimpdoc;
	public decimal? prodebimpdoc { get => _prodebimpdoc; set { if (_prodebimpdoc != value) { _prodebimpdoc = value; NotifyPropertyChanged();} } }
	private int? _prodebfor;
	public int? prodebfor { get => _prodebfor; set { if (_prodebfor != value) { _prodebfor = value; NotifyPropertyChanged();} } }
	private int? _prodebcont;
	public int? prodebcont { get => _prodebcont; set { if (_prodebcont != value) { _prodebcont = value; NotifyPropertyChanged();} } }
	private string? _prodebtipdoc;
	public string? prodebtipdoc { get => _prodebtipdoc; set { if (_prodebtipdoc != value) { _prodebtipdoc = value; NotifyPropertyChanged();} } }
	private string? _prodebdocext;
	public string? prodebdocext { get => _prodebdocext; set { if (_prodebdocext != value) { _prodebdocext = value; NotifyPropertyChanged();} } }
	private string? _prodebdocnom;
	public string? prodebdocnom { get => _prodebdocnom; set { if (_prodebdocnom != value) { _prodebdocnom = value; NotifyPropertyChanged();} } }
	private byte[]? _prodebdoc;
	public byte[]? prodebdoc { get => _prodebdoc; set { if (_prodebdoc != value) { _prodebdoc = value; NotifyPropertyChanged();} } }
	private int? _prodebnumregpag;
	public int? prodebnumregpag { get => _prodebnumregpag; set { if (_prodebnumregpag != value) { _prodebnumregpag = value; NotifyPropertyChanged();} } }
	private string? _prodebforloc;
	public string? prodebforloc { get => _prodebforloc; set { if (_prodebforloc != value) { _prodebforloc = value; NotifyPropertyChanged();} } }
	private string? _prodebfordes;
	public string? prodebfordes { get => _prodebfordes; set { if (_prodebfordes != value) { _prodebfordes = value; NotifyPropertyChanged();} } }
	private string? _prodebpiva;
	public string? prodebpiva { get => _prodebpiva; set { if (_prodebpiva != value) { _prodebpiva = value; NotifyPropertyChanged();} } }
	private decimal? _prodebimprit;
	public decimal? prodebimprit { get => _prodebimprit; set { if (_prodebimprit != value) { _prodebimprit = value; NotifyPropertyChanged();} } }
	private decimal? _prodebimpiva;
	public decimal? prodebimpiva { get => _prodebimpiva; set { if (_prodebimpiva != value) { _prodebimpiva = value; NotifyPropertyChanged();} } }
	private decimal? _prodebimponibile;
	public decimal? prodebimponibile { get => _prodebimponibile; set { if (_prodebimponibile != value) { _prodebimponibile = value; NotifyPropertyChanged();} } }
	private string? _prodebcontab;
	public string? prodebcontab { get => _prodebcontab; set { if (_prodebcontab != value) { _prodebcontab = value; NotifyPropertyChanged();} } }
	private string? _prodebflgdocarr;
	public string? prodebflgdocarr { get => _prodebflgdocarr; set { if (_prodebflgdocarr != value) { _prodebflgdocarr = value; NotifyPropertyChanged();} } }
	private int? _prodebdocprovv;
	public int? prodebdocprovv { get => _prodebdocprovv; set { if (_prodebdocprovv != value) { _prodebdocprovv = value; NotifyPropertyChanged();} } }
	private string? _prodebcon;
	public string? prodebcon { get => _prodebcon; set { if (_prodebcon != value) { _prodebcon = value; NotifyPropertyChanged();} } }
	private int? _prodebcab;
	public int? prodebcab { get => _prodebcab; set { if (_prodebcab != value) { _prodebcab = value; NotifyPropertyChanged();} } }
	private int? _prodebabi;
	public int? prodebabi { get => _prodebabi; set { if (_prodebabi != value) { _prodebabi = value; NotifyPropertyChanged();} } }
}