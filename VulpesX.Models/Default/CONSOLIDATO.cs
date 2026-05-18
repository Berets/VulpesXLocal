namespace VulpesX.Models.Default;
 
public partial class CONSOLIDATO : Base 
{
	private int _conann;
	public int conann { get => _conann; set { if (_conann != value) { _conann = value; NotifyPropertyChanged();} } }
	private string _consgr = null!;
	public required string consgr { get => _consgr; set { if (_consgr != value) { _consgr = value; NotifyPropertyChanged();} } }
	private string _consco = null!;
	public required string consco { get => _consco; set { if (_consco != value) { _consco = value; NotifyPropertyChanged();} } }
	private DateTime? _condat;
	public DateTime? condat { get => _condat; set { if (_condat != value) { _condat = value; NotifyPropertyChanged();} } }
}