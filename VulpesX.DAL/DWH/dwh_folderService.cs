namespace VulpesX.DAL.DWH
{
    public interface Idwh_folderRepository
    {
        DWH_Folder? Get(string CompanyID, Guid ID);

        bool Exists(string CompanyID, Guid ID);

        #region CRUD
        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }
        string DELETE_QUERY { get; }

        bool Insert(DWH_Folder Model);

        bool Update(DWH_Folder Model);

        bool Delete(string CompanyID, Guid FolderID);

        string? Validate(DWH_Folder Model, bool IsInsert);
        #endregion
    }

    public class dwh_folderRepository : RepositoryBase, Idwh_folderRepository
    {
        public dwh_folderRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public DWH_Folder? Get(string CompanyID, Guid ID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var query = connection.Query<DWH_Folder>(
                    "SELECT * FROM DWH_Folder WHERE SocietaID = @SocietaID and FolderID = @FolderID",
                    new { SocietaID = CompanyID, FolderID = ID })
                    .FirstOrDefault();

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
                        "SELECT COUNT(*) FROM DWH_Folder WHERE SocietaID = @SocietaID and FolderID = @ID",
                        new { SocietaID = CompanyID, ID = ID }) > 0;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO DWH_Folder (SocietaID,FolderID,FolderID_Father,Descrizione,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID) OUTPUT INSERTED.rv VALUES(@SocietaID,@FolderID,@FolderID_Father,@Descrizione,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID)";
        public string UPDATE_QUERY => "UPDATE DWH_Folder SET FolderID_Father = @FolderID_Father,Descrizione = @Descrizione,LogAdded = @LogAdded,LogUpdated = @LogUpdated,LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID, LogCanceledUserID = @LogCanceledUserID OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND FolderID =@FolderID AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM DWH_Folder OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND FolderID =@FolderID AND rv = @rv";

        public bool Insert(DWH_Folder Model)
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var result = connection.Execute(INSERT_QUERY, Model, transaction);

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

        public bool Update(DWH_Folder Model)
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var result = connection.ExecuteScalar(UPDATE_QUERY, Model, transaction);

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

        public bool Delete(string CompanyID, Guid FolderID)
        {
            try
            {
                using var connection = GetOpenConnection();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var result = connection.ExecuteScalar("DELETE FROM DWH_Folder OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND FolderID =@FolderID", new { SocietaID = CompanyID, FolderID = FolderID }, transaction);

                        var templates = connection.ExecuteScalar("UPDATE DWH_Template SET FolderID = null WHERE SocietaID = @SocietaID AND FolderID =@FolderID", new { SocietaID = CompanyID, FolderID = FolderID }, transaction);

                        transaction.Commit();

                        var sub = connection.Query<DWH_Folder>(
                    "SELECT * FROM DWH_Folder WHERE SocietaID = @SocietaID AND FolderID_Father = @FolderID", new { SocietaID = CompanyID, FolderID = FolderID });



                        foreach (var sb in sub)
                        {
                            Delete(sb.SocietaID, sb.FolderID);
                        }

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

        public string? Validate(DWH_Folder Model, bool IsInsert)
        {
            try
            {
                if (Model.FolderID != Guid.Empty && IsInsert && !Exists(Model.SocietaID, Model.FolderID) || !IsInsert)
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

    public class dwh_folderUfpRepository : RepositoryBase, Idwh_folderRepository
    {
        public dwh_folderUfpRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public DWH_Folder? Get(string CompanyID, Guid ID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var query = connection.Query<DWH_Folder>(
                    @"SELECT 
cid as SocietaID,
fid as FolderID,
fid_father as FolderIDFather,
description as Descrizione,
creator as LogAddedUserID,
*
FROM DWH_Folder WHERE cid = @SocietaID and fid = @FolderID",
                    new { SocietaID = CompanyID, FolderID = ID })
                    .FirstOrDefault();

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
                        "SELECT COUNT(*) FROM DWH_Folder WHERE cid = @SocietaID and fid = @ID",
                        new { SocietaID = CompanyID, ID = ID }) > 0;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO DWH_Folder (cid,fid,fid_father,description,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID) OUTPUT INSERTED.rv VALUES(@SocietaID,@FolderID,@FolderID_Father,@Descrizione,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID)";
        public string UPDATE_QUERY => "UPDATE DWH_Folder SET fid_father = @FolderID_Father,description = @Descrizione,LogAdded = @LogAdded,LogUpdated = @LogUpdated,LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID, LogCanceledUserID = @LogCanceledUserID OUTPUT INSERTED.rv WHERE cid = @SocietaID AND fid =@FolderID AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM DWH_Folder OUTPUT DELETED.rv WHERE cid = @SocietaID AND fid =@FolderID AND rv = @rv";

        public bool Insert(DWH_Folder Model)
        {
            using var connection = GetOpenConnection();

            try
            {
                var result = connection.Execute(INSERT_QUERY, Model);

                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }

        }

        public bool Update(DWH_Folder Model)
        {
            using var connection = GetOpenConnection();

            try
            {
                var result = connection.ExecuteScalar(UPDATE_QUERY, Model);

                if (result != null)
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }

        }

        public bool Delete(string CompanyID, Guid FolderID)
        {
            try
            {
                using var connection = GetOpenConnection();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var result = connection.ExecuteScalar("DELETE FROM DWH_Folder OUTPUT DELETED.rv WHERE cid = @SocietaID AND fid =@FolderID", new { SocietaID = CompanyID, FolderID = FolderID }, transaction);

                        var templates = connection.ExecuteScalar("UPDATE DWH_Template SET fid = null WHERE cid = @SocietaID AND fid =@FolderID", new { SocietaID = CompanyID, FolderID = FolderID }, transaction);

                        transaction.Commit();

                        var sub = connection.Query<DWH_Folder>(
                    "SELECT * FROM DWH_Folder WHERE cid = @SocietaID AND fid_father = @FolderID", new { SocietaID = CompanyID, FolderID = FolderID });



                        foreach (var sb in sub)
                        {
                            Delete(sb.SocietaID, sb.FolderID);
                        }

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

        public string? Validate(DWH_Folder Model, bool IsInsert)
        {
            try
            {
                if (Model.FolderID != Guid.Empty && IsInsert && !Exists(Model.SocietaID, Model.FolderID) || !IsInsert)
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
