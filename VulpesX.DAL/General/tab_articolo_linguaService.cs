using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL._ConnectionFactory;

namespace VulpesX.DAL.General
{
    public interface Itab_articolo_linguaRepository
    {
        tab_articolo_lingua? Get(string SocietaID, string ArticoloID, string lincod);

        #region CRUD
        public string INSERT_QUERY { get; }
        public string UPDATE_QUERY { get; }
        public string DELETE_QUERY { get; }

        bool Insert(tab_articolo_lingua Model);

        bool InsertOrUpdate(tab_articolo_lingua Model);

        bool Update(tab_articolo_lingua Model);

       bool Delete(tab_articolo_lingua Model);
        #endregion
    }
    public class tab_articolo_linguaRepository : RepositoryBase, Itab_articolo_linguaRepository
    {
        public tab_articolo_linguaRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public tab_articolo_lingua? Get(string SocietaID, string ArticoloID, string lincod)
        {
            try
            {
                using var connection = GetOpenConnection();

                return connection.Query<tab_articolo_lingua>(
                        "SELECT * FROM tab_articolo_lingua WHERE SocietaID = @SocietaID AND ArticoloID=@ArticoloID AND lincod = @lincod",
                        new { SocietaID = SocietaID, ArticoloID = ArticoloID, lincod = lincod })
                        .FirstOrDefault();

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO tab_articolo_lingua (SocietaID,ArticoloID,lincod,Descrizione,Note) OUTPUT INSERTED.rv VALUES(@SocietaID, @ArticoloID,@lincod,@Descrizione,@Note)";
        public string UPDATE_QUERY => "UPDATE tab_articolo_lingua SET SocietaID = @SocietaID,ArticoloID=@ArticoloID, lincod = @lincod, Descrizione=@Descrizione, Note=@Note OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND ArticoloID=@ArticoloID AND lincod = @lincod AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM tab_articolo_lingua OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND ArticoloID=@ArticoloID AND lincod = @lincod AND rv = @rv";

        public bool Insert(tab_articolo_lingua Model)
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
                    ErrorHandler.Validation(Constants.INSERT_VIOLATION);
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public bool InsertOrUpdate(tab_articolo_lingua Model)
        {
            try
            {
                using var connection = GetOpenConnection();

                var exist = connection.Query<tab_articolo_lingua>("SELECT * FROM tab_articolo_lingua WHERE SocietaID = @SocietaID AND ArticoloID=@ArticoloID AND lincod = @lincod", new { SocietaID = Model.SocietaID, ArticoloID = Model.ArticoloID, lincod = Model.lincod }).FirstOrDefault();

                if (exist != null)
                {
                    var result = connection.Execute(UPDATE_QUERY, Model);
                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        ErrorHandler.Validation(Constants.INSERT_VIOLATION);
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
                        ErrorHandler.Validation(Constants.INSERT_VIOLATION);
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

        public bool Update(tab_articolo_lingua Model)
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
                    ErrorHandler.Validation(Constants.CONCURRENCY_VIOLATION);
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public bool Delete(tab_articolo_lingua Model)
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
                    ErrorHandler.Validation(Constants.CONCURRENCY_VIOLATION);
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
