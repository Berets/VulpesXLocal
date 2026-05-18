namespace VulpesX.Models.Default;
 
public partial class ABICAB : Base 
{
	private int _abiabi;
	public int abiabi { get => _abiabi; set { if (_abiabi != value) { _abiabi = value; NotifyPropertyChanged();} } }
	private int _abicab;
	public int abicab { get => _abicab; set { if (_abicab != value) { _abicab = value; NotifyPropertyChanged();} } }
	private string _abiban = null!;
	public required string abiban { get => _abiban; set { if (_abiban != value) { _abiban = value; NotifyPropertyChanged();} } }
	private string? _abiage;
	public string? abiage { get => _abiage; set { if (_abiage != value) { _abiage = value; NotifyPropertyChanged();} } }
	private string? _abicit;
	public string? abicit { get => _abicit; set { if (_abicit != value) { _abicit = value; NotifyPropertyChanged();} } }
	private string? _abiind;
	public string? abiind { get => _abiind; set { if (_abiind != value) { _abiind = value; NotifyPropertyChanged();} } }
	private string? _abipro;
	public string? abipro { get => _abipro; set { if (_abipro != value) { _abipro = value; NotifyPropertyChanged();} } }
	private int? _abicap;
	public int? abicap { get => _abicap; set { if (_abicap != value) { _abicap = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}