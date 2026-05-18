namespace VulpesX.Models.Default;
 
public partial class PERSCREDITI : Base 
{
	private string _pcrkey = null!;
	public required string pcrkey { get => _pcrkey; set { if (_pcrkey != value) { _pcrkey = value; NotifyPropertyChanged();} } }
	private int? _pcrday;
	public int? pcrday { get => _pcrday; set { if (_pcrday != value) { _pcrday = value; NotifyPropertyChanged();} } }
	private string? _pcrcau;
	public string? pcrcau { get => _pcrcau; set { if (_pcrcau != value) { _pcrcau = value; NotifyPropertyChanged();} } }
	private int? _pcrdar;
	public int? pcrdar { get => _pcrdar; set { if (_pcrdar != value) { _pcrdar = value; NotifyPropertyChanged();} } }
	private string? _pcrcac;
	public string? pcrcac { get => _pcrcac; set { if (_pcrcac != value) { _pcrcac = value; NotifyPropertyChanged();} } }
	private int? _pcrdac;
	public int? pcrdac { get => _pcrdac; set { if (_pcrdac != value) { _pcrdac = value; NotifyPropertyChanged();} } }
}