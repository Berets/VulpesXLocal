using VulpesX.DAL;
using VulpesX.DAL.Tables.CRM;

namespace VulpesX.Services.Tables.CRM;

public interface ITAB_CRM_CAUORDRepository
{
    ObservableCollection<TAB_CRM_CAUORD>? GetList(string CompanyID);

    ObservableCollection<TAB_CRM_CAUORD>? GetList();

    TAB_CRM_CAUORD? Get(string CompanyID, string cauacq);

    TAB_CRM_CAUORD? Get(string cauacq);

    TAB_CRM_CAUORD? GetFull(string cauacqsoc, string cauacq);

    TAB_CRM_CAUORD? GetFull(string cauacq);

    bool Exists(string CompanyID, string cauacq);

    bool Exists(string cauacq);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(TAB_CRM_CAUORD Model);

    bool Update(TAB_CRM_CAUORD Model);

    bool Delete(TAB_CRM_CAUORD Model);

    string? Validate(TAB_CRM_CAUORD Model, bool IsInsert);
    #endregion
}

public class TAB_CRM_CAUORDRepository : RepositoryBase, ITAB_CRM_CAUORDRepository
{
    public TAB_CRM_CAUORDRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<TAB_CRM_CAUORD>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<TAB_CRM_CAUORD, CAUSBOLL, TAB_CRM_CAUORD>(
                @"SELECT c.*, cb.* FROM TAB_CRM_CAUORD AS c
                        LEFT JOIN CAUSBOLL AS cb ON cb.bolcod = c.caubol
                        WHERE c.cauacqsoc = @cid",
                (cau, caub) => { cau.CausalDDT = caub; return cau; },
                new { cid = CompanyID }, splitOn: "bolcod");

            return new ObservableCollection<TAB_CRM_CAUORD>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<TAB_CRM_CAUORD>? GetList()
    {
        throw new NotImplementedException();
    }

    public TAB_CRM_CAUORD? Get(string CompanyID, string cauacq)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TAB_CRM_CAUORD>(
                @"SELECT * FROM TAB_CRM_CAUORD 
                        WHERE cauacqsoc = @cid AND cauacq = @cauacq",
                new { cid = CompanyID, cauacq = cauacq })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TAB_CRM_CAUORD? Get(string cauacq)
    {
        throw new NotImplementedException();
    }

    public TAB_CRM_CAUORD? GetFull(string cauacqsoc, string cauacq)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TAB_CRM_CAUORD, TAB_GEN_TEXTS, TAB_CRM_CAUORD>(
                @"SELECT c.*, t.* FROM TAB_CRM_CAUORD AS c
                        LEFT OUTER JOIN TAB_GEN_TEXTS AS t ON t.TTxsoc = c.cauacqsoc AND t.TTxcod = c.cauacqtxc AND t.TTXtip = c.cauacqtxt
                        WHERE c.cauacqsoc = @cauacqsoc AND c.cauacq = @cauacq",
                (cau, tex) => { cau.Text = tex; return cau; },
                new { cauacqsoc = cauacqsoc, cauacq = cauacq }, splitOn: "TTxsoc")
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TAB_CRM_CAUORD? GetFull(string cauacq)
    {
        throw new NotImplementedException();
    }

    public bool Exists(string CompanyID, string cauacq)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM TAB_CRM_CAUORD 
                        WHERE cauacqsoc = @cid AND cauacq = @cauacq",
                new { cid = CompanyID, cauacq = cauacq }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public bool Exists(string cauacq)
    {
        throw new NotImplementedException();
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO TAB_CRM_CAUORD (cauacq,caudec,caufla,cauflb,caubol,caugrp,caucon,causoc,cauacqsoc,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote,cauacqtxc,cauacqtxt,caufat) OUTPUT INSERTED.rv VALUES(@cauacq,@caudec,@caufla,@cauflb,@caubol,@caugrp,@caucon,@causoc,@cauacqsoc,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote,@cauacqtxc,@cauacqtxt,@caufat)";
    public string UPDATE_QUERY => "UPDATE TAB_CRM_CAUORD SET cauacq = @cauacq,caudec = @caudec,caufla = @caufla,cauflb = @cauflb,caubol = @caubol,caugrp = @caugrp,caucon = @caucon,causoc = @causoc,cauacqsoc = @cauacqsoc,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote,cauacqtxc = @cauacqtxc,cauacqtxt = @cauacqtxt,caufat = @caufat OUTPUT INSERTED.rv WHERE cauacqsoc = @cauacqsoc AND cauacq = @cauacq AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM TAB_CRM_CAUORD OUTPUT DELETED.rv WHERE cauacqsoc = @cauacqsoc AND cauacq = @cauacq AND rv = @rv";
    public bool Insert(TAB_CRM_CAUORD Model)
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

    public bool Update(TAB_CRM_CAUORD Model)
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

    public bool Delete(TAB_CRM_CAUORD Model)
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

    public string? Validate(TAB_CRM_CAUORD Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.cauacq) && IsInsert && !Exists(Model.cauacqsoc, Model.cauacq)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.caudec))
                {
                    return null;
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

public class TAB_CRM_CAUORDUfpRepository : RepositoryBase, ITAB_CRM_CAUORDRepository
{
    public TAB_CRM_CAUORDUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<TAB_CRM_CAUORD>? GetList(string CompanyID)
    {
        throw new NotImplementedException();
    }

    public ObservableCollection<TAB_CRM_CAUORD>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<TAB_CRM_CAUORD, CAUSBOLL, TAB_CRM_CAUORD>(
                @"SELECT c.*, cb.* FROM CAUSORD AS c
                        LEFT JOIN CAUSBOLL AS cb ON cb.bolcod = c.caubol",
                (cau, caub) => { cau.CausalDDT = caub; return cau; }, splitOn: "bolcod");

            return new ObservableCollection<TAB_CRM_CAUORD>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TAB_CRM_CAUORD? Get(string CompanyID, string cauacq)
    {
        throw new NotImplementedException();
    }

    public TAB_CRM_CAUORD? Get(string cauacq)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TAB_CRM_CAUORD>(
                @"SELECT * FROM CAUSORD 
                        WHERE  cauacq = @cauacq",
                new {  cauacq = cauacq })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TAB_CRM_CAUORD? GetFull(string cauacqsoc, string cauacq)
    {
        throw new NotImplementedException();
    }

    public TAB_CRM_CAUORD? GetFull(string cauacq)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TAB_CRM_CAUORD, TAB_GEN_TEXTS, TAB_CRM_CAUORD>(
                @"SELECT c.*, t.* FROM CAUSORD AS c
                        LEFT OUTER JOIN TAB_GEN_TEXTS AS t ON t.TTxsoc = c.cauacqsoc AND t.TTxcod = c.cauacqtxc AND t.TTXtip = c.cauacqtxt
                        WHERE c.cauacq = @cauacq",
                (cau, tex) => { cau.Text = tex; return cau; },
                new { cauacq = cauacq }, splitOn: "TTxsoc")
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string CompanyID, string cauacq)
    {
       throw new NotImplementedException();
    }

    public bool Exists( string cauacq)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM CAUSORD 
                        WHERE cauacq = @cauacq",
                new { cauacq = cauacq }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO CAUSORD (cauacq,caudec,caufla,cauflb,caubol,caugrp,caucon,causoc) OUTPUT INSERTED.rv VALUES(@cauacq,@caudec,@caufla,@cauflb,@caubol,@caugrp,@caucon,@causoc)";
    public string UPDATE_QUERY => "UPDATE CAUSORD SET cauacq = @cauacq,caudec = @caudec,caufla = @caufla,cauflb = @cauflb,caubol = @caubol,caugrp = @caugrp,caucon = @caucon,causoc = @causoc OUTPUT INSERTED.rv WHERE cauacqsoc = @cauacqsoc AND cauacq = @cauacq AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM CAUSORD OUTPUT DELETED.rv WHERE cauacq = @cauacq AND rv = @rv";
    public bool Insert(TAB_CRM_CAUORD Model)
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

    public bool Update(TAB_CRM_CAUORD Model)
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

    public bool Delete(TAB_CRM_CAUORD Model)
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

    public string? Validate(TAB_CRM_CAUORD Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.cauacq) && IsInsert && !Exists(Model.cauacqsoc, Model.cauacq)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.caudec))
                {
                    return null;
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