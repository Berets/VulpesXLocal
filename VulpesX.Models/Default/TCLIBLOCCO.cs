namespace VulpesX.Models.Default;
 
public partial class TCLIBLOCCO : Base 
{
	private string _clbazi = null!;
	public required string clbazi { get => _clbazi; set { if (_clbazi != value) { _clbazi = value; NotifyPropertyChanged();} } }
	private int _clbcli;
	public int clbcli { get => _clbcli; set { if (_clbcli != value) { _clbcli = value; NotifyPropertyChanged();} } }
	private DateTime _clbini;
	public DateTime clbini { get => _clbini; set { if (_clbini != value) { _clbini = value; NotifyPropertyChanged();} } }
	private DateTime? _clbfin;
	public DateTime? clbfin { get => _clbfin; set { if (_clbfin != value) { _clbfin = value; NotifyPropertyChanged();} } }
	private string? _clbcab;
	public string? clbcab { get => _clbcab; set { if (_clbcab != value) { _clbcab = value; NotifyPropertyChanged();} } }
	private string? _clbnot;
	public string? clbnot { get => _clbnot; set { if (_clbnot != value) { _clbnot = value; NotifyPropertyChanged();} } }
	private string? _clbusr;
	public string? clbusr { get => _clbusr; set { if (_clbusr != value) { _clbusr = value; NotifyPropertyChanged();} } }
	private string? _clbcas;
	public string? clbcas { get => _clbcas; set { if (_clbcas != value) { _clbcas = value; NotifyPropertyChanged();} } }
	private string? _clbuss;
	public string? clbuss { get => _clbuss; set { if (_clbuss != value) { _clbuss = value; NotifyPropertyChanged();} } }
}