namespace VulpesX.Models.Default;
 
public partial class CLPTA00F : Base 
{
	private string _clpcod = null!;
	public required string clpcod { get => _clpcod; set { if (_clpcod != value) { _clpcod = value; NotifyPropertyChanged();} } }
	private string? _clpdes;
	public string? clpdes { get => _clpdes; set { if (_clpdes != value) { _clpdes = value; NotifyPropertyChanged();} } }
	private string? _clpatt;
	public string? clpatt { get => _clpatt; set { if (_clpatt != value) { _clpatt = value; NotifyPropertyChanged();} } }
	private string? _clpusri;
	public string? clpusri { get => _clpusri; set { if (_clpusri != value) { _clpusri = value; NotifyPropertyChanged();} } }
	private string? _clpusrv;
	public string? clpusrv { get => _clpusrv; set { if (_clpusrv != value) { _clpusrv = value; NotifyPropertyChanged();} } }
	private string? _clptimi;
	public string? clptimi { get => _clptimi; set { if (_clptimi != value) { _clptimi = value; NotifyPropertyChanged();} } }
	private string? _clptimv;
	public string? clptimv { get => _clptimv; set { if (_clptimv != value) { _clptimv = value; NotifyPropertyChanged();} } }
	private string? _clpterm;
	public string? clpterm { get => _clpterm; set { if (_clpterm != value) { _clpterm = value; NotifyPropertyChanged();} } }
}