namespace VulpesX.Models.Default;
 
public partial class TAB_ACC_TIPINC : Base 
{
	private string _icscod = null!;
	public required string icscod { get => _icscod; set { if (_icscod != value) { _icscod = value; NotifyPropertyChanged();} } }
	private string _icsdes = null!;
	public required string icsdes { get => _icsdes; set { if (_icsdes != value) { _icsdes = value; NotifyPropertyChanged();} } }
	private string? _icssup;
	public string? icssup { get => _icssup; set { if (_icssup != value) { _icssup = value; NotifyPropertyChanged();} } }
	private string? _icsics;
	public string? icsics { get => _icsics; set { if (_icsics != value) { _icsics = value; NotifyPropertyChanged();} } }
	private string? _icscau;
	public string? icscau { get => _icscau; set { if (_icscau != value) { _icscau = value; NotifyPropertyChanged();} } }
	private string? _icsfepacod;
	public string? icsfepacod { get => _icsfepacod; set { if (_icsfepacod != value) { _icsfepacod = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}