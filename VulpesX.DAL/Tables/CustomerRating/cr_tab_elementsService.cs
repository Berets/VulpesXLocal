using VulpesX.DAL;

namespace VulpesX.DAL.Tables.CustomerRating
{
    public interface Icr_tab_elementsRepository
    {
        ObservableCollection<cr_tab_elements>? GetList(string CompanyID);

        cr_tab_elements? Get(string CompanyID, string ID);

        bool Exists(string CompanyID, string ID);

        #region CRUD
        bool Insert(cr_tab_elements Model);

        bool Update(cr_tab_elements Model);

        bool Delete(cr_tab_elements Model);

        string? Validate(cr_tab_elements Model, bool IsInsert);

        bool GenerateDefault(string CompanyID);
        #endregion
    }

    public class cr_tab_elementsRepository : RepositoryBase, Icr_tab_elementsRepository
    {
        public cr_tab_elementsRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<cr_tab_elements>? GetList(string CompanyID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<cr_tab_elements>(
                    "SELECT * FROM cr_tab_elements  WHERE societaID = @SocietaID", new { SocietaID = CompanyID });

                return new ObservableCollection<cr_tab_elements>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public cr_tab_elements? Get(string CompanyID, string ID)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<cr_tab_elements>(
                    "SELECT * FROM cr_tab_elements WHERE societaID = @SocietaID and id = @ID",
                    new { SocietaID = CompanyID, ID = ID })
                    .FirstOrDefault();

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool Exists(string CompanyID, string ID)
        {
            try
            {
                using var connection = GetOpenConnection();


                return (int?)connection.ExecuteScalar(
                    "SELECT COUNT(*) FROM cr_tab_elements WHERE societaID = @SocietaID and id = @ID",
                    new { SocietaID = CompanyID, ID = ID }) > 0;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        #region CRUD
        public bool Insert(cr_tab_elements Model)
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO cr_tab_elements (societaID,id,description,percentage,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID)" +
                " OUTPUT INSERTED.rv VALUES(@societaID,@id,@description,@percentage,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID)",
                Model);

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

        public bool Update(cr_tab_elements Model)
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                @"UPDATE cr_tab_elements SET LogAdded = @LogAdded, LogUpdated = @LogUpdated,LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID,LogCanceledUserID = @LogCanceledUserID,
                            description = @description, percentage=@percentage OUTPUT INSERTED.rv WHERE societaID = @SocietaID AND id = @id AND rv = @rv",
                Model);

            if (result != null)
            {
                return true;
            }
            else
            {
                ErrorHandler.Show(Constants.INSERT_VIOLATION);
                return false;
            }


        }

        public bool Delete(cr_tab_elements Model)
        {
            try
            {
                using var connection = GetOpenConnection();



                var result = connection.Execute(
             "DELETE FROM cr_tab_elements OUTPUT DELETED.rv WHERE societaID = @SocietaID AND id = @ID AND rv = @rv",
             Model);

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

        public string? Validate(cr_tab_elements Model, bool IsInsert)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Model.id) && IsInsert && !Exists(Model.societaID, Model.id) || !IsInsert)
                {
                    if (!string.IsNullOrWhiteSpace(Model.description) && Model.description.Length <= 255)
                    {
                        return null;
                    }
                    else
                    { return "La descrizione è obbligatoria e può contenere al massimo 255 caratteri"; }
                }
                else
                { return "Il codice inserito è già in uso o non è valido"; }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public bool GenerateDefault(string CompanyID)
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
         "DELETE FROM cr_tab_elements OUTPUT DELETED.rv WHERE societaID = @SocietaID",
         new { SocietaID = CompanyID });

            var listDefault = new List<cr_tab_elements>();
            listDefault.Add(new cr_tab_elements
            {
                societaID = CompanyID,
                id = "FIN",
                description = "Gestione finanziaria",
                percentage = 100,
            });

            result = 0;

            foreach (var rt in listDefault)
            {
                result = connection.Execute(
                "INSERT INTO cr_tab_elements (societaID,id,description,percentage,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID)" +
                " OUTPUT INSERTED.rv VALUES(@societaID,@id,@description,@percentage,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID)",
                rt);
            }

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
        #endregion
    }
}
