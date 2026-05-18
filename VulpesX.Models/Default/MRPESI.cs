namespace VulpesX.Models.Default;
 
public partial class MRPESI : Base 
{
	private string _mreart = null!;
	public required string mreart { get => _mreart; set { if (_mreart != value) { _mreart = value; NotifyPropertyChanged();} } }
	private decimal? _mreqta;
	public decimal? mreqta { get => _mreqta; set { if (_mreqta != value) { _mreqta = value; NotifyPropertyChanged();} } }
	private decimal? _mreqtu;
	public decimal? mreqtu { get => _mreqtu; set { if (_mreqtu != value) { _mreqtu = value; NotifyPropertyChanged();} } }
}