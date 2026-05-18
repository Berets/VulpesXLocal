namespace VulpesX.Models.Default;
 
public partial class TABOR00F : Base 
{
	private string _tasoce = null!;
	public required string tasoce { get => _tasoce; set { if (_tasoce != value) { _tasoce = value; NotifyPropertyChanged();} } }
	private int _tavoce;
	public int tavoce { get => _tavoce; set { if (_tavoce != value) { _tavoce = value; NotifyPropertyChanged();} } }
	private string _tatipo = null!;
	public required string tatipo { get => _tatipo; set { if (_tatipo != value) { _tatipo = value; NotifyPropertyChanged();} } }
	private string _taqual = null!;
	public required string taqual { get => _taqual; set { if (_taqual != value) { _taqual = value; NotifyPropertyChanged();} } }
	private string _tarapp = null!;
	public required string tarapp { get => _tarapp; set { if (_tarapp != value) { _tarapp = value; NotifyPropertyChanged();} } }
	private decimal? _tadivi;
	public decimal? tadivi { get => _tadivi; set { if (_tadivi != value) { _tadivi = value; NotifyPropertyChanged();} } }
	private decimal? _tamult;
	public decimal? tamult { get => _tamult; set { if (_tamult != value) { _tamult = value; NotifyPropertyChanged();} } }
}