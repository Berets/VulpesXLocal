using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Ufp;

namespace VulpesX.DAL.Tables.Productions
{
    public interface IRIVESTIMENTIRepository
    {
        ObservableCollection<RIVESTIMENTI>? GetList();
    }

    public class RIVESTIMENTIRepository : RepositoryBase, IRIVESTIMENTIRepository
    {
        public RIVESTIMENTIRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<RIVESTIMENTI>? GetList()
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<RIVESTIMENTI>(
                    @"SELECT * FROM RIVESTIMENTI
                        ORDER BY rivecod");

                return new ObservableCollection<RIVESTIMENTI>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }
    }
}
