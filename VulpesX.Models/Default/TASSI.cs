namespace VulpesX.Models.Default;
 
public partial class TASSI : Base 
{
	private string _tassoc = null!;
	public required string tassoc { get => _tassoc; set { if (_tassoc != value) { _tassoc = value; NotifyPropertyChanged();} } }
	private DateTime _tasdat;
	public DateTime tasdat { get => _tasdat; set { if (_tasdat != value) { _tasdat = value; NotifyPropertyChanged();} } }
	private decimal? _tasper;
	public decimal? tasper { get => _tasper; set { if (_tasper != value) { _tasper = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}