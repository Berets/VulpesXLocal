namespace VulpesX.Models.Default;
 
public partial class CLDESFA : Base 
{
	private int _dfcocl;
	public int dfcocl { get => _dfcocl; set { if (_dfcocl != value) { _dfcocl = value; NotifyPropertyChanged();} } }
	private string? _dfrags;
	public string? dfrags { get => _dfrags; set { if (_dfrags != value) { _dfrags = value; NotifyPropertyChanged();} } }
	private string? _dfindi;
	public string? dfindi { get => _dfindi; set { if (_dfindi != value) { _dfindi = value; NotifyPropertyChanged();} } }
	private string? _dfloca;
	public string? dfloca { get => _dfloca; set { if (_dfloca != value) { _dfloca = value; NotifyPropertyChanged();} } }
	private int? _dfcaps;
	public int? dfcaps { get => _dfcaps; set { if (_dfcaps != value) { _dfcaps = value; NotifyPropertyChanged();} } }
	private string? _dfprov;
	public string? dfprov { get => _dfprov; set { if (_dfprov != value) { _dfprov = value; NotifyPropertyChanged();} } }
	private int? _dfcapp;
	public int? dfcapp { get => _dfcapp; set { if (_dfcapp != value) { _dfcapp = value; NotifyPropertyChanged();} } }
}