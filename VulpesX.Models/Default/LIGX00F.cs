namespace VulpesX.Models.Default;
 
public partial class LIGX00F : Base 
{
	private string _Ligxsoc = null!;
	public required string Ligxsoc { get => _Ligxsoc; set { if (_Ligxsoc != value) { _Ligxsoc = value; NotifyPropertyChanged();} } }
	private string _ligxart = null!;
	public required string ligxart { get => _ligxart; set { if (_ligxart != value) { _ligxart = value; NotifyPropertyChanged();} } }
	private decimal _ligxpre;
	public decimal ligxpre { get => _ligxpre; set { if (_ligxpre != value) { _ligxpre = value; NotifyPropertyChanged();} } }
	private decimal _ligxacq;
	public decimal ligxacq { get => _ligxacq; set { if (_ligxacq != value) { _ligxacq = value; NotifyPropertyChanged();} } }
	private int _ligxmri;
	public int ligxmri { get => _ligxmri; set { if (_ligxmri != value) { _ligxmri = value; NotifyPropertyChanged();} } }
	private string _ligxtip = null!;
	public required string ligxtip { get => _ligxtip; set { if (_ligxtip != value) { _ligxtip = value; NotifyPropertyChanged();} } }
	private string _ligxval = null!;
	public required string ligxval { get => _ligxval; set { if (_ligxval != value) { _ligxval = value; NotifyPropertyChanged();} } }
	private string _ligxdiv = null!;
	public required string ligxdiv { get => _ligxdiv; set { if (_ligxdiv != value) { _ligxdiv = value; NotifyPropertyChanged();} } }
	private string _ligxvaa = null!;
	public required string ligxvaa { get => _ligxvaa; set { if (_ligxvaa != value) { _ligxvaa = value; NotifyPropertyChanged();} } }
	private string _ligxdia = null!;
	public required string ligxdia { get => _ligxdia; set { if (_ligxdia != value) { _ligxdia = value; NotifyPropertyChanged();} } }
	private decimal _ligxbon;
	public decimal ligxbon { get => _ligxbon; set { if (_ligxbon != value) { _ligxbon = value; NotifyPropertyChanged();} } }
}