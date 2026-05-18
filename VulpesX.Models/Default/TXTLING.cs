namespace VulpesX.Models.Default;
 
public partial class TXTLING : Base 
{
	private string _TTxsoc = null!;
	public required string TTxsoc { get => _TTxsoc; set { if (_TTxsoc != value) { _TTxsoc = value; NotifyPropertyChanged();} } }
	private string _TTxcod = null!;
	public required string TTxcod { get => _TTxcod; set { if (_TTxcod != value) { _TTxcod = value; NotifyPropertyChanged();} } }
	private string _TTXtip = null!;
	public required string TTXtip { get => _TTXtip; set { if (_TTXtip != value) { _TTXtip = value; NotifyPropertyChanged();} } }
	private string _ttxlin = null!;
	public required string ttxlin { get => _ttxlin; set { if (_ttxlin != value) { _ttxlin = value; NotifyPropertyChanged();} } }
	private string? _ttxldes;
	public string? ttxldes { get => _ttxldes; set { if (_ttxldes != value) { _ttxldes = value; NotifyPropertyChanged();} } }
}