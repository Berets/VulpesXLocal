namespace VulpesX.Models.Default;
 
public partial class FILIALI : Base 
{
	private int _filcod;
	public int filcod { get => _filcod; set { if (_filcod != value) { _filcod = value; NotifyPropertyChanged();} } }
	private string _fildes = null!;
	public required string fildes { get => _fildes; set { if (_fildes != value) { _fildes = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private string _filsoc = null!;
	public required string filsoc { get => _filsoc; set { if (_filsoc != value) { _filsoc = value; NotifyPropertyChanged();} } }
}