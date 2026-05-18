namespace VulpesX.Models.Default;
 
public partial class FOBOLNH : Base 
{
	private string _Fbolso = null!;
	public required string Fbolso { get => _Fbolso; set { if (_Fbolso != value) { _Fbolso = value; NotifyPropertyChanged();} } }
	private int _Fbanno;
	public int Fbanno { get => _Fbanno; set { if (_Fbanno != value) { _Fbanno = value; NotifyPropertyChanged();} } }
	private int _FBNUOR;
	public int FBNUOR { get => _FBNUOR; set { if (_FBNUOR != value) { _FBNUOR = value; NotifyPropertyChanged();} } }
	private string? _FBUTE1;
	public string? FBUTE1 { get => _FBUTE1; set { if (_FBUTE1 != value) { _FBUTE1 = value; NotifyPropertyChanged();} } }
	private string? _FBORA1;
	public string? FBORA1 { get => _FBORA1; set { if (_FBORA1 != value) { _FBORA1 = value; NotifyPropertyChanged();} } }
	private string? _FBUTE2;
	public string? FBUTE2 { get => _FBUTE2; set { if (_FBUTE2 != value) { _FBUTE2 = value; NotifyPropertyChanged();} } }
	private string? _FBORA2;
	public string? FBORA2 { get => _FBORA2; set { if (_FBORA2 != value) { _FBORA2 = value; NotifyPropertyChanged();} } }
	private DateTime? _FBDAOR;
	public DateTime? FBDAOR { get => _FBDAOR; set { if (_FBDAOR != value) { _FBDAOR = value; NotifyPropertyChanged();} } }
	private string? _FBDES1;
	public string? FBDES1 { get => _FBDES1; set { if (_FBDES1 != value) { _FBDES1 = value; NotifyPropertyChanged();} } }
	private string? _FBDES2;
	public string? FBDES2 { get => _FBDES2; set { if (_FBDES2 != value) { _FBDES2 = value; NotifyPropertyChanged();} } }
	private string? _FBDES3;
	public string? FBDES3 { get => _FBDES3; set { if (_FBDES3 != value) { _FBDES3 = value; NotifyPropertyChanged();} } }
	private string? _FBDES4;
	public string? FBDES4 { get => _FBDES4; set { if (_FBDES4 != value) { _FBDES4 = value; NotifyPropertyChanged();} } }
	private string? _FBFLAG;
	public string? FBFLAG { get => _FBFLAG; set { if (_FBFLAG != value) { _FBFLAG = value; NotifyPropertyChanged();} } }
	private string? _FBFDES;
	public string? FBFDES { get => _FBFDES; set { if (_FBFDES != value) { _FBFDES = value; NotifyPropertyChanged();} } }
}