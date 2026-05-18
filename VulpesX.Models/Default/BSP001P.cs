namespace VulpesX.Models.Default;
 
public partial class BSP001P : Base 
{
	private string _BSPSOC = null!;
	public required string BSPSOC { get => _BSPSOC; set { if (_BSPSOC != value) { _BSPSOC = value; NotifyPropertyChanged();} } }
	private int _BSPANN;
	public int BSPANN { get => _BSPANN; set { if (_BSPANN != value) { _BSPANN = value; NotifyPropertyChanged();} } }
	private int _BSPMES;
	public int BSPMES { get => _BSPMES; set { if (_BSPMES != value) { _BSPMES = value; NotifyPropertyChanged();} } }
	private string _BSPGRU = null!;
	public required string BSPGRU { get => _BSPGRU; set { if (_BSPGRU != value) { _BSPGRU = value; NotifyPropertyChanged();} } }
	private string _BSPCON = null!;
	public required string BSPCON { get => _BSPCON; set { if (_BSPCON != value) { _BSPCON = value; NotifyPropertyChanged();} } }
	private string _BSTSOT = null!;
	public required string BSTSOT { get => _BSTSOT; set { if (_BSTSOT != value) { _BSTSOT = value; NotifyPropertyChanged();} } }
	private decimal? _BSTIMP;
	public decimal? BSTIMP { get => _BSTIMP; set { if (_BSTIMP != value) { _BSTIMP = value; NotifyPropertyChanged();} } }
	private string? _BSPTIP;
	public string? BSPTIP { get => _BSPTIP; set { if (_BSPTIP != value) { _BSPTIP = value; NotifyPropertyChanged();} } }
	private int? _BSPCod;
	public int? BSPCod { get => _BSPCod; set { if (_BSPCod != value) { _BSPCod = value; NotifyPropertyChanged();} } }
}