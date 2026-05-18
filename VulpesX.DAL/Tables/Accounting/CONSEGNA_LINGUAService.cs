
namespace VulpesX.DAL.Tables.Accounting
{
    public interface ICONSEGNA_LINGUARepository
    {

        CONSEGNA_LINGUA? Get(string ID, string lincod);

        #region CRUD
        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }
        string DELETE_QUERY { get; }

        bool Insert(CONSEGNA_LINGUA Model);

        bool InsertOrUpdate(CONSEGNA_LINGUA Model);

        bool Update(CONSEGNA_LINGUA Model);

        bool Delete(CONSEGNA_LINGUA Model);
        #endregion
    }

    public class CONSEGNA_LINGUARepository : RepositoryBase, ICONSEGNA_LINGUARepository
    {
        public CONSEGNA_LINGUARepository(IConnectionFactory factory) : base(factory)
        {
        }

        public CONSEGNA_LINGUA? Get(string ID, string lincod)
        {
            try
            {
                using var connection = GetOpenConnection();

                return connection.Query<CONSEGNA_LINGUA>(
                    "SELECT * FROM CONSEGNA_LINGUA WHERE concod = @concod AND lincod = @lincod",
                    new { concod = ID, lincod = lincod })
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO CONSEGNA_LINGUA (concod,lincod,condes) OUTPUT INSERTED.rv VALUES(@concod,@lincod,@condes)";
        public string UPDATE_QUERY => "UPDATE CONSEGNA_LINGUA SET concod = @concod, lincod = @lincod, condes=@condes OUTPUT INSERTED.rv WHERE concod = @concod AND lincod = @lincod AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM CONSEGNA_LINGUA OUTPUT DELETED.rv WHERE concod = @concod AND lincod = @lincod AND rv = @rv";

        public bool Insert(CONSEGNA_LINGUA Model)
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

        public bool InsertOrUpdate(CONSEGNA_LINGUA Model)
        {
            try
            {
                using var connection = GetOpenConnection();



                var exist = connection.Query<CONSEGNA_LINGUA>("SELECT * FROM CONSEGNA_LINGUA WHERE concod = @concod AND lincod = @lincod", new { concod = Model.concod, lincod = Model.lincod }).FirstOrDefault();

                if (exist != null)
                {
                    var result = connection.Execute(UPDATE_QUERY, Model);
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
                else
                {
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

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public bool Update(CONSEGNA_LINGUA Model)
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

        public bool Delete(CONSEGNA_LINGUA Model)
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
        #endregion
    }
}
