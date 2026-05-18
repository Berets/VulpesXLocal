namespace VulpesX.Models.Default;
 
public partial class TXTDETTA : Base 
{
	private string _TTxsoc = null!;
	public required string TTxsoc { get => _TTxsoc; set { if (_TTxsoc != value) { _TTxsoc = value; NotifyPropertyChanged();} } }
	private string _TTxcod = null!;
	public required string TTxcod { get => _TTxcod; set { if (_TTxcod != value) { _TTxcod = value; NotifyPropertyChanged();} } }
	private string _TTXtip = null!;
	public required string TTXtip { get => _TTXtip; set { if (_TTXtip != value) { _TTXtip = value; NotifyPropertyChanged();} } }
	private int _DTXrig;
	public int DTXrig { get => _DTXrig; set { if (_DTXrig != value) { _DTXrig = value; NotifyPropertyChanged();} } }
	private string? _DTxdes;
	public string? DTxdes { get => _DTxdes; set { if (_DTxdes != value) { _DTxdes = value; NotifyPropertyChanged();} } }
}