namespace VulpesX.DAL.Tables.CustomerRating
{
    public interface Icr_tab_ratingsRepository
    {

        ObservableCollection<cr_tab_ratings>? GetList(string CompanyID);

        cr_tab_ratings? Get(string CompanyID, string ID);

        bool Exists(string CompanyID, string ID);

        #region CRUD
        bool Insert(cr_tab_ratings Model);

        bool Update(cr_tab_ratings Model);

        bool Delete(cr_tab_ratings Model);

        string? Validate(cr_tab_ratings Model, bool IsInsert);

        bool GenerateDefault(string CompanyID);
        #endregion
    }

    public class cr_tab_ratingsRepository : RepositoryBase, Icr_tab_ratingsRepository
    {
        public cr_tab_ratingsRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<cr_tab_ratings>? GetList(string CompanyID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<cr_tab_ratings>(
                    "SELECT * FROM cr_tab_ratings  WHERE societaID = @SocietaID", new { SocietaID = CompanyID });

                return new ObservableCollection<cr_tab_ratings>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public cr_tab_ratings? Get(string CompanyID, string ID)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<cr_tab_ratings>(
                    "SELECT * FROM cr_tab_ratings WHERE societaID = @SocietaID and id = @ID",
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
                    "SELECT COUNT(*) FROM cr_tab_ratings WHERE societaID = @SocietaID and id = @ID",
                    new { SocietaID = CompanyID, ID = ID }) > 0;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        #region CRUD
        public bool Insert(cr_tab_ratings Model)
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO cr_tab_ratings (societaID,id,description,from_point,to_point,sign,color,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID)" +
                " OUTPUT INSERTED.rv VALUES(@societaID,@id,@description,@from_point,@to_point,@sign,@color,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID)",
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

        public bool Update(cr_tab_ratings Model)
        {
            using var connection = GetOpenConnection();



            var result = connection.ExecuteScalar(
                @"UPDATE cr_tab_ratings SET LogAdded = @LogAdded, LogUpdated = @LogUpdated,LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID,LogCanceledUserID = @LogCanceledUserID,
                            description = @description, from_point=@from_point, to_point = @to_point, color=@color, sign = @sign OUTPUT INSERTED.rv WHERE societaID = @SocietaID AND id = @id AND rv = @rv",
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

        public bool Delete(cr_tab_ratings Model)
        {
            try
            {
                using var connection = GetOpenConnection();



                var result = connection.Execute(
             "DELETE FROM cr_tab_ratings OUTPUT DELETED.rv WHERE societaID = @SocietaID AND id = @ID AND rv = @rv",
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

        public string? Validate(cr_tab_ratings Model, bool IsInsert)
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
           "DELETE FROM cr_tab_ratings OUTPUT DELETED.rv WHERE societaID = @SocietaID",
           new { SocietaID = CompanyID });

            var listDefault = new List<cr_tab_ratings>();
            listDefault.Add(new cr_tab_ratings
            {
                societaID = CompanyID,
                id = "001",
                description = "Elevata capacità di ripagare il debito",
                from_point = 0,
                to_point = 1,
                sign = "AAA",
                color = "#FF1EC77E"
            });
            listDefault.Add(new cr_tab_ratings
            {
                societaID = CompanyID,
                id = "002",
                description = "Alta capacità di pagare il debito",
                from_point = 2,
                to_point = 3,
                sign = "AA",
                color = "#FFE8EB24"
            });
            listDefault.Add(new cr_tab_ratings
            {
                societaID = CompanyID,
                id = "003",
                description = "Solida capacità di ripagare il debito",
                from_point = 4,
                to_point = 4,
                sign = "A",
                color = "#FFEBBB13"
            });
            listDefault.Add(new cr_tab_ratings
            {
                societaID = CompanyID,
                id = "004",
                description = "Debito prevalentemente speculativo",
                from_point = 5,
                to_point = 6,
                sign = "B",
                color = "#FFF68C69"
            });
            listDefault.Add(new cr_tab_ratings
            {
                societaID = CompanyID,
                id = "005",
                description = "Debito altamente speculativo",
                from_point = 6,
                to_point = 999999,
                sign = "C",
                color = "#FFDE2D2D"
            });

            foreach (var rt in listDefault)
            {
                result = connection.Execute(
                "INSERT INTO cr_tab_ratings (societaID,id,description,from_point,to_point,sign,color,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID)" +
                " OUTPUT INSERTED.rv VALUES(@societaID,@id,@description,@from_point,@to_point,@sign,@color,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID)",
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
