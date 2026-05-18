using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Ufp;

namespace VulpesX.DAL.Tables.Productions
{
    public interface IANALOGIERepository
    {
        ObservableCollection<ANALOGIE>? GetList();
    }

    public class ANALOGIERepository : RepositoryBase, IANALOGIERepository
    {
        public ANALOGIERepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<ANALOGIE>? GetList()
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<ANALOGIE>(
                    @"SELECT * FROM ANALOGIE
                        ORDER BY angcod");

                return new ObservableCollection<ANALOGIE>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }
    }
}
