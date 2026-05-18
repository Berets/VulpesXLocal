namespace VulpesX.Models.Default;
 
public partial class GESIVACASSA : Base 
{
	private string _ivasoc = null!;
	public required string ivasoc { get => _ivasoc; set { if (_ivasoc != value) { _ivasoc = value; NotifyPropertyChanged();} } }
	private int _ivacod;
	public int ivacod { get => _ivacod; set { if (_ivacod != value) { _ivacod = value; NotifyPropertyChanged();} } }
	private string? _ivanota;
	public string? ivanota { get => _ivanota; set { if (_ivanota != value) { _ivanota = value; NotifyPropertyChanged();} } }
}