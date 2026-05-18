using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Ufp;

namespace VulpesX.DAL.Tables.Productions
{
    public interface IATTACCORepository
    {
        ObservableCollection<ATTACCO>? GetList();
    }

    public class ATTACCORepository : RepositoryBase, IATTACCORepository
    {
        public ATTACCORepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<ATTACCO>? GetList()
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<ATTACCO>(
                    @"SELECT * FROM ATTACCO
                        ORDER BY attacod");

                return new ObservableCollection<ATTACCO>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }
    }
}
