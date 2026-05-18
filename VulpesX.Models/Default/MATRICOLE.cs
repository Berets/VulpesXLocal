namespace VulpesX.Models.Default;
 
public partial class MATRICOLE : Base 
{
	private string _matsoc = null!;
	public required string matsoc { get => _matsoc; set { if (_matsoc != value) { _matsoc = value; NotifyPropertyChanged();} } }
	private string _matcod = null!;
	public required string matcod { get => _matcod; set { if (_matcod != value) { _matcod = value; NotifyPropertyChanged();} } }
	private string? _matnom;
	public string? matnom { get => _matnom; set { if (_matnom != value) { _matnom = value; NotifyPropertyChanged();} } }
	private string? _mattma;
	public string? mattma { get => _mattma; set { if (_mattma != value) { _mattma = value; NotifyPropertyChanged();} } }
	private string? _matrep;
	public string? matrep { get => _matrep; set { if (_matrep != value) { _matrep = value; NotifyPropertyChanged();} } }
	private DateTime? _matdat;
	public DateTime? matdat { get => _matdat; set { if (_matdat != value) { _matdat = value; NotifyPropertyChanged();} } }
	private int? _matori;
	public int? matori { get => _matori; set { if (_matori != value) { _matori = value; NotifyPropertyChanged();} } }
	private int? _matorf;
	public int? matorf { get => _matorf; set { if (_matorf != value) { _matorf = value; NotifyPropertyChanged();} } }
	private int? _matoip;
	public int? matoip { get => _matoip; set { if (_matoip != value) { _matoip = value; NotifyPropertyChanged();} } }
	private int? _matofp;
	public int? matofp { get => _matofp; set { if (_matofp != value) { _matofp = value; NotifyPropertyChanged();} } }
	private decimal? _mattpp;
	public decimal? mattpp { get => _mattpp; set { if (_mattpp != value) { _mattpp = value; NotifyPropertyChanged();} } }
	private string? _matso1;
	public string? matso1 { get => _matso1; set { if (_matso1 != value) { _matso1 = value; NotifyPropertyChanged();} } }
	private string? _matso2;
	public string? matso2 { get => _matso2; set { if (_matso2 != value) { _matso2 = value; NotifyPropertyChanged();} } }
}