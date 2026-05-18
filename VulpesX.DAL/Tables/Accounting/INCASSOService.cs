using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Ufp;
using VulpesX.Shared.Generics;

namespace VulpesX.DAL.Tables.Accounting
{
    public interface IINCASSORepository
    {
        ObservableCollection<INCASSO>? GetList();

        INCASSO? Get(string mancod);

        bool Exists(string mancod);

        #region CRUD
        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }
        string DELETE_QUERY { get; }
        bool Insert(INCASSO Model);

        bool Update(INCASSO Model);

        bool Delete(INCASSO Model);

        string? Validate(INCASSO Model, bool IsInsert);
        #endregion
    }

    public class INCASSORepository : RepositoryBase, IINCASSORepository
    {
        public INCASSORepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<INCASSO>? GetList()
        {
            throw new NotImplementedException();
        }

        public INCASSO? Get(string mancod)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string mancod)
        {
            throw new NotImplementedException();
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO INCASSO (mancod,mandes,mantip,manpag) OUTPUT INSERTED.rv VALUES(@mancod,@mandes,@mantip,@manpag)";
        public string UPDATE_QUERY => "UPDATE INCASSO SET mancod=@mancod,mandes=@mandes,mantip=@mantip,manpag=@manpag OUTPUT INSERTED.rv WHERE mancod = @mancod AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM INCASSO OUTPUT DELETED.rv WHERE mancod = @mancod AND rv = @rv";
        public bool Insert(INCASSO Model)
        {
            throw new NotImplementedException();
        }

        public bool Update(INCASSO Model)
        {
            throw new NotImplementedException();
        }

        public bool Delete(INCASSO Model)
        {
            throw new NotImplementedException();
        }

        public string? Validate(INCASSO Model, bool IsInsert)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class INCASSOUfpRepository : RepositoryBase, IINCASSORepository
    {
        public INCASSOUfpRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<INCASSO>? GetList()
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<INCASSO>(
                    "SELECT * FROM INCASSO");

                return new ObservableCollection<INCASSO>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public INCASSO? Get(string iclcod)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<INCASSO>(
                    "SELECT * FROM INCASSO WHERE iclcod = @iclcod",
                    new { iclcod = iclcod })
                    .FirstOrDefault();

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool Exists(string iclcod)
        {
            try
            {
                using var connection = GetOpenConnection();


                return (int?)connection.ExecuteScalar(
                    "SELECT COUNT(*) FROM INCASSO WHERE iclcod = @iclcod",
                    new { iclcod = iclcod }) > 0;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO INCASSO (iclcod,icldes,icltip) OUTPUT INSERTED.rv VALUES(@iclcod,@icldes,@icltip)";
        public string UPDATE_QUERY => "UPDATE INCASSO SET iclcod=@iclcod,icldes=@icldes,icltip=@icltip OUTPUT INSERTED.rv WHERE iclcod = @iclcod AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM INCASSO OUTPUT DELETED.rv WHERE iclcod = @iclcod AND rv = @rv";
        public bool Insert(INCASSO Model)
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

        public bool Update(INCASSO Model)
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

        public bool Delete(INCASSO Model)
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

        public string? Validate(INCASSO Model, bool IsInsert)
        {
            try
            {
                if ((!string.IsNullOrEmpty(Model.iclcod) && IsInsert && !Exists(Model.iclcod)) || !IsInsert)
                {
                    if (!string.IsNullOrWhiteSpace(Model.icldes))
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
