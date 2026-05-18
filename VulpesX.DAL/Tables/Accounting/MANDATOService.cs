using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Shared.Generics;

namespace VulpesX.DAL.Tables.Accounting
{
    public interface IMANDATORepository
    {
        ObservableCollection<MANDATO>? GetList();

        MANDATO? Get(string mancod);

        bool Exists(string mancod);

        #region CRUD
        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }
        string DELETE_QUERY { get; }
        bool Insert(MANDATO Model);

        bool Update(MANDATO Model);

        bool Delete(MANDATO Model);

        string? Validate(MANDATO Model, bool IsInsert);
        #endregion
    }

    public class MANDATORepository : RepositoryBase, IMANDATORepository
    {
        public MANDATORepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<MANDATO>? GetList()
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<MANDATO>(
                    "SELECT * FROM MANDATO");

                return new ObservableCollection<MANDATO>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public MANDATO? Get(string mancod)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<MANDATO>(
                    "SELECT * FROM MANDATO WHERE mancod = @mancod",
                    new { mancod = mancod })
                    .FirstOrDefault();

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool Exists(string mancod)
        {
            try
            {
                using var connection = GetOpenConnection();


                return (int?)connection.ExecuteScalar(
                    "SELECT COUNT(*) FROM MANDATO WHERE mancod = @mancod",
                    new { mancod = mancod }) > 0;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO MANDATO (mancod,mandes,mantip,manpag) OUTPUT INSERTED.rv VALUES(@mancod,@mandes,@mantip,@manpag)";
        public string UPDATE_QUERY => "UPDATE MANDATO SET mancod=@mancod,mandes=@mandes,mantip=@mantip,manpag=@manpag OUTPUT INSERTED.rv WHERE mancod = @mancod AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM MANDATO OUTPUT DELETED.rv WHERE mancod = @mancod AND rv = @rv";
        public bool Insert(MANDATO Model)
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

        public bool Update(MANDATO Model)
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

        public bool Delete(MANDATO Model)
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

        public string? Validate(MANDATO Model, bool IsInsert)
        {
            try
            {
                if ((!string.IsNullOrEmpty(Model.mancod) && IsInsert && !Exists(Model.mancod)) || !IsInsert)
                {
                    if (!string.IsNullOrWhiteSpace(Model.mandes))
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

    public class MANDATOUfpRepository : RepositoryBase, IMANDATORepository
    {
        public MANDATOUfpRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<MANDATO>? GetList()
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<MANDATO>(
                    "SELECT * FROM MANDATO");

                return new ObservableCollection<MANDATO>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public MANDATO? Get(string mancod)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<MANDATO>(
                    "SELECT * FROM MANDATO WHERE mancod = @mancod",
                    new { mancod = mancod })
                    .FirstOrDefault();

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool Exists(string mancod)
        {
            try
            {
                using var connection = GetOpenConnection();


                return (int?)connection.ExecuteScalar(
                    "SELECT COUNT(*) FROM MANDATO WHERE mancod = @mancod",
                    new { mancod = mancod }) > 0;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO MANDATO (mancod,mandes,mantip,manpag) OUTPUT INSERTED.rv VALUES(@mancod,@mandes,@mantip,@manpag)";
        public string UPDATE_QUERY => "UPDATE MANDATO SET mancod=@mancod,mandes=@mandes,mantip=@mantip,manpag=@manpag OUTPUT INSERTED.rv WHERE mancod = @mancod AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM MANDATO OUTPUT DELETED.rv WHERE mancod = @mancod AND rv = @rv";
        public bool Insert(MANDATO Model)
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

        public bool Update(MANDATO Model)
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

        public bool Delete(MANDATO Model)
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

        public string? Validate(MANDATO Model, bool IsInsert)
        {
            try
            {
                if ((!string.IsNullOrEmpty(Model.mancod) && IsInsert && !Exists(Model.mancod)) || !IsInsert)
                {
                    if (!string.IsNullOrWhiteSpace(Model.mandes))
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
