namespace VulpesX.Models.Default;
 
public partial class AMMCOND : Base 
{
	private string _ammcod = null!;
	public required string ammcod { get => _ammcod; set { if (_ammcod != value) { _ammcod = value; NotifyPropertyChanged();} } }
	private string? _ammdes;
	public string? ammdes { get => _ammdes; set { if (_ammdes != value) { _ammdes = value; NotifyPropertyChanged();} } }
	private string? _ammind;
	public string? ammind { get => _ammind; set { if (_ammind != value) { _ammind = value; NotifyPropertyChanged();} } }
	private string? _ammloc;
	public string? ammloc { get => _ammloc; set { if (_ammloc != value) { _ammloc = value; NotifyPropertyChanged();} } }
	private int? _ammcap;
	public int? ammcap { get => _ammcap; set { if (_ammcap != value) { _ammcap = value; NotifyPropertyChanged();} } }
	private string? _ammpro;
	public string? ammpro { get => _ammpro; set { if (_ammpro != value) { _ammpro = value; NotifyPropertyChanged();} } }
	private string? _ammtel;
	public string? ammtel { get => _ammtel; set { if (_ammtel != value) { _ammtel = value; NotifyPropertyChanged();} } }
	private string? _ammfax;
	public string? ammfax { get => _ammfax; set { if (_ammfax != value) { _ammfax = value; NotifyPropertyChanged();} } }
	private string? _ammema;
	public string? ammema { get => _ammema; set { if (_ammema != value) { _ammema = value; NotifyPropertyChanged();} } }
	private string? _ammcfi;
	public string? ammcfi { get => _ammcfi; set { if (_ammcfi != value) { _ammcfi = value; NotifyPropertyChanged();} } }
	private string? _ammpiv;
	public string? ammpiv { get => _ammpiv; set { if (_ammpiv != value) { _ammpiv = value; NotifyPropertyChanged();} } }
	private string? _ammper;
	public string? ammper { get => _ammper; set { if (_ammper != value) { _ammper = value; NotifyPropertyChanged();} } }
	private int? _ammpr2;
	public int? ammpr2 { get => _ammpr2; set { if (_ammpr2 != value) { _ammpr2 = value; NotifyPropertyChanged();} } }
	private string? _ammfat;
	public string? ammfat { get => _ammfat; set { if (_ammfat != value) { _ammfat = value; NotifyPropertyChanged();} } }
}