using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Ufp;

namespace VulpesX.DAL.Tables.Productions
{
    public interface IDENTIRepository
    {
        ObservableCollection<DENTI>? GetList();
    }

    public class DENTIRepository : RepositoryBase, IDENTIRepository
    {
        public DENTIRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<DENTI>? GetList()
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<DENTI>(
                    @"SELECT * FROM DENTI
                        ORDER BY Dencod");

                return new ObservableCollection<DENTI>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }
    }
}
