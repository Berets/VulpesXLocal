using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.DAL.General
{
    public interface IAZIENDA_LINGUARepository
    {
        AZIENDA_LINGUA? Get(string AZCode, string lincod);

        #region CRUD
        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }
        string DELETE_QUERY { get; }

        bool Insert(AZIENDA_LINGUA Model);

        bool Update(AZIENDA_LINGUA Model);

        bool Delete(AZIENDA_LINGUA Model);
        #endregion
    }

    public class AZIENDA_LINGUARepository : RepositoryBase, IAZIENDA_LINGUARepository
    {
        public AZIENDA_LINGUARepository(IConnectionFactory factory) : base(factory)
        {
        }

        public AZIENDA_LINGUA? Get(string AZCode, string lincod)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<AZIENDA_LINGUA>(
                    "SELECT * FROM AZIENDA_LINGUA WHERE AZCode = @AZCode AND lincod = @lincod",
                    new { AZCode = AZCode, lincod = lincod })
                    .FirstOrDefault();

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO AZIENDA_LINGUA (AZCode,lincod,azoffgtex,azoffogg,azofftex,azordgtex,azordogg,azordtex,azddtgtex,azddtogg,azddttex,azinvgtex,azinvogg,azinvtex,azacqgtex,azbuyogg,azbuytex) OUTPUT INSERTED.rv VALUES(@AZCode,@lincod,@azoffgtex,@azoffogg,@azofftex,@azordgtex,@azordogg,@azordtex,@azddtgtex,@azddtogg,@azddttex,@azinvgtex,@azinvogg,@azinvtex,@azacqgtex,@azbuyogg,@azbuytex)";
        public string UPDATE_QUERY => "UPDATE AZIENDA_LINGUA SET AZcode = @AZCode, lincod = @lincod,azoffgtex = @azoffgtex, azoffogg = @azoffogg,azofftex = @azofftex,azordgtex = @azordgtex,azordogg = @azordogg,azordtex = @azordtex,azddtgtex = @azddtgtex,azddtogg = @azddtogg,azddttex = @azddttex,azinvgtex = @azinvgtex,azinvogg = @azinvogg,azinvtex = @azinvtex,azacqgtex = @azacqgtex,azbuyogg = @azbuyogg,azbuytex = @azbuytex OUTPUT INSERTED.rv WHERE AZCode = @AZCode AND lincod = @lincod AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM AZIENDA_LINGUA OUTPUT DELETED.rv WHERE AZCode = @AZCode AND lincod = @lincod AND rv = @rv";

        public bool Insert(AZIENDA_LINGUA Model)
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

        public bool Update(AZIENDA_LINGUA Model)
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

        public bool Delete(AZIENDA_LINGUA Model)
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
