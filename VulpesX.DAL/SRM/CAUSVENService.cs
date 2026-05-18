using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.CRM;
using VulpesX.DAL.Store;

namespace VulpesX.DAL.SRM
{
    public interface ICAUSVENService
    {
        ObservableCollection<CAUSVEN>? GetList();

        CAUSVEN? Get(string cauven);

        bool Exists(string cauven);

        #region CRUD
        bool Insert(CAUSVEN Model);

       bool Update(CAUSVEN Model);

        bool Delete(CAUSVEN Model);

        string? Validate(DEPOSITI Model, bool IsInsert);
        #endregion
    }

    public class CAUSVENService : RepositoryBase, ICAUSVENService
    {
        public CAUSVENService(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<CAUSVEN>? GetList()
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<CAUSVEN>(
                    "SELECT * FROM CAUSVEN");

                return new ObservableCollection<CAUSVEN>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public CAUSVEN? Get(string cauven)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<CAUSVEN>(
                    "SELECT * FROM CAUSVEN WHERE cauven = @cauven",
                    new { cauven = cauven })
                    .FirstOrDefault();

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool Exists(string cauven)
        {
            try
            {
                using var connection = GetOpenConnection();

                return (int?)connection.ExecuteScalar(
                    "SELECT COUNT(*) FROM CAUSVEN WHERE cauven = @cauven",
                    new { cauven = cauven }) > 0;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        #region CRUD
        public bool Insert(CAUSVEN Model)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = connection.Execute(
                    "INSERT INTO CAUSVEN (cauven ,caudev ,cauflv , caugrv , caucov , causov , caumav , caucof ) OUTPUT INSERTED.rv VALUES(@cauven,@caudev,@cauflv,  @caugrv,  @caucov,  @causov,  @caumav,  @caucof )",
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

        public bool Update(CAUSVEN Model)
        {
            try
            {
                using var connection = GetOpenConnection();

                var result = connection.ExecuteScalar(
                    "UPDATE CAUSVEN SET cauven = @cauven,caudev = @caudev,cauflv = @cauflv, caugrv = @caugrv, caucov = @caucov, causov = @causov, caumav = @caumav, caucof = @caucof OUTPUT INSERTED.rv WHERE cauven = @cauven AND rv = @rv",
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

        public bool Delete(CAUSVEN Model)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = connection.Execute(
                    "DELETE FROM CAUSVEN OUTPUT DELETED.rv WHERE cauven = @cauven AND rv = @rv",
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

        public string? Validate(DEPOSITI Model, bool IsInsert)
        {
            try
            {
                if ((!string.IsNullOrEmpty(Model.depcod) && IsInsert && !Exists(Model.depcod)) || !IsInsert)
                {
                    if (!string.IsNullOrWhiteSpace(Model.depdes))
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
