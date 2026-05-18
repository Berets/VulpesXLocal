namespace VulpesX.Models.Default;
 
public partial class FOBOLNF : Base 
{
	private string _Fbolso = null!;
	public required string Fbolso { get => _Fbolso; set { if (_Fbolso != value) { _Fbolso = value; NotifyPropertyChanged();} } }
	private int _FBFANN;
	public int FBFANN { get => _FBFANN; set { if (_FBFANN != value) { _FBFANN = value; NotifyPropertyChanged();} } }
	private int _FBFNUO;
	public int FBFNUO { get => _FBFNUO; set { if (_FBFNUO != value) { _FBFNUO = value; NotifyPropertyChanged();} } }
	private string? _FBFATT;
	public string? FBFATT { get => _FBFATT; set { if (_FBFATT != value) { _FBFATT = value; NotifyPropertyChanged();} } }
	private int? _FBFNAZ;
	public int? FBFNAZ { get => _FBFNAZ; set { if (_FBFNAZ != value) { _FBFNAZ = value; NotifyPropertyChanged();} } }
	private string? _FBFTER;
	public string? FBFTER { get => _FBFTER; set { if (_FBFTER != value) { _FBFTER = value; NotifyPropertyChanged();} } }
	private string? _FBFUT1;
	public string? FBFUT1 { get => _FBFUT1; set { if (_FBFUT1 != value) { _FBFUT1 = value; NotifyPropertyChanged();} } }
	private string? _FBFOR1;
	public string? FBFOR1 { get => _FBFOR1; set { if (_FBFOR1 != value) { _FBFOR1 = value; NotifyPropertyChanged();} } }
	private string? _FBFUT2;
	public string? FBFUT2 { get => _FBFUT2; set { if (_FBFUT2 != value) { _FBFUT2 = value; NotifyPropertyChanged();} } }
	private string? _FBFOR2;
	public string? FBFOR2 { get => _FBFOR2; set { if (_FBFOR2 != value) { _FBFOR2 = value; NotifyPropertyChanged();} } }
	private DateTime? _FBFDAO;
	public DateTime? FBFDAO { get => _FBFDAO; set { if (_FBFDAO != value) { _FBFDAO = value; NotifyPropertyChanged();} } }
	private string? _FBFDE1;
	public string? FBFDE1 { get => _FBFDE1; set { if (_FBFDE1 != value) { _FBFDE1 = value; NotifyPropertyChanged();} } }
	private string? _FBFDE2;
	public string? FBFDE2 { get => _FBFDE2; set { if (_FBFDE2 != value) { _FBFDE2 = value; NotifyPropertyChanged();} } }
	private string? _FBFDE3;
	public string? FBFDE3 { get => _FBFDE3; set { if (_FBFDE3 != value) { _FBFDE3 = value; NotifyPropertyChanged();} } }
	private string? _FBFDE4;
	public string? FBFDE4 { get => _FBFDE4; set { if (_FBFDE4 != value) { _FBFDE4 = value; NotifyPropertyChanged();} } }
	private string? _FBFFLA;
	public string? FBFFLA { get => _FBFFLA; set { if (_FBFFLA != value) { _FBFFLA = value; NotifyPropertyChanged();} } }
	private string? _FBFDEC;
	public string? FBFDEC { get => _FBFDEC; set { if (_FBFDEC != value) { _FBFDEC = value; NotifyPropertyChanged();} } }
}