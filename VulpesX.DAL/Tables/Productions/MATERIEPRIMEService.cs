using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Ufp;

namespace VulpesX.DAL.Tables.Productions
{
    public interface IMATERIEPRIMERepository
    {
        ObservableCollection<MATERIEPRIME>? GetList();
    }

    public class MATERIEPRIMERepository : RepositoryBase, IMATERIEPRIMERepository
    {
        public MATERIEPRIMERepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<MATERIEPRIME>? GetList()
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<MATERIEPRIME>(
                    @"SELECT * FROM MATERIEPRIME
                        ORDER BY matpcod");

                return new ObservableCollection<MATERIEPRIME>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }
    }
}
