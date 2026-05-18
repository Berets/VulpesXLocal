namespace VulpesX.Models.Default;
 
public partial class TIPTA00F : Base 
{
	private string _tipcod = null!;
	public required string tipcod { get => _tipcod; set { if (_tipcod != value) { _tipcod = value; NotifyPropertyChanged();} } }
	private string? _tipdes;
	public string? tipdes { get => _tipdes; set { if (_tipdes != value) { _tipdes = value; NotifyPropertyChanged();} } }
	private string? _tipatt;
	public string? tipatt { get => _tipatt; set { if (_tipatt != value) { _tipatt = value; NotifyPropertyChanged();} } }
	private string? _tipusri;
	public string? tipusri { get => _tipusri; set { if (_tipusri != value) { _tipusri = value; NotifyPropertyChanged();} } }
	private string? _tipusrv;
	public string? tipusrv { get => _tipusrv; set { if (_tipusrv != value) { _tipusrv = value; NotifyPropertyChanged();} } }
	private string? _tiptimi;
	public string? tiptimi { get => _tiptimi; set { if (_tiptimi != value) { _tiptimi = value; NotifyPropertyChanged();} } }
	private string? _tiptimv;
	public string? tiptimv { get => _tiptimv; set { if (_tiptimv != value) { _tiptimv = value; NotifyPropertyChanged();} } }
	private string? _tipterm;
	public string? tipterm { get => _tipterm; set { if (_tipterm != value) { _tipterm = value; NotifyPropertyChanged();} } }
	private string? _tiricla;
	public string? tiricla { get => _tiricla; set { if (_tiricla != value) { _tiricla = value; NotifyPropertyChanged();} } }
	private string? _tiarti;
	public string? tiarti { get => _tiarti; set { if (_tiarti != value) { _tiarti = value; NotifyPropertyChanged();} } }
	private string? _tipapp;
	public string? tipapp { get => _tipapp; set { if (_tipapp != value) { _tipapp = value; NotifyPropertyChanged();} } }
	private string? _tipgfd;
	public string? tipgfd { get => _tipgfd; set { if (_tipgfd != value) { _tipgfd = value; NotifyPropertyChanged();} } }
	private string? _tivis;
	public string? tivis { get => _tivis; set { if (_tivis != value) { _tivis = value; NotifyPropertyChanged();} } }
}