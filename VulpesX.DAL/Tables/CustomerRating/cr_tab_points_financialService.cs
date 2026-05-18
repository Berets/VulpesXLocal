using VulpesX.DAL;

namespace VulpesX.DAL.Tables.CustomerRating
{
    public interface Icr_tab_points_financialRepository
    {

        ObservableCollection<cr_tab_points_financial>? GetList(string CompanyID);

        cr_tab_points_financial? Get(string CompanyID, string ID);

        bool Exists(string CompanyID, string ID);

        #region CRUD
        bool Insert(cr_tab_points_financial Model);

        bool Update(cr_tab_points_financial Model);

        bool Delete(cr_tab_points_financial Model);

        string? Validate(cr_tab_points_financial Model, bool IsInsert);

        bool GenerateDefault(string CompanyID);
        #endregion
    }

    public class cr_tab_points_financialRepository : RepositoryBase, Icr_tab_points_financialRepository
    {
        public cr_tab_points_financialRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<cr_tab_points_financial>? GetList(string CompanyID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<cr_tab_points_financial, string, cr_tab_points_financial>(
                    $@"SELECT p.*, c.caudes FROM cr_tab_points_financial as p
                            LEFT OUTER JOIN CAUCONT as c ON c.caucod = p.unsolved_causal_id
                            WHERE societaID = @SocietaID"
                     , (punti, causale) =>
                     {
                         punti.Unsolved_Causal_Description = causale;
                         return punti;
                     }, new { SocietaID = CompanyID }, splitOn: "caudes");

                return new ObservableCollection<cr_tab_points_financial>(list);
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public cr_tab_points_financial? Get(string CompanyID, string ID)
        {
            try
            {
                using var connection = GetOpenConnection();

              
                    return connection.Query<cr_tab_points_financial>(
                        "SELECT * FROM cr_tab_points_financial WHERE societaID = @SocietaID and id = @ID",
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
                        "SELECT COUNT(*) FROM cr_tab_points_financial WHERE societaID = @SocietaID and id = @ID",
                        new { SocietaID = CompanyID, ID = ID }) > 0;
                
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        #region CRUD
        public bool Insert(cr_tab_points_financial Model)
        {
            using var connection = GetOpenConnection();

           
                var result = connection.Execute(
                    "INSERT INTO cr_tab_points_financial (societaID,id,description,point,delay_days,unsolved_causal_id,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID)" +
                    " OUTPUT INSERTED.rv VALUES(@societaID,@id,@description,@point,@delay_days,@unsolved_causal_id,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID)",
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

        public bool Update(cr_tab_points_financial Model)
        {
            using var connection = GetOpenConnection();

            

                var result = connection.ExecuteScalar(
                    @"UPDATE cr_tab_points_financial SET LogAdded = @LogAdded, LogUpdated = @LogUpdated,LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID,LogCanceledUserID = @LogCanceledUserID, description = @description, point = @point, delay_days = @delay_days, unsolved_causal_id = @unsolved_causal_id OUTPUT INSERTED.rv WHERE societaID = @SocietaID AND id = @id AND rv = @rv",
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

        public bool Delete(cr_tab_points_financial Model)
        {
            try
            {
                using var connection = GetOpenConnection();

            
                    var result = connection.Execute(
                 "DELETE FROM cr_tab_points_financial OUTPUT DELETED.rv WHERE societaID = @SocietaID AND id = @ID AND rv = @rv",
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

        public string? Validate(cr_tab_points_financial Model, bool IsInsert)
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
             "DELETE FROM cr_tab_points_financial OUTPUT DELETED.rv WHERE societaID = @SocietaID",
             new { SocietaID = CompanyID });

                var listDefault = new List<cr_tab_points_financial>();
                listDefault.Add(new cr_tab_points_financial
                {
                    societaID = CompanyID,
                    id = "001",
                    description = "Insoluto",
                    point = 2,
                    delay_days = 0,
                });
                listDefault.Add(new cr_tab_points_financial
                {
                    societaID = CompanyID,
                    id = "002",
                    description = "Ritardo pagamento 30 gg",
                    point = 3,
                    delay_days = 30,
                });
                listDefault.Add(new cr_tab_points_financial
                {
                    societaID = CompanyID,
                    id = "003",
                    description = "Ritardo pagamento 60 gg",
                    point = 5,
                    delay_days = 60,
                });
                listDefault.Add(new cr_tab_points_financial
                {
                    societaID = CompanyID,
                    id = "004",
                    description = "Ritardo pagamento 90 gg",
                    point = 7,
                    delay_days = 90,
                });
                listDefault.Add(new cr_tab_points_financial
                {
                    societaID = CompanyID,
                    id = "005",
                    description = "Ritardo pagamento 120 gg",
                    point = 9,
                    delay_days = 120,
                });

                result = 0;

                foreach (var rt in listDefault)
                {
                    result = connection.Execute(
                    "INSERT INTO cr_tab_points_financial (societaID,id,description,point,delay_days,unsolved_causal_id,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID)" +
                    " OUTPUT INSERTED.rv VALUES(@societaID,@id,@description,@point,@delay_days,@unsolved_causal_id,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID)",
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
