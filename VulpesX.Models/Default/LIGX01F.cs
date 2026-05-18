namespace VulpesX.Models.Default;
 
public partial class LIGX01F : Base 
{
	private string _Ligxsoc = null!;
	public required string Ligxsoc { get => _Ligxsoc; set { if (_Ligxsoc != value) { _Ligxsoc = value; NotifyPropertyChanged();} } }
	private string _ligxart = null!;
	public required string ligxart { get => _ligxart; set { if (_ligxart != value) { _ligxart = value; NotifyPropertyChanged();} } }
	private int _ligxrig;
	public int ligxrig { get => _ligxrig; set { if (_ligxrig != value) { _ligxrig = value; NotifyPropertyChanged();} } }
	private int _ligxqta1;
	public int ligxqta1 { get => _ligxqta1; set { if (_ligxqta1 != value) { _ligxqta1 = value; NotifyPropertyChanged();} } }
	private int _ligxqta2;
	public int ligxqta2 { get => _ligxqta2; set { if (_ligxqta2 != value) { _ligxqta2 = value; NotifyPropertyChanged();} } }
	private decimal _ligxpres;
	public decimal ligxpres { get => _ligxpres; set { if (_ligxpres != value) { _ligxpres = value; NotifyPropertyChanged();} } }
	private decimal _ligxacqs;
	public decimal ligxacqs { get => _ligxacqs; set { if (_ligxacqs != value) { _ligxacqs = value; NotifyPropertyChanged();} } }
}