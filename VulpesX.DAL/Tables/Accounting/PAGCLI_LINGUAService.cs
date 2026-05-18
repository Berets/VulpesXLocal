using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.DAL.Tables.Accounting
{
    public interface IPAGCLI_LINGUARepository
    {
        PAGCLI_LINGUA? Get(string ID, string lincod);

        #region CRUD
        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }
        string DELETE_QUERY { get; }

        bool Insert(PAGCLI_LINGUA Model);

        bool InsertOrUpdate(PAGCLI_LINGUA Model);

        bool Update(PAGCLI_LINGUA Model);

        bool Delete(PAGCLI_LINGUA Model);
        #endregion
    }

    public class PAGCLI_LINGUARepository : RepositoryBase, IPAGCLI_LINGUARepository
    {
        public PAGCLI_LINGUARepository(IConnectionFactory factory) : base(factory)
        {
        }

        public PAGCLI_LINGUA? Get(string ID, string lincod)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<PAGCLI_LINGUA>(
                    "SELECT * FROM PAGCLI_LINGUA WHERE pclcod = @pclcod AND lincod = @lincod",
                    new { pclcod = ID, lincod = lincod })
                    .FirstOrDefault();

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO PAGCLI_LINGUA (pclcod,lincod,pcldes) OUTPUT INSERTED.rv VALUES(@pclcod,@lincod,@pcldes)";
        public string UPDATE_QUERY => "UPDATE PAGCLI_LINGUA SET pclcod = @pclcod, lincod = @lincod, pcldes=@pcldes OUTPUT INSERTED.rv WHERE pclcod = @pclcod AND lincod = @lincod AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM PAGCLI_LINGUA OUTPUT DELETED.rv WHERE pclcod = @pclcod AND lincod = @lincod AND rv = @rv";

        public bool Insert(PAGCLI_LINGUA Model)
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

        public bool InsertOrUpdate(PAGCLI_LINGUA Model)
        {
            try
            {
                using var connection = GetOpenConnection();


                var exist = connection.Query<PAGCLI_LINGUA>("SELECT * FROM PAGCLI_LINGUA WHERE pclcod = @pclcod AND lincod = @lincod", new { pclcod = Model.pclcod, lincod = Model.lincod }).FirstOrDefault();

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

        public bool Update(PAGCLI_LINGUA Model)
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

        public bool Delete(PAGCLI_LINGUA Model)
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

    public class PAGCLI_LINGUAUfpRepository : RepositoryBase, IPAGCLI_LINGUARepository
    {
        public PAGCLI_LINGUAUfpRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public PAGCLI_LINGUA? Get(string ID, string lincod)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<PAGCLI_LINGUA>(
                    $@"SELECT 
Lpagcod as pclcod,
lpaglin as lincod,
lpagdes as pcldes
FROM LINPAGCLI WHERE Lpagcod = @pclcod AND lpaglin = @lincod",
                    new { pclcod = ID, lincod = lincod })
                    .FirstOrDefault();

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO LINPAGCLI (Lpagcod,lpaglin,lpagdes)  VALUES(@pclcod,@lincod,@pcldes)";
        public string UPDATE_QUERY => "UPDATE LINPAGCLI SET Lpagcod=@pclcod,lpaglin=@lincod,lpagdes=@pcldes WHERE Lpagcod = @pclcod AND lpaglin = @lincod ";
        public string DELETE_QUERY => "DELETE FROM LINPAGCLI  WHERE Lpagcod = @pclcod AND lpaglin = @lincod ";

        public bool Insert(PAGCLI_LINGUA Model)
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

        public bool InsertOrUpdate(PAGCLI_LINGUA Model)
        {
            try
            {
                using var connection = GetOpenConnection();


                var exist = connection.Query<PAGCLI_LINGUA>("SELECT * FROM LINPAGCLI WHERE Lpagcod = @Lpagcod AND lpaglin = @lpaglin", new { Lpagcod = Model.pclcod, lpaglin = Model.lincod }).FirstOrDefault();

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

        public bool Update(PAGCLI_LINGUA Model)
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

        public bool Delete(PAGCLI_LINGUA Model)
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
