namespace VulpesX.Models.Default;
 
public partial class PROGRESSIVI : Base 
{
	private string _prosoc = null!;
	public required string prosoc { get => _prosoc; set { if (_prosoc != value) { _prosoc = value; NotifyPropertyChanged();} } }
	private int _proann;
	public int proann { get => _proann; set { if (_proann != value) { _proann = value; NotifyPropertyChanged();} } }
	private decimal _pronum;
	public decimal pronum { get => _pronum; set { if (_pronum != value) { _pronum = value; NotifyPropertyChanged();} } }
	private decimal? _procar;
	public decimal? procar { get => _procar; set { if (_procar != value) { _procar = value; NotifyPropertyChanged();} } }
	private decimal? _prosca;
	public decimal? prosca { get => _prosca; set { if (_prosca != value) { _prosca = value; NotifyPropertyChanged();} } }
	private DateTime? _prodat;
	public DateTime? prodat { get => _prodat; set { if (_prodat != value) { _prodat = value; NotifyPropertyChanged();} } }
	private decimal? _prorim;
	public decimal? prorim { get => _prorim; set { if (_prorim != value) { _prorim = value; NotifyPropertyChanged();} } }
	private int? _pronur;
	public int? pronur { get => _pronur; set { if (_pronur != value) { _pronur = value; NotifyPropertyChanged();} } }
}