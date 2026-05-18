using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Ufp;

namespace VulpesX.DAL.General
{
    public interface IANACERTRepository
    {
        ANACERT? Get(int ID);

        bool Exists(int ID);

        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }

        bool Insert(ANACERT Model);
    }

    public class ANACERTRepository : RepositoryBase, IANACERTRepository
    {
        public ANACERTRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ANACERT? Get(int ID)
        {
            throw new NotImplementedException();
        }

        public bool Exists(int Cliacod)
        {
            throw new NotSupportedException();
        }

        #region CRUD
        public string INSERT_QUERY => string.Empty;
        public string UPDATE_QUERY => string.Empty;

        public bool Insert(ANACERT Model)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class ANACERTUfpRepository : RepositoryBase, IANACERTRepository
    {
        public ANACERTUfpRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ANACERT? Get(int ID)
        {
            try
            {
                using var connection = GetOpenConnection();

                return connection.Query<ANACERT>(
                    @$"SELECT * FROM ANACERT WHERE anacli = @id", new { id = ID }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool Exists(int ID)
        {
            try
            {
                using var connection = GetOpenConnection();


                return (int?)connection.ExecuteScalar(
                    "SELECT COUNT(*) FROM ANACERT WHERE anacli = @id", new { id = ID }) > 0;
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        #region CRUD
        public string INSERT_QUERY => @$"INSERT INTO ANACERT (anacli,clilogoestensione,clilogonome,clilogo,ananmax,anapeobb) OUTPUT INSERTED.rv VALUES(@anacli,@clilogoestensione,@clilogonome,@clilogo,@ananmax,@anapeobb)";
        public string UPDATE_QUERY => "UPDATE ANACERT SET anacli = @anacli,clilogoestensione = @clilogoestensione,clilogonome = @clilogonome, clilogo = @clilogo,ananmax = @ananmax,anapeobb=@anapeobb   OUTPUT INSERTED.rv WHERE anacli = @anacli AND rv = @rv";

        public bool Insert(ANACERT Model)
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
        #endregion
    }
}