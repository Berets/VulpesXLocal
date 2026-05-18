namespace VulpesX.Models.Default;
 
public partial class CONESCBIL : Base 
{
	private string _cebsoc = null!;
	public required string cebsoc { get => _cebsoc; set { if (_cebsoc != value) { _cebsoc = value; NotifyPropertyChanged();} } }
	private string _cebgru = null!;
	public required string cebgru { get => _cebgru; set { if (_cebgru != value) { _cebgru = value; NotifyPropertyChanged();} } }
	private string _cebcon = null!;
	public required string cebcon { get => _cebcon; set { if (_cebcon != value) { _cebcon = value; NotifyPropertyChanged();} } }
	private string _cebsot = null!;
	public required string cebsot { get => _cebsot; set { if (_cebsot != value) { _cebsot = value; NotifyPropertyChanged();} } }
}