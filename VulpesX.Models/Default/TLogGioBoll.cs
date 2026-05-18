namespace VulpesX.Models.Default;
 
public partial class TLogGioBoll : Base 
{
	private int _nregi;
	public int nregi { get => _nregi; set { if (_nregi != value) { _nregi = value; NotifyPropertyChanged();} } }
	private string _npagi = null!;
	public required string npagi { get => _npagi; set { if (_npagi != value) { _npagi = value; NotifyPropertyChanged();} } }
}