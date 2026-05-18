using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Ufp;

namespace VulpesX.DAL.Tables.Productions
{
    public interface IFORILUBRIFICATIRepository
    {
        ObservableCollection<FORILUBRIFICATI>? GetList();
    }

    public class FORILUBRIFICATIRepository : RepositoryBase, IFORILUBRIFICATIRepository
    {
        public FORILUBRIFICATIRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<FORILUBRIFICATI>? GetList()
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<FORILUBRIFICATI>(
                    @"SELECT * FROM FORILUBRIFICATI
                        ORDER BY FLcod");

                return new ObservableCollection<FORILUBRIFICATI>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }
    }
}
