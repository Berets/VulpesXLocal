using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Ufp;

namespace VulpesX.DAL.Tables.Productions
{
    public interface ILDRepository
    {
        ObservableCollection<LD>? GetList();
    }

    public class LDRepository : RepositoryBase, ILDRepository
    {
        public LDRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<LD>? GetList()
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<LD>(
                    @"SELECT * FROM LD
                        ORDER BY Ldcod");

                return new ObservableCollection<LD>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }
    }
}
