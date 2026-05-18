namespace VulpesX.DAL.DWH
{
    public interface Idwh_templateRepository
    {
        ObservableCollection<DWH_Template>? GetList(string CompanyID);

        DWH_Template? Get(string CompanyID, Guid ID);

        List<DWH_Template>? GetFoldersTemplates(string CompanyID, string UserID);

        List<DWH_Template>? GetRootTemplates(string CompanyID, string UserID);

        List<DWH_Template>? GetFolders(string CompanyID);

        List<DWH_Template>? GetFoldersChilds(string CompanyID, Guid FatherID);

        bool Exists(string CompanyID, Guid QueryID, Guid ID);

        #region CRUD
        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }
        string DELETE_QUERY { get; }

        string INSERT_QUERY_PARAMETER { get; }
        string UPDATE_QUERY_PARAMETER { get; }
        string DELETE_QUERY_PARAMETER { get; }

        bool Insert(DWH_Template Model);

        bool Update(DWH_Template Model);

        bool Delete(DWH_Template Model);

        string? Validate(DWH_Template Model, bool IsInsert);
        #endregion
    }

    public class dwh_templateRepository : RepositoryBase, Idwh_templateRepository
    {
        public dwh_templateRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<DWH_Template>? GetList(string CompanyID)
        {
            try
            {
                using var connection = GetOpenConnection();
                var list = connection.Query<DWH_Template>(
                        "SELECT * FROM DWH_Template WHERE SocietaID = @SocietaID", new { SocietaID = CompanyID });

                return new ObservableCollection<DWH_Template>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public DWH_Template? Get(string CompanyID, Guid ID)
        {
            try
            {
                using var connection = GetOpenConnection();
                var query = connection.Query<DWH_Template>(
                        "SELECT * FROM DWH_Template WHERE SocietaID = @SocietaID and ID = @ID",
                        new { SocietaID = CompanyID, ID = ID })
                        .FirstOrDefault();

                if (query != null)
                {
                    query.Parametri = new ObservableCollection<DWH_TemplateParameter>(connection.Query<DWH_TemplateParameter>(
                    "SELECT * FROM DWH_TemplateParameter WHERE SocietaID = @SocietaID AND ID = @ID", new { SocietaID = CompanyID, ID = query.ID }));
                }

                return query;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public List<DWH_Template>? GetFoldersTemplates(string CompanyID, string UserID)
        {
            using var connection = GetOpenConnection();

            var folders = connection.Query<DWH_Folder>(
                @"SELECT *
                  FROM DWH_Folder
                  WHERE SocietaID = @SocietaID",
                new { SocietaID = CompanyID }
            ).ToList();

            var templates = connection.Query<DWH_Template>(
                @"SELECT *,
                    1 as IsTemplate
                  FROM DWH_Template
                  WHERE SocietaID = @SocietaID AND  (IsShared = 1 OR LogAddedUserID = @UserID)",
                new { SocietaID = CompanyID, UserID = UserID }
            ).ToList();

            var folderByParent = folders
        .ToLookup(f => f.FolderID_Father);
            var templatesByFolder = templates
                .ToLookup(t => t.FolderID);

            return BuildFolderTree(null, folderByParent, templatesByFolder);
        }

        private List<DWH_Template> BuildFolderTree(Guid? parentId, ILookup<Guid?, DWH_Folder> Folders, ILookup<Guid?, DWH_Template> Templates)
        {
            var result = new List<DWH_Template>();

            foreach (var fld in Folders[parentId]) // works with null
            {
                var node = new DWH_Template
                {
                    SocietaID = fld.SocietaID,
                    ID = fld.FolderID,
                    StreamName = fld.Descrizione,
                    IsTemplate = false,
                    Childs = new List<DWH_Template>()
                };

                node.Childs.AddRange(BuildFolderTree(fld.FolderID, Folders, Templates));
                node.Childs.AddRange(Templates[fld.FolderID]);

                result.Add(node);
            }

            return result;
        }

        public List<DWH_Template>? GetRootTemplates(string CompanyID, string UserID)
        {
            try
            {
                using var connection = GetOpenConnection();

                var list = connection.Query<DWH_Template>(
                "SELECT * FROM DWH_Template WHERE SocietaID = @SocietaID AND FolderID is null AND  (IsShared = 1 OR LogAddedUserID = @UserID)", new { SocietaID = CompanyID, UserID = UserID });

                foreach (var tmp in list)
                {
                    tmp.IsTemplate = true;
                }

                return new List<DWH_Template>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }

        }

        public List<DWH_Template>? GetFolders(string CompanyID)
        {
            try
            {
                using var connection = GetOpenConnection();

                var retValue = new List<DWH_Template>();
                retValue.Add(new DWH_Template
                {
                    SocietaID = CompanyID,
                    ID = Guid.Empty,
                    StreamName = "-NESSUNA CARTELLA-"
                });

                var list = connection.Query<DWH_Folder>(
                    "SELECT * FROM DWH_Folder WHERE SocietaID = @SocietaID AND FolderID_Father is null", new { SocietaID = CompanyID });

                foreach (var fld in list)
                {
                    var tmp = new DWH_Template
                    {
                        SocietaID = fld.SocietaID,
                        ID = fld.FolderID,
                        FolderID = fld.FolderID,
                        StreamName = fld.Descrizione,
                        IsTemplate = false,
                        Childs = new List<DWH_Template>()
                    };
                    tmp.Childs.AddRange(GetFoldersChilds(fld.SocietaID, fld.FolderID) ?? new List<DWH_Template>());

                    retValue.Add(tmp);
                }

                return retValue;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public List<DWH_Template>? GetFoldersChilds(string CompanyID, Guid FatherID)
        {
            try
            {
                using var connection = GetOpenConnection();

                var retValue = new List<DWH_Template>();

                var list = connection.Query<DWH_Folder>(
                    "SELECT * FROM DWH_Folder WHERE SocietaID = @SocietaID AND FolderID_Father = @FatherID", new { SocietaID = CompanyID, FatherID = FatherID });

                foreach (var fld in list)
                {
                    var tmp = new DWH_Template
                    {
                        SocietaID = fld.SocietaID,
                        ID = fld.FolderID,
                        StreamName = fld.Descrizione,
                        IsTemplate = false,
                        Childs = new List<DWH_Template>()
                    };
                    //tmp.Childs.AddRange(GetFoldersTemplatesChilds(fld.SocietaID, fld.FolderID) ?? new List<DWH_Template>());

                    retValue.Add(tmp);
                }

                return retValue;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool Exists(string CompanyID, Guid QueryID, Guid ID)
        {
            try
            {
                using var connection = GetOpenConnection();
                return (int?)connection.ExecuteScalar(
                        "SELECT COUNT(*) FROM DWH_Template WHERE SocietaID = @SocietaID and QueryID = @QueryID and ID = @ID",
                        new { SocietaID = CompanyID, QueryID = QueryID, ID = ID }) > 0;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO DWH_Template (SocietaID,QueryID,ID,FolderID,StreamByte,StreamName,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID) OUTPUT INSERTED.rv VALUES(@SocietaID,@QueryID,@ID,@FolderID,@StreamByte,@StreamName,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID)";
        public string UPDATE_QUERY => "UPDATE DWH_Template SET FolderID = @FolderID,StreamByte = @StreamByte, StreamName =@StreamName,IsShared = @IsShared,LogAdded = @LogAdded,LogUpdated = @LogUpdated,LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID, LogCanceledUserID = @LogCanceledUserID OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND QueryID = @QueryID AND ID =@ID AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM DWH_Template OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND QueryID = @QueryID AND ID =@ID AND rv = @rv";

        public string INSERT_QUERY_PARAMETER => "INSERT INTO DWH_TemplateParameter (SocietaID,QueryID,ID,Nome,Valore) OUTPUT INSERTED.rv VALUES(@SocietaID,@QueryID,@ID,@Nome,@Valore)";
        public string UPDATE_QUERY_PARAMETER => "UPDATE DWH_TemplateParameter SET Valore=@Valore OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND QueryID = @QueryID AND ID =@ID AND Nome=@Nome rv = @rv";
        public string DELETE_QUERY_PARAMETER => "DELETE FROM DWH_TemplateParameter OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND ID =@ID AND Nome=@Nome AND rv = @rv";

        public bool Insert(DWH_Template Model)
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var result = connection.Execute(INSERT_QUERY, Model, transaction);

                    //pulisce DWH_QueryParameter
                    result = connection.Execute(
                        "DELETE FROM DWH_TemplateParameter OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND QueryID=@QueryID AND ID =@ID",
                        new { SocietaID = Model.SocietaID, QueryID = Model.QueryID, ID = Model.ID }, transaction);

                    foreach (var item in Model.Parametri ?? new ObservableCollection<DWH_TemplateParameter>())
                    {
                        result = connection.Execute(INSERT_QUERY_PARAMETER,
                        new DWH_TemplateParameter { SocietaID = Model.SocietaID, QueryID = Model.QueryID, ID = Model.ID, Nome = item.Nome, Valore = item.Valore }, transaction);
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

        public bool Update(DWH_Template Model)
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var result = connection.ExecuteScalar(UPDATE_QUERY, Model, transaction);

                    //pulisce DWH_QueryParameter
                    result = connection.Execute(
                        "DELETE FROM DWH_TemplateParameter OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND QueryID = @QueryID AND ID =@ID",
                        new { SocietaID = Model.SocietaID, QueryID = Model.QueryID, ID = Model.ID }, transaction);

                    foreach (var item in Model.Parametri ?? new ObservableCollection<DWH_TemplateParameter>())
                    {
                        result = connection.Execute(INSERT_QUERY_PARAMETER,
                        new DWH_TemplateParameter { SocietaID = Model.SocietaID, QueryID = Model.QueryID, ID = Model.ID, Nome = item.Nome, Valore = item.Valore }, transaction);
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

        public bool Delete(DWH_Template Model)
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
                            "DELETE FROM DWH_TemplateParameter OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND QueryID = @QueryID AND ID =@ID",
                            new { SocietaID = Model.SocietaID, QueryID = Model.QueryID, ID = Model.ID }, transaction);

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

        public string? Validate(DWH_Template Model, bool IsInsert)
        {
            try
            {
                if (Model.ID != Guid.Empty && IsInsert && !Exists(Model.SocietaID, Model.QueryID, Model.ID) || !IsInsert)
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

    public class dwh_templateUfpRepository : RepositoryBase, Idwh_templateRepository
    {
        public dwh_templateUfpRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<DWH_Template>? GetList(string CompanyID)
        {
            try
            {
                using var connection = GetOpenConnection();
                var list = connection.Query<DWH_Template>(
                        @"SELECT 
cid as SocietaID,
uid as QueryID,
tui as ID,
fid as FolderID,
stream_byte as StreamByte,
stream_name as StreamNamem
is_shared as IsShared,
creator as LogAddedUserID,
updated as LogUpdated
FROM DWH_Template WHERE cid = @SocietaID", new { SocietaID = CompanyID });

                return new ObservableCollection<DWH_Template>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public DWH_Template? Get(string CompanyID, Guid ID)
        {
            try
            {
                using var connection = GetOpenConnection();
                var query = connection.Query<DWH_Template>(
                        @"SELECT 
cid as SocietaID,
uid as QueryID,
tui as ID,
fid as FolderID,
stream_byte as StreamByte,
stream_name as StreamName,
is_shared as IsShared,
creator as LogAddedUserID,
updated as LogUpdated,
*
FROM DWH_Template WHERE cid = @SocietaID and tui = @ID",
                        new { SocietaID = CompanyID, ID = ID })
                        .FirstOrDefault();

                if (query != null)
                {
                    query.Parametri = new ObservableCollection<DWH_TemplateParameter>(connection.Query<DWH_TemplateParameter>(
                    @"SELECT 
cid as SocietaID,
uid as QueryID,
tui as ID,
parameter_name as Nome,
parameter_value as Valore,
*
FROM DWH_TemplateParameter WHERE cid = @SocietaID AND uid = @ID AND tui = @tui", new { SocietaID = CompanyID, ID = query.QueryID, tui = ID }));
                }

                return query;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public List<DWH_Template>? GetFoldersTemplates(string CompanyID, string UserID)
        {
            using var connection = GetOpenConnection();

            var folders = connection.Query<DWH_Folder>(
                @"SELECT 
                    cid as SocietaID,
                    fid as FolderID,
                    fid_father as FolderIDFather,
                    description as Descrizione,
                    creator as LogAddedUserID
                    FROM DWH_Folder
                    WHERE cid = @SocietaID
                    ORDER BY description",
                new { SocietaID = CompanyID }
            ).ToList();

            var templates = connection.Query<DWH_Template>(
                @"SELECT
                    cid as SocietaID,
                    uid as QueryID,
                    tui as ID,
                    fid as FolderID,
                    stream_byte as StreamByte,
                    stream_name as StreamName,
                    is_shared as IsShared,
                    creator as LogAddedUserID,
                    updated as LogUpdated,
                    1 as IsTemplate
                  FROM DWH_Template
                  WHERE cid = @SocietaID AND (is_shared = 1 OR creator = @UserID)
                  ORDER BY stream_name",
                new { SocietaID = CompanyID, UserID = UserID }
            ).ToList();

            var folderByParent = folders
                .ToLookup(f => f.FolderID_Father);
            var templatesByFolder = templates
                .ToLookup(t => t.FolderID);

            return BuildFolderTree(null, folderByParent, templatesByFolder);
        }

        private List<DWH_Template> BuildFolderTree(Guid? parentId, ILookup<Guid?, DWH_Folder> Folders, ILookup<Guid?, DWH_Template> Templates)
        {
            var result = new List<DWH_Template>();

            foreach (var fld in Folders[parentId])
            {
                var node = new DWH_Template
                {
                    SocietaID = fld.SocietaID,
                    ID = fld.FolderID,
                    StreamName = fld.Descrizione,
                    IsTemplate = false,
                    Childs = new List<DWH_Template>()
                };

                var templates = Templates[fld.FolderID];

                node.Childs.AddRange(BuildFolderTree(fld.FolderID, Folders, Templates));
                node.Childs.AddRange(templates);

                if (templates.Any())
                    result.Add(node);
            }

            return result;
        }

        public List<DWH_Template>? GetRootTemplates(string CompanyID, string UserID)
        {
            try
            {
                using var connection = GetOpenConnection();

                var list = connection.Query<DWH_Template>(
                @"SELECT 
cid as SocietaID,
uid as QueryID,
tui as ID,
fid as FolderID,
stream_byte as StreamByte,
stream_name as StreamName,
is_shared as IsShared,
creator as LogAddedUserID,
updated as LogUpdated,
*
FROM DWH_Template WHERE cid = @SocietaID AND fid is null AND (is_shared = 1 OR creator = @UserID)
ORDER BY stream_name 
", new { SocietaID = CompanyID, UserID = UserID });


                foreach (var tmp in list)
                {
                    tmp.IsTemplate = true;
                }

                return new List<DWH_Template>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }

        }

        public List<DWH_Template>? GetFolders(string CompanyID)
        {
            try
            {
                using var connection = GetOpenConnection();

                var retValue = new List<DWH_Template>();
                retValue.Add(new DWH_Template
                {
                    SocietaID = CompanyID,
                    ID = Guid.Empty,
                    StreamName = "-NESSUNA CARTELLA-"
                });

                var list = connection.Query<DWH_Folder>(
                    @"SELECT 
cid as SocietaID,
fid as FolderID,
fid_father as FolderIDFather,
description as Descrizione,
creator as LogAddedUserID,
*
FROM DWH_Folder WHERE cid = @SocietaID AND fid_father is null", new { SocietaID = CompanyID });

                foreach (var fld in list)
                {
                    var tmp = new DWH_Template
                    {
                        SocietaID = fld.SocietaID,
                        ID = fld.FolderID,
                        FolderID = fld.FolderID,
                        StreamName = fld.Descrizione,
                        IsTemplate = false,
                        Childs = new List<DWH_Template>()
                    };
                    tmp.Childs.AddRange(GetFoldersChilds(fld.SocietaID, fld.FolderID) ?? new List<DWH_Template>());

                    retValue.Add(tmp);
                }

                return retValue;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public List<DWH_Template>? GetFoldersChilds(string CompanyID, Guid FatherID)
        {
            try
            {
                using var connection = GetOpenConnection();

                var retValue = new List<DWH_Template>();

                var list = connection.Query<DWH_Folder>(
                    @"SELECT 
cid as SocietaID,
fid as FolderID,
fid_father as FolderIDFather,
description as Descrizione,
creator as LogAddedUserID,
*
FROM DWH_Folder WHERE cid = @SocietaID AND fid_father = @FatherID", new { SocietaID = CompanyID, FatherID = FatherID });

                foreach (var fld in list)
                {
                    var tmp = new DWH_Template
                    {
                        SocietaID = fld.SocietaID,
                        ID = fld.FolderID,
                        StreamName = fld.Descrizione,
                        IsTemplate = false,
                        Childs = new List<DWH_Template>()
                    };
                    //tmp.Childs.AddRange(GetFoldersTemplatesChilds(fld.SocietaID, fld.FolderID) ?? new List<DWH_Template>());

                    retValue.Add(tmp);
                }

                return retValue;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool Exists(string CompanyID, Guid QueryID, Guid ID)
        {
            try
            {
                using var connection = GetOpenConnection();
                return (int?)connection.ExecuteScalar(
                        "SELECT COUNT(*) FROM DWH_Template WHERE cid = @SocietaID and uid = @QueryID and tui = @ID",
                        new { SocietaID = CompanyID, QueryID = QueryID, ID = ID }) > 0;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO DWH_Template (cid,uid,tui,fid,Stream_Byte,Stream_Name,is_shared,creator,updated, description) OUTPUT INSERTED.rv VALUES(@SocietaID,@QueryID,@ID,@FolderID,@StreamByte,@StreamName,@IsShared,@LogAddedUserID,@LogAdded, @description)";
        public string UPDATE_QUERY => "UPDATE DWH_Template SET fid = @FolderID,Stream_Byte = @StreamByte, Stream_Name =@StreamName,Is_Shared = @IsShared,updated = @LogUpdated,creator = @LogUpdatedUserID, description = @description OUTPUT INSERTED.rv WHERE cid = @SocietaID AND uid = @QueryID AND tui =@ID AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM DWH_Template OUTPUT DELETED.rv WHERE cid = @SocietaID AND uid = @QueryID AND tui =@ID AND rv = @rv";

        public string INSERT_QUERY_PARAMETER => "INSERT INTO DWH_TemplateParameter (cid,uid,tui,parameter_name, parameter_type,parameter_value) OUTPUT INSERTED.rv VALUES(@SocietaID,@QueryID,@ID,@Nome,@Tipo,@ParameterValue)";
        public string UPDATE_QUERY_PARAMETER => "UPDATE DWH_TemplateParameter SET parameter_value=@ParameterValue OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND QueryID = @QueryID AND ID =@ID AND Nome=@Nome rv = @rv";
        public string DELETE_QUERY_PARAMETER => "DELETE FROM DWH_TemplateParameter OUTPUT DELETED.rv WHERE cid = @SocietaID AND uid =@ID AND Nome=@Nome AND rv = @rv";

        public bool Insert(DWH_Template Model)
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var result = connection.Execute(INSERT_QUERY, Model, transaction);

                    //pulisce DWH_QueryParameter
                    result = connection.Execute(
                        "DELETE FROM DWH_TemplateParameter OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND QueryID=@QueryID AND ID =@ID",
                        new { SocietaID = Model.SocietaID, QueryID = Model.QueryID, ID = Model.ID }, transaction);

                    foreach (var item in Model.Parametri ?? new ObservableCollection<DWH_TemplateParameter>())
                    {
                        result = connection.Execute(INSERT_QUERY_PARAMETER,
                        new DWH_TemplateParameter { SocietaID = Model.SocietaID, QueryID = Model.QueryID, ID = Model.ID, Nome = item.Nome, Valore = item.Valore }, transaction);
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

        public bool Update(DWH_Template Model)
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var result = connection.ExecuteScalar(UPDATE_QUERY, Model, transaction);

                    //pulisce DWH_QueryParameter
                    result = connection.Execute(
                        "DELETE FROM DWH_TemplateParameter OUTPUT DELETED.rv WHERE cid = @SocietaID AND uid = @QueryID AND tui =@ID",
                        new { SocietaID = Model.SocietaID, QueryID = Model.QueryID, ID = Model.ID }, transaction);

                    foreach (var item in Model.Parametri ?? new ObservableCollection<DWH_TemplateParameter>())
                    {
                        result = connection.Execute(INSERT_QUERY_PARAMETER, item, transaction);
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

        public bool Delete(DWH_Template Model)
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
                            "DELETE FROM DWH_TemplateParameter OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND QueryID = @QueryID AND ID =@ID",
                            new { SocietaID = Model.SocietaID, QueryID = Model.QueryID, ID = Model.ID }, transaction);

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

        public string? Validate(DWH_Template Model, bool IsInsert)
        {
            try
            {
                if (Model.ID != Guid.Empty && IsInsert && !Exists(Model.SocietaID, Model.QueryID, Model.ID) || !IsInsert)
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
