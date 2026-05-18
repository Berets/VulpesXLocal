using VulpesX.DAL;

namespace VulpesX.DAL.Auth
{
    public interface ICompanyRepository
    {
        ObservableCollection<SOCBASE>? GetList(bool AddAllItem);

        ObservableCollection<SOCBASE>? GetDescriptionsList(List<string> IDs);

        SOCBASE? Get(string ID);

        bool Exists(string SOMCOD);

        #region CRUD

        bool Insert(SOCBASE Model);

        bool Update(SOCBASE Model);

        bool Delete(SOCBASE Model);

        string? Validate(SOCBASE Model, bool IsInsert);
        #endregion
    }

    public class CompanyRepository : RepositoryBase, ICompanyRepository
    {
        public CompanyRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<SOCBASE>? GetList(bool AddAllItem)
        {
            try
            {
                using var connection = GetOpenConnection();

                var list = connection.Query<SOCBASE>(
                    "SELECT * FROM SOCBASE").ToList();

                if (AddAllItem)
                {
                    list.Add(new SOCBASE()
                    {
                        SOMCOD = string.Empty,
                        SOMDES = "Tutte le società"
                    });
                }

                return new ObservableCollection<SOCBASE>(list);
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<SOCBASE>? GetDescriptionsList(List<string> IDs)
        {
            try
            {
                using var connection = GetOpenConnection();

                var list = connection.Query<SOCBASE>(
                    "SELECT SOMCOD, SOMDES, SOMF02, SOCUID FROM SOCBASE WHERE SOMCOD IN @ids",
                    new { ids = IDs });

                return new ObservableCollection<SOCBASE>(list);
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public SOCBASE? Get(string ID)
        {
            try
            {
                using var connection = GetOpenConnection();

                var company = connection.Query<SOCBASE>(
                    "SELECT * FROM SOCBASE where SOMCOD = @ID", new { ID = ID }).FirstOrDefault();

                return company;
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool Exists(string SOMCOD)
        {
            try
            {
                using var connection = GetOpenConnection();

                return (int?)connection.ExecuteScalar(
                    "SELECT COUNT(*) FROM SOCBASE WHERE SOMCOD = @SOMCOD",
                    new { SOMCOD = SOMCOD }) > 0;
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        #region CRUD

        public bool Insert(SOCBASE Model)
        {
            try
            {
                using var connection = GetOpenConnection();

                var result = connection.Execute(
                    "INSERT INTO SOCBASE (SOMCOD,SOMDES,SOMF01,SOMF02,SOMF05,SOCUID) OUTPUT INSERTED.rv VALUES(@SOMCOD,@SOMDES,@SOMF01,@SOMF02,@SOMF05,@SOCUID)",
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
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public bool Update(SOCBASE Model)
        {
            try
            {
                using var connection = GetOpenConnection();

                var result = connection.ExecuteScalar(
                    "UPDATE SOCBASE SET SOMCOD = @SOMCOD,SOMDES = @SOMDES,SOMF01 = @SOMF01,SOMF02 = @SOMF02,SOMF05 = @SOMF05,SOCUID = @SOCUID OUTPUT INSERTED.rv WHERE SOMCOD = @SOMCOD AND rv = @rv",
                    Model);
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

        public bool Delete(SOCBASE Model)
        {
            try
            {
                using var connection = GetOpenConnection();

                var result = connection.Execute(
                    "DELETE FROM SOCBASE OUTPUT DELETED.rv WHERE SOMCOD = @SOMCOD AND rv = @rv",
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

        public string? Validate(SOCBASE Model, bool IsInsert)
        {
            try
            {
                if ((!string.IsNullOrEmpty(Model.SOMCOD) && IsInsert && !Exists(Model.SOMCOD)) || !IsInsert)
                {
                    if (!string.IsNullOrWhiteSpace(Model.SOMDES))
                    {
                        return null;
                    }
                    else
                    { return "La descrizione è obbligatoria"; }
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

    public class CompanyUfpRepository : RepositoryBase, ICompanyRepository
    {
        public CompanyUfpRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<SOCBASE>? GetList(bool AddAllItem)
        {
            try
            {
                using var connection = GetOpenConnection();

                var list = connection.Query<SOCBASE>(
                    "SELECT * FROM SOCI_BASE").ToList();

                if (AddAllItem)
                {
                    list.Add(new SOCBASE()
                    {
                        SOMCOD = string.Empty,
                        SOMDES = "Tutte le società"
                    });
                }

                return new ObservableCollection<SOCBASE>(list);
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<SOCBASE>? GetDescriptionsList(List<string> IDs)
        {
            try
            {
                using var connection = GetOpenConnection();

                var list = connection.Query<SOCBASE>(
                    "SELECT SOMCOD, SOMDES, SOMF02, SOCUID FROM SOCI_BASE WHERE SOMCOD IN @ids",
                    new { ids = IDs });

                return new ObservableCollection<SOCBASE>(list);
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public SOCBASE? Get(string ID)
        {
            try
            {
                using var connection = GetOpenConnection();

                var company = connection.Query<SOCBASE>(
                    "SELECT * FROM SOCI_BASE where SOMCOD = @ID", new { ID = ID }).FirstOrDefault();

                return company;
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool Exists(string SOMCOD)
        {
            try
            {
                using var connection = GetOpenConnection();

                return (int?)connection.ExecuteScalar(
                    "SELECT COUNT(*) FROM SOCI_BASE WHERE SOMCOD = @SOMCOD",
                    new { SOMCOD = SOMCOD }) > 0;
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        #region CRUD

        public bool Insert(SOCBASE Model)
        {
            try
            {
                using var connection = GetOpenConnection();

                var result = connection.Execute(
                    "INSERT INTO SOCI_BASE (SOMCOD,SOMDES,SOMF01,SOMF02,SOMF05,SOCUID) OUTPUT INSERTED.rv VALUES(@SOMCOD,@SOMDES,@SOMF01,@SOMF02,@SOMF05,@SOCUID)",
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
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public bool Update(SOCBASE Model)
        {
            try
            {
                using var connection = GetOpenConnection();

                var result = connection.ExecuteScalar(
                    "UPDATE SOCI_BASE SET SOMCOD = @SOMCOD,SOMDES = @SOMDES,SOMF01 = @SOMF01,SOMF02 = @SOMF02,SOMF05 = @SOMF05,SOCUID = @SOCUID OUTPUT INSERTED.rv WHERE SOMCOD = @SOMCOD AND rv = @rv",
                    Model);
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

        public bool Delete(SOCBASE Model)
        {
            try
            {
                using var connection = GetOpenConnection();

                var result = connection.Execute(
                    "DELETE FROM SOCI_BASE OUTPUT DELETED.rv WHERE SOMCOD = @SOMCOD AND rv = @rv",
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

        public string? Validate(SOCBASE Model, bool IsInsert)
        {
            try
            {
                if ((!string.IsNullOrEmpty(Model.SOMCOD) && IsInsert && !Exists(Model.SOMCOD)) || !IsInsert)
                {
                    if (!string.IsNullOrWhiteSpace(Model.SOMDES))
                    {
                        return null;
                    }
                    else
                    { return "La descrizione è obbligatoria"; }
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
