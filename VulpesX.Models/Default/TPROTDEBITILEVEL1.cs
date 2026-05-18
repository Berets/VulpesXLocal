namespace VulpesX.Models.Default;
 
public partial class TPROTDEBITILEVEL1 : Base 
{
	private string _prodebsoc = null!;
	public required string prodebsoc { get => _prodebsoc; set { if (_prodebsoc != value) { _prodebsoc = value; NotifyPropertyChanged();} } }
	private int _prodebanno;
	public int prodebanno { get => _prodebanno; set { if (_prodebanno != value) { _prodebanno = value; NotifyPropertyChanged();} } }
	private int _prodebprog;
	public int prodebprog { get => _prodebprog; set { if (_prodebprog != value) { _prodebprog = value; NotifyPropertyChanged();} } }
	private int _prodebriga;
	public int prodebriga { get => _prodebriga; set { if (_prodebriga != value) { _prodebriga = value; NotifyPropertyChanged();} } }
	private DateTime? _prodebrigadat;
	public DateTime? prodebrigadat { get => _prodebrigadat; set { if (_prodebrigadat != value) { _prodebrigadat = value; NotifyPropertyChanged();} } }
	private decimal? _prodebrigaimp;
	public decimal? prodebrigaimp { get => _prodebrigaimp; set { if (_prodebrigaimp != value) { _prodebrigaimp = value; NotifyPropertyChanged();} } }
	private string? _prodebrigatippag;
	public string? prodebrigatippag { get => _prodebrigatippag; set { if (_prodebrigatippag != value) { _prodebrigatippag = value; NotifyPropertyChanged();} } }
}