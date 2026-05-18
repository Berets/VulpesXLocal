namespace VulpesX.Models.Default;
 
public partial class Tabiapp : Base 
{
	private string _abappsoc = null!;
	public required string abappsoc { get => _abappsoc; set { if (_abappsoc != value) { _abappsoc = value; NotifyPropertyChanged();} } }
	private string _abappnom = null!;
	public required string abappnom { get => _abappnom; set { if (_abappnom != value) { _abappnom = value; NotifyPropertyChanged();} } }
	private DateTime? _abappdata;
	public DateTime? abappdata { get => _abappdata; set { if (_abappdata != value) { _abappdata = value; NotifyPropertyChanged();} } }
	private string? _apappflg;
	public string? apappflg { get => _apappflg; set { if (_apappflg != value) { _apappflg = value; NotifyPropertyChanged();} } }
}