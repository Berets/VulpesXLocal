namespace VulpesX.Models.Default;
 
public partial class CATBILANNO : Base 
{
	private string _catbilsoc = null!;
	public required string catbilsoc { get => _catbilsoc; set { if (_catbilsoc != value) { _catbilsoc = value; NotifyPropertyChanged();} } }
	private string _catbilcod = null!;
	public required string catbilcod { get => _catbilcod; set { if (_catbilcod != value) { _catbilcod = value; NotifyPropertyChanged();} } }
	private int _catbilann;
	public int catbilann { get => _catbilann; set { if (_catbilann != value) { _catbilann = value; NotifyPropertyChanged();} } }
	private decimal _catbilval;
	public decimal catbilval { get => _catbilval; set { if (_catbilval != value) { _catbilval = value; NotifyPropertyChanged();} } }
	private decimal _catbilper;
	public decimal catbilper { get => _catbilper; set { if (_catbilper != value) { _catbilper = value; NotifyPropertyChanged();} } }
	private decimal _calbilper2;
	public decimal calbilper2 { get => _calbilper2; set { if (_calbilper2 != value) { _calbilper2 = value; NotifyPropertyChanged();} } }
	private decimal _calbilper3;
	public decimal calbilper3 { get => _calbilper3; set { if (_calbilper3 != value) { _calbilper3 = value; NotifyPropertyChanged();} } }
	private decimal _catbilpro;
	public decimal catbilpro { get => _catbilpro; set { if (_catbilpro != value) { _catbilpro = value; NotifyPropertyChanged();} } }
}