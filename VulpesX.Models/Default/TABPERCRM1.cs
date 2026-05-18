namespace VulpesX.Models.Default;
 
public partial class TABPERCRM1 : Base 
{
	private string _Crmsoc = null!;
	public required string Crmsoc { get => _Crmsoc; set { if (_Crmsoc != value) { _Crmsoc = value; NotifyPropertyChanged();} } }
	private string _crmcod = null!;
	public required string crmcod { get => _crmcod; set { if (_crmcod != value) { _crmcod = value; NotifyPropertyChanged();} } }
	private string _crmrig = null!;
	public required string crmrig { get => _crmrig; set { if (_crmrig != value) { _crmrig = value; NotifyPropertyChanged();} } }
	private string? _crmdes;
	public string? crmdes { get => _crmdes; set { if (_crmdes != value) { _crmdes = value; NotifyPropertyChanged();} } }
	private string? _crmris;
	public string? crmris { get => _crmris; set { if (_crmris != value) { _crmris = value; NotifyPropertyChanged();} } }
}