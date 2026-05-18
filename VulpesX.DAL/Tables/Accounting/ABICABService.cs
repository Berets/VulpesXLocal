using Microsoft.Extensions.DependencyInjection;
using System.Text;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default.Partials;

namespace VulpesX.DAL.Tables.Accounting;

public interface IABICABRepository
{
    ObservableCollection<ABICAB>? GetList(int? FilterABI, int? FilterCAB, string? FilterText, int PageSize, int RequestedPage, out int TotalCount);

    ObservableCollection<BankItem>? GetSimpleList(string CompanyID, string? PaymentType);

    ObservableCollection<BankItem>? GetSimpleListSuppliers(string CompanyID, string PaymentType);

    void RefreshOfflineBanksListAsync();

    ABICAB? Get(int ABI, int CAB);

    bool Exists(int ABI, int CAB);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ABICAB Model);

    bool Update(ABICAB Model);

    bool Delete(ABICAB Model);

    string? Validate(ABICAB Model, bool IsInsert);
    #endregion
}

public class ABICABRepository : RepositoryBase, IABICABRepository
{
    public ABICABRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<ABICAB>? GetList(int? FilterABI, int? FilterCAB, string? FilterText, int PageSize, int RequestedPage, out int TotalCount)
    {
        TotalCount = 0;
        try
        {
            using var connection = GetOpenConnection();


            var whereFilter = new StringBuilder("");
            bool whereAdded = false;

            if (FilterABI.HasValue && FilterABI.Value > 0)
            {
                whereFilter.Append($"WHERE abiabi={FilterABI} ");
                whereAdded = true;
            }
            if (FilterCAB.HasValue && FilterCAB.Value > 0)
            {
                whereFilter.Append($"{(whereAdded ? " AND " : "WHERE ")} abicab={FilterCAB} ");
                if (!whereAdded)
                    whereAdded = true;
            }
            if (!string.IsNullOrWhiteSpace(FilterText))
            {
                whereFilter.Append($"{(whereAdded ? " AND " : "WHERE ")} (abiban like '%{FilterText}%' OR abiage like '%{FilterText}%') ");
            }

            TotalCount = (int?)connection.ExecuteScalar($@"SELECT COUNT(*) FROM ABICAB {whereFilter.ToString()};") ?? 0;
            var list = connection.Query<ABICAB>(
                $@"SELECT abiabi,abicab,abiban,abiage,abicit,abiind,abipro,abicap FROM ABICAB 
                        {whereFilter.ToString()} 
                        ORDER BY abiabi,abicab
                        OFFSET @skip ROWS 
                        FETCH NEXT @ps ROWS ONLY;",
                new { skip = RequestedPage * PageSize, ps = PageSize });

            return new ObservableCollection<ABICAB>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<BankItem>? GetSimpleList(string CompanyID, string? PaymentType)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(PaymentType) && PaymentType != "R")
            {
                var result = new ObservableCollection<BankItem>();
                foreach (var bank in VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().GetListActive(CompanyID) ?? new ObservableCollection<BANAZIEN>())
                {
                    result.Add(new BankItem()
                    {
                        ABI = bank.abiabi,
                        CAB = bank.abicab,
                        Description = $"{bank.BankName} {bank.BankAgency}",
                        CIN = bank.abicin,
                        BIC = bank.abibic,
                        BBAN = bank.abibba,
                        IBAN = bank.abibiba,
                        Account = bank.abicon
                    });
                }
                return result;
            }
            else
            {
                return BankContext.Instance.OfflineBanks;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<BankItem>? GetSimpleListSuppliers(string CompanyID, string PaymentType)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(PaymentType) && PaymentType != "R")
            {
                return BankContext.Instance.OfflineBanks;
            }
            else
            {
                var result = new ObservableCollection<BankItem>();
                foreach (var bank in VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().GetListActive(CompanyID) ?? new ObservableCollection<BANAZIEN>())
                {
                    result.Add(new BankItem()
                    {
                        ABI = bank.abiabi,
                        CAB = bank.abicab,
                        Description = $"{bank.BankName} {bank.BankAgency}",
                        CIN = bank.abicin,
                        BIC = bank.abibic,
                        BBAN = bank.abibba,
                        IBAN = bank.abibiba,
                        Account = bank.abicon
                    });
                }
                return result;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public async void RefreshOfflineBanksListAsync()
    {
        try
        {
            using var connection = GetOpenConnection();


            BankContext.Instance.OfflineBanks = new ObservableCollection<BankItem>();
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            // TODO optimize 11/13 seconds nvarchar
            // TODO optimize 7/8 seconds varchar 90.000
            // TODO optimize 15 seconds varchar 125.000
            // TODO optimize 23/25 seconds varchar 145.554
            BankContext.Instance.OfflineBanks = new ObservableCollection<BankItem>(await connection.QueryAsync<BankItem>(
                "SELECT abiabi AS ABI, abicab AS CAB, abiban + ' ' + abiage AS Description FROM ABICAB"));
            //sw.Stop();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            BankContext.Instance.OfflineBanks = null;
        }
    }

    public ABICAB? Get(int ABI, int CAB)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ABICAB>(
                "SELECT * FROM ABICAB WHERE abiabi = @abi AND abicab = @cab",
                new { abi = ABI, cab = CAB })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int ABI, int CAB)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ABICAB WHERE abiabi = @abiabi AND abicab = @abicab",
                new { abiabi = ABI, abicab = CAB }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ABICAB (abiabi,abicab,abiban,abiage,abicit,abiind,abipro,abicap) OUTPUT INSERTED.rv VALUES(@abiabi,@abicab,@abiban,@abiage,@abicit,@abiind,@abipro,@abicap)";
    public string UPDATE_QUERY => "UPDATE ABICAB SET abiabi = @abiabi,abicab = @abicab,abiban = @abiban,abiage = @abiage,abicit = @abicit,abiind = @abiind,abipro = @abipro,abicap = @abicap OUTPUT INSERTED.rv WHERE abiabi = @abiabi AND abicab = @abicab AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ABICAB OUTPUT DELETED.rv WHERE abiabi = @abiabi AND abicab = @abicab AND rv = @rv";
    public bool Insert(ABICAB Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            if (string.IsNullOrWhiteSpace(Model.abiage?.Trim()))
                Model.abiage = null;
            if (string.IsNullOrWhiteSpace(Model.abiind?.Trim()))
                Model.abiind = null;
            var result = connection.Execute(INSERT_QUERY, Model);
            if (result > 0)
            {
                return true;
            }
            else
            {
                ErrorHandler.Show(Constants.INSERT_VIOLATION);
                return false;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Update(ABICAB Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            if (string.IsNullOrWhiteSpace(Model.abiage?.Trim()))
                Model.abiage = null;
            if (string.IsNullOrWhiteSpace(Model.abiind?.Trim()))
                Model.abiind = null;
            var result = connection.ExecuteScalar(UPDATE_QUERY, Model);
            if (result != null)
            {
                return true;
            }
            else
            {
                ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                return false;
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Delete(ABICAB Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(DELETE_QUERY, Model);
            if (result > 0)
            {
                return true;
            }
            else
            {
                ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                return false;
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public string? Validate(ABICAB Model, bool IsInsert)
    {
        try
        {
            if (Model.abiabi > 0 && Model.abicab > 0 && IsInsert && !Exists(Model.abiabi, Model.abicab) || !IsInsert)
            {
                if (!String.IsNullOrWhiteSpace(Model.abiban) && Model.abiban.Length <= 255)
                {
                    if (!String.IsNullOrWhiteSpace(Model.abiage) && Model.abiage.Length <= 255)
                    {
                        if (Model.abiind == null || Model.abiind?.Length <= 255)
                        {
                            return null;
                        }
                        else
                        { return "L'indirizzo può contenere al massimo 255 caratteri"; }
                    }
                    else
                    { return "L'agenzia è obbligatoria e può contenere al massimo 255 caratteri"; }
                }
                else
                { return "L'istituto è obbligatorio e può contenere al massimo 255 caratteri"; }
            }
            else
            { return "I codici inseriti sono già in uso o non sono validi"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}

public class ABICABUfpRepository : RepositoryBase, IABICABRepository
{
    public ABICABUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<ABICAB>? GetList(int? FilterABI, int? FilterCAB, string? FilterText, int PageSize, int RequestedPage, out int TotalCount)
    {
        TotalCount = 0;
        try
        {
            using var connection = GetOpenConnection();


            var whereFilter = new StringBuilder("");
            bool whereAdded = false;

            if (FilterABI.HasValue && FilterABI.Value > 0)
            {
                whereFilter.Append($"WHERE abiabi={FilterABI} ");
                whereAdded = true;
            }
            if (FilterCAB.HasValue && FilterCAB.Value > 0)
            {
                whereFilter.Append($"{(whereAdded ? " AND " : "WHERE ")} abicab={FilterCAB} ");
                if (!whereAdded)
                    whereAdded = true;
            }
            if (!string.IsNullOrWhiteSpace(FilterText))
            {
                whereFilter.Append($"{(whereAdded ? " AND " : "WHERE ")} (abiban like '%{FilterText}%' OR abiage like '%{FilterText}%') ");
            }

            TotalCount = (int?)connection.ExecuteScalar($@"SELECT COUNT(*) FROM TAB_ABICAB {whereFilter.ToString()};") ?? 0;
            var list = connection.Query<ABICAB>(
                $@"SELECT abiabi,abicab,abiban,abiage,abicit,abiind,abipro,abicap,rv FROM TAB_ABICAB 
                        {whereFilter.ToString()} 
                        ORDER BY abiabi,abicab
                        OFFSET @skip ROWS 
                        FETCH NEXT @ps ROWS ONLY;",
                new { skip = RequestedPage * PageSize, ps = PageSize });

            return new ObservableCollection<ABICAB>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<BankItem>? GetSimpleList(string CompanyID, string? PaymentType)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(PaymentType) && PaymentType != "R")
            {
                var result = new ObservableCollection<BankItem>();
                foreach (var bank in VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().GetListActive(CompanyID) ?? new ObservableCollection<BANAZIEN>())
                {
                    result.Add(new BankItem()
                    {
                        ABI = bank.abiabi,
                        CAB = bank.abicab,
                        Description = $"{bank.BankName} {bank.BankAgency}",
                        CIN = bank.abicin,
                        BIC = bank.abibic,
                        BBAN = bank.abibba,
                        IBAN = bank.abibiba,
                        Account = bank.abicon.TrimEnd()
                    });
                }
                return result;
            }
            else
            {
                return BankContext.Instance.OfflineBanks;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<BankItem>? GetSimpleListSuppliers(string CompanyID, string PaymentType)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(PaymentType) && PaymentType != "R")
            {
                return BankContext.Instance.OfflineBanks;
            }
            else
            {
                var result = new ObservableCollection<BankItem>();
                foreach (var bank in VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().GetListActive(CompanyID) ?? new ObservableCollection<BANAZIEN>())
                {
                    result.Add(new BankItem()
                    {
                        ABI = bank.abiabi,
                        CAB = bank.abicab,
                        Description = $"{bank.BankName} {bank.BankAgency}",
                        CIN = bank.abicin,
                        BIC = bank.abibic,
                        BBAN = bank.abibba,
                        IBAN = bank.abibiba,
                        Account = bank.abicon
                    });
                }
                return result;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public async void RefreshOfflineBanksListAsync()
    {
        try
        {
            using var connection = GetOpenConnection();


            BankContext.Instance.OfflineBanks = new ObservableCollection<BankItem>();
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            // TODO optimize 11/13 seconds nvarchar
            // TODO optimize 7/8 seconds varchar 90.000
            // TODO optimize 15 seconds varchar 125.000
            // TODO optimize 23/25 seconds varchar 145.554
            BankContext.Instance.OfflineBanks = new ObservableCollection<BankItem>(await connection.QueryAsync<BankItem>(
                "SELECT abiabi AS ABI, abicab AS CAB, abiban + ' ' + abiage AS Description FROM TAB_ABICAB"));
            //sw.Stop();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            BankContext.Instance.OfflineBanks = null;
        }
    }

    public ABICAB? Get(int ABI, int CAB)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ABICAB>(
                "SELECT * FROM TAB_ABICAB WHERE abiabi = @abi AND abicab = @cab",
                new { abi = ABI, cab = CAB })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int ABI, int CAB)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM TAB_ABICAB WHERE abiabi = @abiabi AND abicab = @abicab",
                new { abiabi = ABI, abicab = CAB }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO TAB_ABICAB (abiabi,abicab,abiban,abiage,abicit,abiind,abipro,abicap) OUTPUT INSERTED.rv VALUES(@abiabi,@abicab,@abiban,@abiage,@abicit,@abiind,@abipro,@abicap)";
    public string UPDATE_QUERY => "UPDATE TAB_ABICAB SET abiabi = @abiabi,abicab = @abicab,abiban = @abiban,abiage = @abiage,abicit = @abicit,abiind = @abiind,abipro = @abipro,abicap = @abicap OUTPUT INSERTED.rv WHERE abiabi = @abiabi AND abicab = @abicab AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM TAB_ABICAB OUTPUT DELETED.rv WHERE abiabi = @abiabi AND abicab = @abicab AND rv = @rv";
    public bool Insert(ABICAB Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            if (string.IsNullOrWhiteSpace(Model.abiage?.Trim()))
                Model.abiage = null;
            if (string.IsNullOrWhiteSpace(Model.abiind?.Trim()))
                Model.abiind = null;
            var result = connection.Execute(INSERT_QUERY, Model);
            if (result > 0)
            {
                return true;
            }
            else
            {
                ErrorHandler.Show(Constants.INSERT_VIOLATION);
                return false;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Update(ABICAB Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            if (string.IsNullOrWhiteSpace(Model.abiage?.Trim()))
                Model.abiage = null;
            if (string.IsNullOrWhiteSpace(Model.abiind?.Trim()))
                Model.abiind = null;
            var result = connection.ExecuteScalar(UPDATE_QUERY, Model);
            if (result != null)
            {
                return true;
            }
            else
            {
                ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                return false;
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Delete(ABICAB Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(DELETE_QUERY, Model);
            if (result > 0)
            {
                return true;
            }
            else
            {
                ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                return false;
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public string? Validate(ABICAB Model, bool IsInsert)
    {
        try
        {
            if (Model.abiabi > 0 && Model.abicab > 0 && IsInsert && !Exists(Model.abiabi, Model.abicab) || !IsInsert)
            {
                if (!String.IsNullOrWhiteSpace(Model.abiban) && Model.abiban.Length <= 255)
                {
                    if (!String.IsNullOrWhiteSpace(Model.abiage) && Model.abiage.Length <= 255)
                    {
                        if (Model.abiind == null || Model.abiind?.Length <= 255)
                        {
                            return null;
                        }
                        else
                        { return "L'indirizzo può contenere al massimo 255 caratteri"; }
                    }
                    else
                    { return "L'agenzia è obbligatoria e può contenere al massimo 255 caratteri"; }
                }
                else
                { return "L'istituto è obbligatorio e può contenere al massimo 255 caratteri"; }
            }
            else
            { return "I codici inseriti sono già in uso o non sono validi"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}