namespace VulpesX.DAL.Tables.Accounting;


public interface ICAUCONT_GROUPSRepository
{
    ObservableCollection<CAUCONT_GROUPS>? GetList();

    ObservableCollection<CAUCONT_GROUPS>? GetList(string grpsoc, string caucod);

    CAUCONT_GROUPS? GetListNoCR(string grpsoc, string caucod);

    CAUCONT_GROUPS? GetFirstRicavo(string grpsoc, string caucod, string Sign);

    CAUCONT_GROUPS? GetFirstSign(string grpsoc, string caucod, string Sign);

    CAUCONT_GROUPS? Get(string grpsoc, string caucod, int prog);

    bool Exists(string grpsoc, string caucod, int prog);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(CAUCONT_GROUPS Model);

    bool Update(CAUCONT_GROUPS Model);

    bool Delete(CAUCONT_GROUPS Model);

    string? Validate(CAUCONT_GROUPS Model, bool IsInsert);
    #endregion
}

public class CAUCONT_GROUPSRepository : RepositoryBase, ICAUCONT_GROUPSRepository
{
    public CAUCONT_GROUPSRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<CAUCONT_GROUPS>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<CAUCONT_GROUPS>(
                @"SELECT * FROM CAUCONT_GROUPS
                        ORDER BY prog");

            return new ObservableCollection<CAUCONT_GROUPS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<CAUCONT_GROUPS>? GetList(string grpsoc, string caucod)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<CAUCONT_GROUPS>(
                @"SELECT * FROM CAUCONT_GROUPS
                        WHERE grpsoc=@grpsoc AND caucod=@caucod
                        ORDER BY prog", new { grpsoc = grpsoc, caucod = caucod });

            return new ObservableCollection<CAUCONT_GROUPS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CAUCONT_GROUPS? GetListNoCR(string grpsoc, string caucod)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<CAUCONT_GROUPS>(
                @"SELECT TOP (1) m.* FROM CAUCONT_GROUPS AS m
                        INNER JOIN PDCGRUPPI AS g ON g.P1GRUP=m.grpgrp
                        WHERE m.grpsoc=@grpsoc AND m.caucod=@caucod AND g.P1TICO<>'C' AND g.P1TICO<>'R' AND m.grpseg='A'
                        ORDER BY m.prog", new { grpsoc = grpsoc, caucod = caucod }).FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CAUCONT_GROUPS? GetFirstRicavo(string grpsoc, string caucod, string Sign)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<CAUCONT_GROUPS>(
                @"SELECT TOP(1) m.* FROM CAUCONT_GROUPS AS m
                        INNER JOIN PDCGRUPPI AS g ON g.P1GRUP=m.grpgrp
                        WHERE m.grpsoc=@grpsoc AND m.caucod=@caucod AND g.P1TICO = 'R' AND m.grpseg=@sign
                        ORDER BY m.prog", new { grpsoc = grpsoc, caucod = caucod, sign = Sign }).FirstOrDefault();


        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CAUCONT_GROUPS? GetFirstSign(string grpsoc, string caucod, string Sign)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<CAUCONT_GROUPS>(
                @"SELECT TOP(1) m.* FROM CAUCONT_GROUPS AS m
                        WHERE m.grpsoc=@grpsoc AND m.caucod=@caucod AND m.grpseg=@sign
                        ORDER BY m.prog", new { grpsoc = grpsoc, caucod = caucod, sign = Sign }).FirstOrDefault();


        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CAUCONT_GROUPS? Get(string grpsoc, string caucod, int prog)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<CAUCONT_GROUPS>(
                "SELECT * FROM CAUCONT_GROUPS WHERE grpsoc = @grpsoc AND caucod = @caucod AND prog = @prog",
                new { grpsoc = grpsoc, caucod = caucod, prog = prog })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string grpsoc, string caucod, int prog)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM CAUCONT_GROUPS WHERE grpsoc = @grpsoc AND caucod = @caucod AND prog = @prog",
                new { grpsoc = grpsoc, caucod = caucod, prog = prog }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO CAUCONT_GROUPS (grpsoc,caucod,prog,grpgrp,grpcto,grpsct,grpseg) OUTPUT INSERTED.rv VALUES(@grpsoc,@caucod,@prog,@grpgrp,@grpcto,@grpsct,@grpseg)";
    public string UPDATE_QUERY => "UPDATE CAUCONT_GROUPS SET grpsoc = @grpsoc,caucod = @caucod,prog = @prog,grpgrp = @grpgrp,grpcto = @grpcto,grpsct = @grpsct,grpseg = @grpseg OUTPUT INSERTED.rv WHERE grpsoc = @grpsoc AND caucod = @caucod AND prog = @prog AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM CAUCONT_GROUPS OUTPUT DELETED.rv WHERE grpsoc = @grpsoc AND caucod = @caucod AND prog = @prog AND rv = @rv";
    public bool Insert(CAUCONT_GROUPS Model)
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

    public bool Update(CAUCONT_GROUPS Model)
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

    public bool Delete(CAUCONT_GROUPS Model)
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

    public string? Validate(CAUCONT_GROUPS Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.caucod)))
            {
                if (Model.SelectedGroup != null && Model.SelectedAccount != null && Model.SelectedSubaccount != null)
                {
                    if (!string.IsNullOrWhiteSpace(Model.grpseg))
                    {
                        return null;
                    }
                    else
                    { return "Il segno è obbligatorio"; }
                }
                else
                { return "Gruppo, conto e sottoconto somo obbligatori"; }
            }
            else
            { return "Il codice della causale è obbligatorio"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}

public class CAUCONT_GROUPSUfpRepository : RepositoryBase, ICAUCONT_GROUPSRepository
{
    public CAUCONT_GROUPSUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<CAUCONT_GROUPS>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<CAUCONT_GROUPS>(
                @"SELECT * FROM GRPCAUCOD
                        ORDER BY prog");

            return new ObservableCollection<CAUCONT_GROUPS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<CAUCONT_GROUPS>? GetList(string grpsoc, string caucod)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<CAUCONT_GROUPS>(
                @"SELECT * FROM GRPCAUCOD
                        WHERE grpsoc=@grpsoc AND caucod=@caucod
                        ORDER BY prog", new { grpsoc = grpsoc, caucod = caucod });

            return new ObservableCollection<CAUCONT_GROUPS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CAUCONT_GROUPS? GetListNoCR(string grpsoc, string caucod)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<CAUCONT_GROUPS>(
                @"SELECT TOP (1) m.* FROM GRPCAUCOD AS m
                        INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=m.grpgrp
                        WHERE m.grpsoc=@grpsoc AND m.caucod=@caucod AND g.P1TICO<>'C' AND g.P1TICO<>'R' AND m.grpseg='A'
                        ORDER BY m.prog", new { grpsoc = grpsoc, caucod = caucod }).FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CAUCONT_GROUPS? GetFirstRicavo(string grpsoc, string caucod, string Sign)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<CAUCONT_GROUPS>(
                @"SELECT TOP(1) m.* FROM GRPCAUCOD AS m
                        INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=m.grpgrp
                        WHERE m.grpsoc=@grpsoc AND m.caucod=@caucod AND g.P1TICO = 'R' AND m.grpseg=@sign
                        ORDER BY m.prog", new { grpsoc = grpsoc, caucod = caucod, sign = Sign }).FirstOrDefault();


        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CAUCONT_GROUPS? GetFirstSign(string grpsoc, string caucod, string Sign)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<CAUCONT_GROUPS>(
                @"SELECT TOP(1) m.* FROM GRPCAUCOD AS m
                        WHERE m.grpsoc=@grpsoc AND m.caucod=@caucod AND m.grpseg=@sign
                        ORDER BY m.prog", new { grpsoc = grpsoc, caucod = caucod, sign = Sign }).FirstOrDefault();


        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CAUCONT_GROUPS? Get(string grpsoc, string caucod, int prog)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<CAUCONT_GROUPS>(
                "SELECT * FROM GRPCAUCOD WHERE grpsoc = @grpsoc AND caucod = @caucod AND prog = @prog",
                new { grpsoc = grpsoc, caucod = caucod, prog = prog })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string grpsoc, string caucod, int prog)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM GRPCAUCOD WHERE grpsoc = @grpsoc AND caucod = @caucod AND prog = @prog",
                new { grpsoc = grpsoc, caucod = caucod, prog = prog }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO GRPCAUCOD (grpsoc,caucod,prog,grpgrp,grpcto,grpsct,grpseg) OUTPUT INSERTED.rv VALUES(@grpsoc,@caucod,@prog,@grpgrp,@grpcto,@grpsct,@grpseg)";
    public string UPDATE_QUERY => "UPDATE GRPCAUCOD SET grpsoc = @grpsoc,caucod = @caucod,prog = @prog,grpgrp = @grpgrp,grpcto = @grpcto,grpsct = @grpsct,grpseg = @grpseg OUTPUT INSERTED.rv WHERE grpsoc = @grpsoc AND caucod = @caucod AND prog = @prog AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM GRPCAUCOD OUTPUT DELETED.rv WHERE grpsoc = @grpsoc AND caucod = @caucod AND prog = @prog AND rv = @rv";
    public bool Insert(CAUCONT_GROUPS Model)
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

    public bool Update(CAUCONT_GROUPS Model)
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

    public bool Delete(CAUCONT_GROUPS Model)
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

    public string? Validate(CAUCONT_GROUPS Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.caucod)))
            {
                if (Model.SelectedGroup != null && Model.SelectedAccount != null && Model.SelectedSubaccount != null)
                {
                    if (!string.IsNullOrWhiteSpace(Model.grpseg))
                    {
                        return null;
                    }
                    else
                    { return "Il segno è obbligatorio"; }
                }
                else
                { return "Gruppo, conto e sottoconto somo obbligatori"; }
            }
            else
            { return "Il codice della causale è obbligatorio"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}