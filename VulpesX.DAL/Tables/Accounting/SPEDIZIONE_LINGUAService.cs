using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.DAL.Tables.Accounting
{
    public interface ISPEDIZIONE_LINGUARepository
    {

        SPEDIZIONE_LINGUA? Get(string ID, string lincod);

        #region CRUD
        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }
        string DELETE_QUERY { get; }

        bool Insert(SPEDIZIONE_LINGUA Model);

        bool InsertOrUpdate(SPEDIZIONE_LINGUA Model);

        bool Update(SPEDIZIONE_LINGUA Model);

        bool Delete(SPEDIZIONE_LINGUA Model);
        #endregion
    }

    public class SPEDIZIONE_LINGUARepository : RepositoryBase, ISPEDIZIONE_LINGUARepository
    {
        public SPEDIZIONE_LINGUARepository(IConnectionFactory factory) : base(factory)
        {
        }

        public SPEDIZIONE_LINGUA? Get(string ID, string lincod)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<SPEDIZIONE_LINGUA>(
                    "SELECT * FROM SPEDIZIONE_LINGUA WHERE specod = @specod AND lincod = @lincod",
                    new { specod = ID, lincod = lincod })
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO SPEDIZIONE_LINGUA (specod,lincod,spedes) OUTPUT INSERTED.rv VALUES(@specod,@lincod,@spedes)";
        public string UPDATE_QUERY => "UPDATE SPEDIZIONE_LINGUA SET specod = @specod, lincod = @lincod, spedes=@spedes OUTPUT INSERTED.rv WHERE specod = @specod AND lincod = @lincod AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM SPEDIZIONE_LINGUA OUTPUT DELETED.rv WHERE specod = @specod AND lincod = @lincod AND rv = @rv";

        public bool Insert(SPEDIZIONE_LINGUA Model)
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

        public bool InsertOrUpdate(SPEDIZIONE_LINGUA Model)
        {
            try
            {
                using var connection = GetOpenConnection();


                var exist = connection.Query<SPEDIZIONE_LINGUA>("SELECT * FROM SPEDIZIONE_LINGUA WHERE specod = @specod AND lincod = @lincod", new { specod = Model.specod, lincod = Model.lincod }).FirstOrDefault();

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

        public bool Update(SPEDIZIONE_LINGUA Model)
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

        public bool Delete(SPEDIZIONE_LINGUA Model)
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
