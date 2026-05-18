namespace VulpesX.Models.Default;
 
public partial class CAUCONT_GROUPS : Base 
{
	private string _grpsoc = null!;
	public required string grpsoc { get => _grpsoc; set { if (_grpsoc != value) { _grpsoc = value; NotifyPropertyChanged();} } }
	private string _caucod = null!;
	public required string caucod { get => _caucod; set { if (_caucod != value) { _caucod = value; NotifyPropertyChanged();} } }
	private int _prog;
	public int prog { get => _prog; set { if (_prog != value) { _prog = value; NotifyPropertyChanged();} } }
	private string? _grpgrp;
	public string? grpgrp { get => _grpgrp; set { if (_grpgrp != value) { _grpgrp = value; NotifyPropertyChanged();} } }
	private string? _grpcto;
	public string? grpcto { get => _grpcto; set { if (_grpcto != value) { _grpcto = value; NotifyPropertyChanged();} } }
	private string? _grpsct;
	public string? grpsct { get => _grpsct; set { if (_grpsct != value) { _grpsct = value; NotifyPropertyChanged();} } }
	private string? _grpseg;
	public string? grpseg { get => _grpseg; set { if (_grpseg != value) { _grpseg = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}