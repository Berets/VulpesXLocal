namespace VulpesX.Models.Default;
 
public partial class CAUSMAG : Base 
{
	private string _codcau = null!;
	public required string codcau { get => _codcau; set { if (_codcau != value) { _codcau = value; NotifyPropertyChanged();} } }
	private string? _descau;
	public string? descau { get => _descau; set { if (_descau != value) { _descau = value; NotifyPropertyChanged();} } }
	private string? _segcau;
	public string? segcau { get => _segcau; set { if (_segcau != value) { _segcau = value; NotifyPropertyChanged();} } }
	private string? _flgcau;
	public string? flgcau { get => _flgcau; set { if (_flgcau != value) { _flgcau = value; NotifyPropertyChanged();} } }
	private string? _flgsca;
	public string? flgsca { get => _flgsca; set { if (_flgsca != value) { _flgsca = value; NotifyPropertyChanged();} } }
	private string? _codsca;
	public string? codsca { get => _codsca; set { if (_codsca != value) { _codsca = value; NotifyPropertyChanged();} } }
	private string? _tipcau;
	public string? tipcau { get => _tipcau; set { if (_tipcau != value) { _tipcau = value; NotifyPropertyChanged();} } }
	private string? _codtip;
	public string? codtip { get => _codtip; set { if (_codtip != value) { _codtip = value; NotifyPropertyChanged();} } }
}