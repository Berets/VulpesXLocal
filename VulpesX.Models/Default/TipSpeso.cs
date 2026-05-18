namespace VulpesX.Models.Default;
 
public partial class TipSpeso : Base 
{
	private string _tipssoc = null!;
	public required string tipssoc { get => _tipssoc; set { if (_tipssoc != value) { _tipssoc = value; NotifyPropertyChanged();} } }
	private string _tipscod = null!;
	public required string tipscod { get => _tipscod; set { if (_tipscod != value) { _tipscod = value; NotifyPropertyChanged();} } }
	private string? _tipsper;
	public string? tipsper { get => _tipsper; set { if (_tipsper != value) { _tipsper = value; NotifyPropertyChanged();} } }
	private string? _tipsnom;
	public string? tipsnom { get => _tipsnom; set { if (_tipsnom != value) { _tipsnom = value; NotifyPropertyChanged();} } }
	private int? _tipsann;
	public int? tipsann { get => _tipsann; set { if (_tipsann != value) { _tipsann = value; NotifyPropertyChanged();} } }
	private string? _tipsuso;
	public string? tipsuso { get => _tipsuso; set { if (_tipsuso != value) { _tipsuso = value; NotifyPropertyChanged();} } }
	private decimal? _tipslimval;
	public decimal? tipslimval { get => _tipslimval; set { if (_tipslimval != value) { _tipslimval = value; NotifyPropertyChanged();} } }
	private int? _tipspue;
	public int? tipspue { get => _tipspue; set { if (_tipspue != value) { _tipspue = value; NotifyPropertyChanged();} } }
	private string? _tipscf;
	public string? tipscf { get => _tipscf; set { if (_tipscf != value) { _tipscf = value; NotifyPropertyChanged();} } }
	private DateTime? _tipsdti;
	public DateTime? tipsdti { get => _tipsdti; set { if (_tipsdti != value) { _tipsdti = value; NotifyPropertyChanged();} } }
	private string? _tipsnia;
	public string? tipsnia { get => _tipsnia; set { if (_tipsnia != value) { _tipsnia = value; NotifyPropertyChanged();} } }
	private string? _tipstra;
	public string? tipstra { get => _tipstra; set { if (_tipstra != value) { _tipstra = value; NotifyPropertyChanged();} } }
	private string? _tipteco;
	public string? tipteco { get => _tipteco; set { if (_tipteco != value) { _tipteco = value; NotifyPropertyChanged();} } }
	private string? _tipsema;
	public string? tipsema { get => _tipsema; set { if (_tipsema != value) { _tipsema = value; NotifyPropertyChanged();} } }
	private string? _tipstel;
	public string? tipstel { get => _tipstel; set { if (_tipstel != value) { _tipstel = value; NotifyPropertyChanged();} } }
	private string? _tipsfax;
	public string? tipsfax { get => _tipsfax; set { if (_tipsfax != value) { _tipsfax = value; NotifyPropertyChanged();} } }
}