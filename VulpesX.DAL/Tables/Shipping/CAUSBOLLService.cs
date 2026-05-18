namespace VulpesX.DAL.Tables.Shipping;

public interface ICAUSBOLLRepository
{
    ObservableCollection<CAUSBOLL>? GetList(string? EntityType = null);

    CAUSBOLL? Get(string bolcod);

    bool Exists(string bolcod);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(CAUSBOLL Model);

    bool Update(CAUSBOLL Model);

    string? CanDelete(string ID);

    bool Delete(CAUSBOLL Model);

    string? Validate(CAUSBOLL Model, bool IsInsert);
    #endregion
}

public class CAUSBOLLRepository : RepositoryBase, ICAUSBOLLRepository
{
    public CAUSBOLLRepository(IConnectionFactory factory) : base(factory)
    {
    }

    /// <summary>
    /// Get shipping causals list
    /// </summary>
    /// <param name="EntityType">C - customers, F - supliers, null - all</param>
    /// <returns></returns>
    public ObservableCollection<CAUSBOLL>? GetList(string? EntityType = null)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<CAUSBOLL>(
                $@"SELECT * FROM CAUSBOLL
                        {(string.IsNullOrWhiteSpace(EntityType) ? null : (EntityType == "C" ? " WHERE bolcli = 'S' " : (EntityType == "F" ? " WHERE bolfor = 'S'" : null)))}");

            return new ObservableCollection<CAUSBOLL>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CAUSBOLL? Get(string bolcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<CAUSBOLL>(
                "SELECT * FROM CAUSBOLL WHERE bolcod = @bolcod",
                new { bolcod = bolcod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string bolcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM CAUSBOLL WHERE bolcod = @bolcod",
                new { bolcod = bolcod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO CAUSBOLL (bolcod,boldes,BOLCAU,bolmag,bolfat,bolfac,bolcli,bolfor,bolnum,bolpre) OUTPUT INSERTED.rv VALUES(@bolcod,@boldes,@BOLCAU,@bolmag,@bolfat,@bolfac,@bolcli,@bolfor,@bolnum,@bolpre)";
    public string UPDATE_QUERY => "UPDATE CAUSBOLL SET bolcod = @bolcod,boldes = @boldes,BOLCAU = @BOLCAU,bolmag = @bolmag,bolfat = @bolfat,bolfac = @bolfac,bolcli = @bolcli,bolfor = @bolfor,bolnum = @bolnum,bolpre = @bolpre OUTPUT INSERTED.rv WHERE bolcod = @bolcod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM CAUSBOLL OUTPUT DELETED.rv WHERE bolcod = @bolcod AND rv = @rv";
    public bool Insert(CAUSBOLL Model)
    {
        try
        {
            using var connection = GetOpenConnection();


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

    public bool Update(CAUSBOLL Model)
    {
        try
        {
            using var connection = GetOpenConnection();


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

    public string? CanDelete(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var multiAccount = connection.QueryMultiple(
                                    @"SELECT COUNT(*) FROM BOLLT00F WHERE BTCAUS = @id;
                                            SELECT COUNT(*) FROM TAB_CRM_CAUORD WHERE caubol = @id;"
            , new { id = ID });
            var ddts = multiAccount.Read<int?>().Single() ?? 0;
            var ordersCausals = multiAccount.Read<int?>().Single() ?? 0;

            if (ddts == 0 && ordersCausals == 0)
            {
                return null;
            }
            else
            {
                return $"Impossibile cancellare la causale [{ID}] perchè in uso nelle seguenti gestioni:\n\n" +
                            $"{(ddts > 0 ? "- DDT\n" : null)}" +
                            $"{(ordersCausals > 0 ? "- Causali ordini clienti\n" : null)}";
            }

        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public bool Delete(CAUSBOLL Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var check = CanDelete(Model.bolcod);
            if (string.IsNullOrWhiteSpace(check))
            {

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
            else
            {
                ErrorHandler.Show(check);
                return false;
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public string? Validate(CAUSBOLL Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.bolcod) && IsInsert && !Exists(Model.bolcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.boldes))
                {
                    if ((Model.bolmagBool && !string.IsNullOrWhiteSpace(Model.BOLCAU)) ||
                        (!Model.bolmagBool && string.IsNullOrWhiteSpace(Model.BOLCAU)))
                    {
                        if ((Model.bolfatBool && !string.IsNullOrWhiteSpace(Model.bolfac)) ||
                        (!Model.bolfatBool && string.IsNullOrWhiteSpace(Model.bolfac)))
                        {
                            return null;
                        }
                        else
                        { return "Se è prevista la generazione di fatture la causale fattura è obbligatoria"; }
                    }
                    else
                    { return "Se è prevista una scrittura di magazzino la causale di magazzino è obbligatoria"; }
                }
                else
                { return "La descrizione è obbligatoria"; }
            }
            else
            { return "Il codice inserito è già in uso o non è valido"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}