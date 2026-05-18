using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Ufp;

namespace VulpesX.DAL.Tables.Productions
{
    public interface ITIPTA00FRepository
    {
        ObservableCollection<TIPTA00F>? GetList();
    }

    public class TIPTA00FRepository : RepositoryBase, ITIPTA00FRepository
    {
        public TIPTA00FRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<TIPTA00F>? GetList()
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<TIPTA00F>(
                    @"SELECT * FROM TIPTA00F
                        ORDER BY tipcod");

                return new ObservableCollection<TIPTA00F>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }
    }
}
