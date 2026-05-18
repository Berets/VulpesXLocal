using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Ufp;

namespace VulpesX.DAL.Tables.Productions
{
    public interface ITIPMATPRIRepository
    {
        ObservableCollection<TIPMATPRI>? GetList();
    }

    public class TIPMATPRIRepository : RepositoryBase, ITIPMATPRIRepository
    {
        public TIPMATPRIRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<TIPMATPRI>? GetList()
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<TIPMATPRI>(
                    @"SELECT * FROM TIPMATPRI
                        ORDER BY tmpcod");

                return new ObservableCollection<TIPMATPRI>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }
    }
}
