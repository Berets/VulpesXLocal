namespace VulpesX.DAL.Treasury
{
    public interface IRBCC01F0Repository
    {
        RBCC01F0? GetByPDC(string CompanyID, string GroupID, string AccountID, string SubaccountID);

        decimal GetTotaleDisponibilita(string CompanyID);

        #region CRUD
        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }
        string DELETE_QUERY { get; }
        bool Insert(RBCC01F0 Model);

        bool Update(RBCC01F0 Model);

        bool Delete(RBCC01F0 Model);

        string? Validate(RBCC01F0 Model, bool IsInsert);
        #endregion
    }

    public class RBCC01F0Repository : RepositoryBase, IRBCC01F0Repository
    {
        public RBCC01F0Repository(IConnectionFactory factory) : base(factory)
        {
        }

        public RBCC01F0? GetByPDC(string CompanyID, string GroupID, string AccountID, string SubaccountID)
        {
            try
            {
                using var connection = GetOpenConnection();

                return connection.Query<RBCC01F0>(
                    "SELECT * FROM RBCC01F0 WHERE cnsl34 = @cid AND cnsl30 = @group AND cnsl31 = @account AND cnsl32 = @subaccount",
                    new { cid = CompanyID, group = GroupID, account = AccountID, subaccount = SubaccountID })
                    .FirstOrDefault();

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public decimal GetTotaleDisponibilita(string CompanyID)
        {
            try
            {
                using var connection = GetOpenConnection();

                    var result = connection.Query<dynamic>(
                        "SELECT SUM(cnsl13) AS Liquid1, SUM(cnsl15) AS Liquid2, SUM(cnsl17) AS Intern FROM RBCC01F0 WHERE cnsl34 = @cid",
                        new { cid = CompanyID })
                        .FirstOrDefault();

                    return (result != null && result?.Liquid1 != null) ? (result?.Liquid1 > 0 ? result?.Liquid1 + result?.Intern : result?.Liquid1 - result?.Intern) : 0;
              
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return -1;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO RBCC01F0 (cnsl34,cnsl01,cnsl02,cnsl05,cnsl03,cnsl04,cnsl06,cnsl08c,cnsl08,cnsl10,cnsl11,cnsl12,cnsl13,cnsl14,cnsl15,cnsl16,cnsl17,cnsl18,cnsl19,cnsl20,cnsl21,cnsl22,cnsl23,cnsl24,cnsl25,cnsl29,cnsl30,cnsl31,cnsl32,cnsl33,cnsl35,cnsl36,cnsl37,cnsl44,cnslnewcab,cnslnewabi) OUTPUT INSERTED.rv VALUES(@cnsl34,@cnsl01,@cnsl02,@cnsl05,@cnsl03,@cnsl04,@cnsl06,@cnsl08c,@cnsl08,@cnsl10,@cnsl11,@cnsl12,@cnsl13,@cnsl14,@cnsl15,@cnsl16,@cnsl17,@cnsl18,@cnsl19,@cnsl20,@cnsl21,@cnsl22,@cnsl23,@cnsl24,@cnsl25,@cnsl29,@cnsl30,@cnsl31,@cnsl32,@cnsl33,@cnsl35,@cnsl36,@cnsl37,@cnsl44,@cnslnewcab,@cnslnewabi)";
        public string UPDATE_QUERY => "UPDATE RBCC01F0 SET cnsl34 = @cnsl34,cnsl01 = @cnsl01,cnsl02 = @cnsl02,cnsl05 = @cnsl05,cnsl03 = @cnsl03,cnsl04 = @cnsl04,cnsl06 = @cnsl06,cnsl08c = @cnsl08c,cnsl08 = @cnsl08,cnsl10 = @cnsl10,cnsl11 = @cnsl11,cnsl12 = @cnsl12,cnsl13 = @cnsl13,cnsl14 = @cnsl14,cnsl15 = @cnsl15,cnsl16 = @cnsl16,cnsl17 = @cnsl17,cnsl18 = @cnsl18,cnsl19 = @cnsl19,cnsl20 = @cnsl20,cnsl21 = @cnsl21,cnsl22 = @cnsl22,cnsl23 = @cnsl23,cnsl24 = @cnsl24,cnsl25 = @cnsl25,cnsl29 = @cnsl29,cnsl30 = @cnsl30,cnsl31 = @cnsl31,cnsl32 = @cnsl32,cnsl33 = @cnsl33,cnsl35 = @cnsl35,cnsl36 = @cnsl36,cnsl37 = @cnsl37,cnsl44 = @cnsl44,cnslnewcab = @cnslnewcab,cnslnewabi = @cnslnewabi OUTPUT INSERTED.rv WHERE cnsl34 = @cnsl34 AND cnsl01 = @cnsl01 AND cnsl02 = @cnsl02 AND cnsl05 = @cnsl05 AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM RBCC01F0 OUTPUT DELETED.rv WHERE cnsl34 = @cnsl34 AND cnsl01 = @cnsl01 AND cnsl02 = @cnsl02 AND cnsl05 = @cnsl05 AND rv = @rv";
        public bool Insert(RBCC01F0 Model)
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

        public bool Update(RBCC01F0 Model)
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

        public bool Delete(RBCC01F0 Model)
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

        public string? Validate(RBCC01F0 Model, bool IsInsert)
        {
            try
            {
                if (!string.IsNullOrEmpty(Model.cnsl34))
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
