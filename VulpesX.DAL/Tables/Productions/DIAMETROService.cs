using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Ufp;

namespace VulpesX.DAL.Tables.Productions
{
    public interface IDIAMETRORepository
    {
        ObservableCollection<DIAMETRO>? GetList();
    }

    public class DIAMETRORepository : RepositoryBase, IDIAMETRORepository
    {
        public DIAMETRORepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<DIAMETRO>? GetList()
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<DIAMETRO>(
                    @"SELECT * FROM DIAMETRO
                        ORDER BY Diamcod");

                return new ObservableCollection<DIAMETRO>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }
    }
}
