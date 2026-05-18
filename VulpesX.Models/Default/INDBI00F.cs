namespace VulpesX.Models.Default;
 
public partial class INDBI00F : Base 
{
	private string _ANASOC = null!;
	public required string ANASOC { get => _ANASOC; set { if (_ANASOC != value) { _ANASOC = value; NotifyPropertyChanged();} } }
	private int _ANACOD;
	public int ANACOD { get => _ANACOD; set { if (_ANACOD != value) { _ANACOD = value; NotifyPropertyChanged();} } }
	private string? _ANADES;
	public string? ANADES { get => _ANADES; set { if (_ANADES != value) { _ANADES = value; NotifyPropertyChanged();} } }
	private string? _ANAFOR;
	public string? ANAFOR { get => _ANAFOR; set { if (_ANAFOR != value) { _ANAFOR = value; NotifyPropertyChanged();} } }
}