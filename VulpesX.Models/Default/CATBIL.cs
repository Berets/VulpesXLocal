namespace VulpesX.Models.Default;
 
public partial class CATBIL : Base 
{
	private string _catbilsoc = null!;
	public required string catbilsoc { get => _catbilsoc; set { if (_catbilsoc != value) { _catbilsoc = value; NotifyPropertyChanged();} } }
	private string _catbilcod = null!;
	public required string catbilcod { get => _catbilcod; set { if (_catbilcod != value) { _catbilcod = value; NotifyPropertyChanged();} } }
	private string _catbildes = null!;
	public required string catbildes { get => _catbildes; set { if (_catbildes != value) { _catbildes = value; NotifyPropertyChanged();} } }
}