namespace VulpesX.Models.Default;
 
public partial class TCOMLIQIVA : Base 
{
	private string _CLICodSOC = null!;
	public required string CLICodSOC { get => _CLICodSOC; set { if (_CLICodSOC != value) { _CLICodSOC = value; NotifyPropertyChanged();} } }
	private int _CLIAnnLiq;
	public int CLIAnnLiq { get => _CLIAnnLiq; set { if (_CLIAnnLiq != value) { _CLIAnnLiq = value; NotifyPropertyChanged();} } }
	private int _CLIPerLiq;
	public int CLIPerLiq { get => _CLIPerLiq; set { if (_CLIPerLiq != value) { _CLIPerLiq = value; NotifyPropertyChanged();} } }
	private string _CLIVocLiq = null!;
	public required string CLIVocLiq { get => _CLIVocLiq; set { if (_CLIVocLiq != value) { _CLIVocLiq = value; NotifyPropertyChanged();} } }
	private string _CLITipLiq = null!;
	public required string CLITipLiq { get => _CLITipLiq; set { if (_CLITipLiq != value) { _CLITipLiq = value; NotifyPropertyChanged();} } }
	private decimal _CLIImpLiqD;
	public decimal CLIImpLiqD { get => _CLIImpLiqD; set { if (_CLIImpLiqD != value) { _CLIImpLiqD = value; NotifyPropertyChanged();} } }
	private decimal _CLIImpLiqC;
	public decimal CLIImpLiqC { get => _CLIImpLiqC; set { if (_CLIImpLiqC != value) { _CLIImpLiqC = value; NotifyPropertyChanged();} } }
	private string _addedUserID = null!;
	public required string addedUserID { get => _addedUserID; set { if (_addedUserID != value) { _addedUserID = value; NotifyPropertyChanged();} } }
	private DateTime? _added;
	public DateTime? added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private DateTime? _updated;
	public DateTime? updated { get => _updated; set { if (_updated != value) { _updated = value; NotifyPropertyChanged();} } }
	private DateTime? _canceled;
	public DateTime? canceled { get => _canceled; set { if (_canceled != value) { _canceled = value; NotifyPropertyChanged();} } }
	private string? _updatedUserID;
	public string? updatedUserID { get => _updatedUserID; set { if (_updatedUserID != value) { _updatedUserID = value; NotifyPropertyChanged();} } }
	private string? _canceledUserID;
	public string? canceledUserID { get => _canceledUserID; set { if (_canceledUserID != value) { _canceledUserID = value; NotifyPropertyChanged();} } }
	private string? _canceledNote;
	public string? canceledNote { get => _canceledNote; set { if (_canceledNote != value) { _canceledNote = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}