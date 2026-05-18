using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Ufp;

namespace VulpesX.DAL.General
{
    public interface ISCADCLIService
    {
        SCADCLI? Get(int CustomerID);

        bool Exists(int CustomerID);

        #region CRUD
        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }
        string DELETE_QUERY { get; }
        bool Save(SCADCLI Model);
        #endregion
    }

    public class SCADCLIUfpService : RepositoryBase, ISCADCLIService
    {
        public SCADCLIUfpService(IConnectionFactory factory) : base(factory)
        {
        }

        public SCADCLI? Get(int CustomerID)
        {
            try
            {
                using var connection = GetOpenConnection();

                return connection.Query<SCADCLI>(
                    @$"SELECT * FROM SCADCLI WHERE scclicod = @id", new { id = CustomerID }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool Exists(int CustomerID)
        {
            try
            {
                using var connection = GetOpenConnection();

                return (int?)connection.ExecuteScalar(
                    "SELECT COUNT(*) FROM SCADCLI WHERE scclicod = @id", new { id = CustomerID }) > 0;
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        #region CRUD
        public string INSERT_QUERY => @$"INSERT INTO SCADCLI (scclicod,Scagior,Scnote,descli)  OUTPUT INSERTED.rv  VALUES(@scclicod,@Scagior,@Scnote,@descli)";
        public string UPDATE_QUERY => "UPDATE SCADCLI SET scclicod = @scclicod,Scagior = @Scagior,Scnote = @Scnote,descli = @descli  OUTPUT INSERTED.rv  WHERE scclicod = @scclicod";
        public string DELETE_QUERY => "DELETE SCADCLI WHERE scclicod = @scclicod";

        public bool Save(SCADCLI Model)
        {
            try
            {
                using var connection = GetOpenConnection();

                if (Exists(Model.scclicod))
                {
                    var result = connection.ExecuteScalar(UPDATE_QUERY, Model);
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
        #endregion
    }
}
