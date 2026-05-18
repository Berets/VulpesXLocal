using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Ufp;

namespace VulpesX.DAL.General
{
    public interface IANACERTLEVEL1Repository
    {
        ObservableCollection<ANACERTLEVEL1>? GetList(int ID);

        string INSERT_QUERY { get; }

        bool Insert(ANACERTLEVEL1 Model);
    }

    public class ANACERTLEVEL1Repository : RepositoryBase, IANACERTLEVEL1Repository
    {
        public ANACERTLEVEL1Repository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<ANACERTLEVEL1>? GetList(int ID)
        {
            throw new NotImplementedException();
        }

        public string INSERT_QUERY => string.Empty;

        public bool Insert(ANACERTLEVEL1 Model)
        {
            throw new NotImplementedException();
        }
    }

    public class ANACERTLEVEL1UfpRepository : RepositoryBase, IANACERTLEVEL1Repository
    {
        public ANACERTLEVEL1UfpRepository(IConnectionFactory factory) : base(factory)
        {

        }

        public ObservableCollection<ANACERTLEVEL1>? GetList(int ID)
        {
            try
            {
                using var connection = GetOpenConnection();

                var list = connection.Query<ANACERTLEVEL1>(
                    @$"SELECT * FROM ANACERTLEVEL1 WHERE anacli = @id", new { id = ID });

                return new ObservableCollection<ANACERTLEVEL1>(list);
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        #region CRUD
        public string INSERT_QUERY => @$"INSERT INTO ANACERTLEVEL1 (anacli,anapro,anadescer,anaper) OUTPUT INSERTED.rv VALUES(@anacli,@anapro,@anadescer,@anaper)";

        public bool Insert(ANACERTLEVEL1 Model)
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
