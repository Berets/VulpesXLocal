using Microsoft.Data.SqlClient;
using System.Data;

namespace VulpesX.DAL.DWH
{
    public interface Idwh_queryRepository
    {

        ObservableCollection<DWH_Query>? GetList(string CompanyID);

        DWH_Query? Get(string CompanyID, Guid ID);

        bool Exists(string CompanyID, Guid ID);

        DataTable? Execute(DWH_Query Query);

        #region CRUD
        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }
        string DELETE_QUERY { get; }

        string INSERT_QUERY_PARAMETER { get; }
        string UPDATE_QUERY_PARAMETER { get; }
        string DELETE_QUERY_PARAMETER { get; }

        bool Insert(DWH_Query Model);

        bool Update(DWH_Query Model);

        bool Delete(DWH_Query Model);

        string? Validate(DWH_Query Model, bool IsInsert);
        #endregion
    }

    public class dwh_queryRepository : RepositoryBase, Idwh_queryRepository
    {
        public dwh_queryRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<DWH_Query>? GetList(string CompanyID)
        {
            try
            {
                using var connection = GetOpenConnection();

                var list = connection.Query<DWH_Query>(
                        "SELECT * FROM DWH_QUERY WHERE SocietaID = @SocietaID ORDER BY Titolo", new { SocietaID = CompanyID });

                return new ObservableCollection<DWH_Query>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public DWH_Query? Get(string CompanyID, Guid ID)
        {
            try
            {
                using var connection = GetOpenConnection();

                var query = connection.Query<DWH_Query>(
                        "SELECT * FROM DWH_Query WHERE SocietaID = @SocietaID and ID = @ID",
                        new { SocietaID = CompanyID, ID = ID })
                        .FirstOrDefault();

                if (query != null)
                {
                    query.Parametri = new ObservableCollection<DWH_QueryParameter>(connection.Query<DWH_QueryParameter>(
                    "SELECT * FROM DWH_QUERYPARAMETER WHERE SocietaID = @SocietaID AND ID = @ID", new { SocietaID = CompanyID, ID = query.ID }));
                }

                return query;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool Exists(string CompanyID, Guid ID)
        {
            try
            {
                using var connection = GetOpenConnection();

                return (int?)connection.ExecuteScalar(
                        "SELECT COUNT(*) FROM DWH_Query WHERE SocietaID = @SocietaID and ID = @ID",
                        new { SocietaID = CompanyID, ID = ID }) > 0;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        public DataTable? Execute(DWH_Query Query)
        {
            using var connection = GetOpenConnection();

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = Query.Query;

                if (Query.EStoredProcedure ?? false)
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = Query.StoredProcedureName;
                }

                foreach (var parameter in Query.Parametri ?? new ObservableCollection<DWH_QueryParameter>())
                {
                    var param = cmd.CreateParameter();
                    param.ParameterName = parameter.Nome;

                    if (parameter.Tipo == (int)SqlDbType.DateTime)
                        param.Value = parameter.ParameterDate;
                    else
                        param.Value = parameter.ParameterValue ?? DBNull.Value;

                    cmd.Parameters.Add(param);
                }

                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                return dt;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO DWH_Query (SocietaID,ID,Titolo,Query,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID, EStoredProcedure, StoredProcedureName) OUTPUT INSERTED.rv VALUES(@SocietaID,@ID,@Titolo,@Query,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID,@EStoredProcedure,@StoredProcedureName)";
        public string UPDATE_QUERY => "UPDATE DWH_Query SET Titolo = @Titolo,Query = @Query,LogAdded = @LogAdded,LogUpdated = @LogUpdated,LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID, LogCanceledUserID = @LogCanceledUserID, EStoredProcedure = @EStoredProcedure,StoredProcedureName = @StoredProcedureName OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND ID =@ID AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM DWH_Query OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND ID =@ID AND rv = @rv";

        public string INSERT_QUERY_PARAMETER => "INSERT INTO DWH_QueryParameter (SocietaID,ID,Nome,Posizione,Tipo) OUTPUT INSERTED.rv VALUES(@SocietaID,@ID,@Nome,@Posizione,@Tipo)";
        public string UPDATE_QUERY_PARAMETER => "UPDATE DWH_QueryParameter SET Nome=@Nome,Posizione@Posizione, Tipo = @Tipo OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND ID =@ID AND Nome=@Nome rv = @rv";
        public string DELETE_QUERY_PARAMETER => "DELETE FROM DWH_QueryParameter OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND ID =@ID AND Nome=@Nome AND rv = @rv";

        public bool Insert(DWH_Query Model)
        {
            using var connection = GetOpenConnection();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var result = connection.Execute(INSERT_QUERY, Model, transaction);

                    //pulisce DWH_QueryParameter
                    result = connection.Execute(
                        "DELETE FROM DWH_QueryParameter OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND ID =@ID",
                        new { SocietaID = Model.SocietaID, ID = Model.ID }, transaction);

                    foreach (var item in Model.Parametri ?? new ObservableCollection<DWH_QueryParameter>())
                    {
                        result = connection.Execute(INSERT_QUERY_PARAMETER,
                        new DWH_QueryParameter { SocietaID = Model.SocietaID, ID = Model.ID, Nome = item.Nome, Posizione = item.Posizione, Tipo = item.Tipo }, transaction);
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(ex.Message);
                    return false;
                }
            }

        }

        public bool Update(DWH_Query Model)
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var result = connection.ExecuteScalar(UPDATE_QUERY, Model, transaction);

                    //pulisce DWH_QueryParameter
                    result = connection.Execute(
                        "DELETE FROM DWH_QueryParameter OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND ID =@ID",
                        new { SocietaID = Model.SocietaID, ID = Model.ID }, transaction);

                    foreach (var item in Model.Parametri ?? new ObservableCollection<DWH_QueryParameter>())
                    {
                        result = connection.Execute(INSERT_QUERY_PARAMETER,
                        new DWH_QueryParameter { SocietaID = Model.SocietaID, ID = Model.ID, Nome = item.Nome, Posizione = item.Posizione, Tipo = item.Tipo }, transaction);
                    }

                    transaction.Commit();
                    return true;

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(ex.Message);
                    return false;
                }
            }

        }

        public bool Delete(DWH_Query Model)
        {
            try
            {
                using var connection = GetOpenConnection();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var result = connection.ExecuteScalar(DELETE_QUERY, Model, transaction);

                        //pulisce DWH_QueryParameter
                        result = connection.Execute(
                            "DELETE FROM DWH_QueryParameter OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND ID =@ID",
                            new { SocietaID = Model.SocietaID, ID = Model.ID }, transaction);

                        transaction.Commit();
                        return true;

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ErrorHandler.Show(ex.Message);
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public string? Validate(DWH_Query Model, bool IsInsert)
        {
            try
            {
                if (Model.ID != Guid.Empty && IsInsert && !Exists(Model.SocietaID, Model.ID) || !IsInsert)
                {
                    return null;
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

    public class dwh_queryUfpRepository : RepositoryBase, Idwh_queryRepository
    {
        public dwh_queryUfpRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<DWH_Query>? GetList(string CompanyID)
        {
            try
            {
                using var connection = GetOpenConnection();

                var list = connection.Query<DWH_Query>(
                        @$"SELECT 
cid as SocietaID,
uid as ID,
title as Titolo,
* 
FROM DWH_QUERY WHERE cid = @SocietaID ORDER BY title", new { SocietaID = CompanyID });

                return new ObservableCollection<DWH_Query>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public DWH_Query? Get(string CompanyID, Guid ID)
        {
            try
            {
                using var connection = GetOpenConnection();

                var query = connection.Query<DWH_Query>(
                        $@"SELECT 
cid as SocietaID,
uid as ID,
title as Titolo,
*
FROM DWH_Query WHERE cid = @SocietaID and uid = @ID",
                        new { SocietaID = CompanyID, ID = ID })
                        .FirstOrDefault();

                if (query != null)
                {
                    query.Parametri = new ObservableCollection<DWH_QueryParameter>(connection.Query<DWH_QueryParameter>(
                    @"SELECT 
cid as SocietaID,
uid as ID,
parameter_name as Nome,
parameter_position as Posizione,
parameter_type as Tipo,
*
FROM DWH_QUERYPARAMETER WHERE cid = @SocietaID AND uid = @ID", new { SocietaID = CompanyID, ID = query.ID }));
                }

                return query;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool Exists(string CompanyID, Guid ID)
        {
            try
            {
                using var connection = GetOpenConnection();

                return (int?)connection.ExecuteScalar(
                        "SELECT COUNT(*) FROM DWH_Query WHERE cid = @SocietaID and uid = @ID",
                        new { SocietaID = CompanyID, ID = ID }) > 0;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        public DataTable? Execute(DWH_Query Query)
        {
            using var connection = GetOpenConnection();

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = Query.Query;

                if (Query.EStoredProcedure ?? false)
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = Query.StoredProcedureName;
                }

                foreach (var parameter in Query.Parametri ?? new ObservableCollection<DWH_QueryParameter>())
                {
                    var param = cmd.CreateParameter();
                    param.ParameterName = parameter.Nome;

                    if (parameter.Tipo == (int)SqlDbType.DateTime)
                        param.Value = parameter.ParameterDate;
                    else
                        param.Value = parameter.ParameterValue ?? DBNull.Value;

                    cmd.Parameters.Add(param);
                }

                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                return dt;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO DWH_Query (cid,uid,title,query,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID, EStoredProcedure, StoredProcedureName,description) OUTPUT INSERTED.rv VALUES(@SocietaID,@ID,@Titolo,@Query,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID,@EStoredProcedure,@StoredProcedureName,@description)";
        public string UPDATE_QUERY => "UPDATE DWH_Query SET title = @Titolo,Query = @Query,LogAdded = @LogAdded,LogUpdated = @LogUpdated,LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID, LogCanceledUserID = @LogCanceledUserID, EStoredProcedure = @EStoredProcedure,StoredProcedureName = @StoredProcedureName, description = @description OUTPUT INSERTED.rv WHERE cid = @SocietaID AND uid =@ID AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM DWH_Query OUTPUT DELETED.rv WHERE cid = @SocietaID AND uid =@ID AND rv = @rv";

        public string INSERT_QUERY_PARAMETER => "INSERT INTO DWH_QueryParameter (cid,uid,parameter_name,parameter_position,parameter_type) OUTPUT INSERTED.rv VALUES(@SocietaID,@ID,@Nome,@Posizione,@Tipo)";
        public string UPDATE_QUERY_PARAMETER => "UPDATE DWH_QueryParameter SET parameter_name=@Nome,parameter_position@Posizione, parameter_type = @Tipo OUTPUT INSERTED.rv WHERE cid = @SocietaID AND uid =@ID AND parameter_name=@Nome rv = @rv";
        public string DELETE_QUERY_PARAMETER => "DELETE FROM DWH_QueryParameter OUTPUT DELETED.rv WHERE cid = @SocietaID AND uid =@ID AND parameter_name=@Nome AND rv = @rv";

        public bool Insert(DWH_Query Model)
        {
            using var connection = GetOpenConnection();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var result = connection.Execute(INSERT_QUERY, Model, transaction);

                    //pulisce DWH_QueryParameter
                    result = connection.Execute(
                        "DELETE FROM DWH_QueryParameter OUTPUT DELETED.rv WHERE cid = @SocietaID AND uid =@ID",
                        new { SocietaID = Model.SocietaID, ID = Model.ID }, transaction);

                    foreach (var item in Model.Parametri ?? new ObservableCollection<DWH_QueryParameter>())
                    {
                        result = connection.Execute(INSERT_QUERY_PARAMETER,
                        new DWH_QueryParameter { SocietaID = Model.SocietaID, ID = Model.ID, Nome = item.Nome, Posizione = item.Posizione, Tipo = item.Tipo }, transaction);
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(ex.Message);
                    return false;
                }
            }

        }

        public bool Update(DWH_Query Model)
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var result = connection.ExecuteScalar(UPDATE_QUERY, Model, transaction);

                    //pulisce DWH_QueryParameter
                    result = connection.Execute(
                        "DELETE FROM DWH_QueryParameter OUTPUT DELETED.rv WHERE cid = @SocietaID AND uid =@ID",
                        new { SocietaID = Model.SocietaID, ID = Model.ID }, transaction);

                    foreach (var item in Model.Parametri ?? new ObservableCollection<DWH_QueryParameter>())
                    {
                        result = connection.Execute(INSERT_QUERY_PARAMETER,
                        new DWH_QueryParameter { SocietaID = Model.SocietaID, ID = Model.ID, Nome = item.Nome, Posizione = item.Posizione, Tipo = item.Tipo }, transaction);
                    }

                    transaction.Commit();
                    return true;

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(ex.Message);
                    return false;
                }
            }

        }

        public bool Delete(DWH_Query Model)
        {
            try
            {
                using var connection = GetOpenConnection();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var result = connection.ExecuteScalar(DELETE_QUERY, Model, transaction);

                        //pulisce DWH_QueryParameter
                        result = connection.Execute(
                            "DELETE FROM DWH_QueryParameter OUTPUT DELETED.rv WHERE cid = @SocietaID AND uid =@ID",
                            new { SocietaID = Model.SocietaID, ID = Model.ID }, transaction);

                        transaction.Commit();
                        return true;

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ErrorHandler.Show(ex.Message);
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public string? Validate(DWH_Query Model, bool IsInsert)
        {
            try
            {
                if (Model.ID != Guid.Empty && IsInsert && !Exists(Model.SocietaID, Model.ID) || !IsInsert)
                {
                    return null;
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

}
