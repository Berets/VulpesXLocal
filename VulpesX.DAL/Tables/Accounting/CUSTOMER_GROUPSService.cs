using VulpesX.DAL;

namespace VulpesX.DAL.Tables.Accounting;


public interface ICUSTOMER_GROUPSRepository
{
    ObservableCollection<CUSTOMER_GROUPS>? GetList();

    ObservableCollection<CUSTOMER_GROUPS>? GetList(string cccsoc, int cccode);

    ObservableCollection<CUSTOMER_GROUPS>? GetList(string cccsoc, int cccode, string cccaus);

    CUSTOMER_GROUPS? Get(string cccsoc, int cccode, int ccprog);

    bool Exists(string cccsoc, int cccode, int ccprog);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(CUSTOMER_GROUPS Model);

    bool Update(CUSTOMER_GROUPS Model);

    bool Delete(CUSTOMER_GROUPS Model);

    string? Validate(CUSTOMER_GROUPS Model, bool IsInsert);
    #endregion
}

public class CUSTOMER_GROUPSRepository : RepositoryBase, ICUSTOMER_GROUPSRepository
{
    public CUSTOMER_GROUPSRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<CUSTOMER_GROUPS>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<CUSTOMER_GROUPS>(
                @"SELECT * FROM CUSTOMER_GROUPS
                        ORDER BY ccprog");

            return new ObservableCollection<CUSTOMER_GROUPS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<CUSTOMER_GROUPS>? GetList(string cccsoc, int cccode)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<CUSTOMER_GROUPS>(
                @"SELECT * FROM CUSTOMER_GROUPS
                        WHERE cccsoc=@cccsoc AND cccode=@cccode
                        ORDER BY ccprog", new { cccsoc = cccsoc, cccode = cccode });

            return new ObservableCollection<CUSTOMER_GROUPS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<CUSTOMER_GROUPS>? GetList(string cccsoc, int cccode, string cccaus)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<CUSTOMER_GROUPS>(
                @"SELECT * FROM CUSTOMER_GROUPS
                        WHERE cccsoc=@cccsoc AND cccode=@cccode AND cccaus=@cccaus
                        ORDER BY ccprog", new { cccsoc = cccsoc, cccode = cccode, cccaus = cccaus });

            return new ObservableCollection<CUSTOMER_GROUPS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CUSTOMER_GROUPS? Get(string cccsoc, int cccode, int ccprog)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<CUSTOMER_GROUPS>(
                "SELECT * FROM CUSTOMER_GROUPS WHERE cccsoc = @cccsoc AND cccode = @cccode AND ccprog = @ccprog",
                new { cccsoc = cccsoc, cccode = cccode, ccprog = ccprog })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string cccsoc, int cccode, int ccprog)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM CUSTOMER_GROUPS WHERE cccsoc = @cccsoc AND cccode = @cccode AND ccprog = @ccprog",
                new { cccsoc = cccsoc, cccode = cccode, ccprog = ccprog }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO CUSTOMER_GROUPS (cccsoc,cccode,ccprog,ccgrup,cccont,ccsott,ccsegn,cccaus) OUTPUT INSERTED.rv VALUES(@cccsoc,@cccode,@ccprog,@ccgrup,@cccont,@ccsott,@ccsegn,@cccaus)";
    public string UPDATE_QUERY => "UPDATE CUSTOMER_GROUPS SET cccsoc = @cccsoc,cccode = @cccode,ccprog = @ccprog,ccgrup = @ccgrup,cccont = @cccont,ccsott = @ccsott,ccsegn = @ccsegn,cccaus = @cccaus OUTPUT INSERTED.rv WHERE cccsoc = @cccsoc AND cccode = @cccode AND ccprog = @ccprog AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM CUSTOMER_GROUPS OUTPUT DELETED.rv WHERE cccsoc = @cccsoc AND cccode = @cccode AND ccprog = @ccprog AND rv = @rv";
    public bool Insert(CUSTOMER_GROUPS Model)
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

    public bool Update(CUSTOMER_GROUPS Model)
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

    public bool Delete(CUSTOMER_GROUPS Model)
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

    public string? Validate(CUSTOMER_GROUPS Model, bool IsInsert)
    {
        try
        {
            if (Model.cccode > 0)
            {
                if (Model.SelectedGroup != null && Model.SelectedAccount != null && Model.SelectedSubaccount != null)
                {
                    if (Model.SelectedCausal != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Model.ccsegn))
                        {
                            return null;
                        }
                        else
                        { return "Il segno è obbligatorio"; }
                    }
                    else
                    { return "La causale contabile è obbligatoria"; }
                }
                else
                { return "Gruppo, conto e sottoconto sono obbligatori"; }
            }
            else
            { return "Il codice cliente è obbligatorio"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}