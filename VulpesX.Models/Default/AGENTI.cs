namespace VulpesX.Models.Default;
 
public partial class AGENTI : Base 
{
	private string _agecod = null!;
	public required string agecod { get => _agecod; set { if (_agecod != value) { _agecod = value; NotifyPropertyChanged();} } }
	private string _agedes = null!;
	public required string agedes { get => _agedes; set { if (_agedes != value) { _agedes = value; NotifyPropertyChanged();} } }
	private decimal? _agepvg;
	public decimal? agepvg { get => _agepvg; set { if (_agepvg != value) { _agepvg = value; NotifyPropertyChanged();} } }
	private string? _agecal;
	public string? agecal { get => _agecal; set { if (_agecal != value) { _agecal = value; NotifyPropertyChanged();} } }
	private string? _ageliq;
	public string? ageliq { get => _ageliq; set { if (_ageliq != value) { _ageliq = value; NotifyPropertyChanged();} } }
	private string _ageflag = null!;
	public required string ageflag { get => _ageflag; set { if (_ageflag != value) { _ageflag = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private bool _agecap;
	public bool agecap { get => _agecap; set { if (_agecap != value) { _agecap = value; NotifyPropertyChanged();} } }
	private int? _agefor;
	public int? agefor { get => _agefor; set { if (_agefor != value) { _agefor = value; NotifyPropertyChanged();} } }
	private string? _agepvgt;
	public string? agepvgt { get => _agepvgt; set { if (_agepvgt != value) { _agepvgt = value; NotifyPropertyChanged();} } }
    private DateTime? _LogCanceled;
    public DateTime? LogCanceled { get => _LogCanceled; set { if (_LogCanceled != value) { _LogCanceled = value; NotifyPropertyChanged(); } } }
    private string? _LogCanceledUserID;
    public string? LogCanceledUserID { get => _LogCanceledUserID; set { if (_LogCanceledUserID != value) { _LogCanceledUserID = value; NotifyPropertyChanged(); } } }
}