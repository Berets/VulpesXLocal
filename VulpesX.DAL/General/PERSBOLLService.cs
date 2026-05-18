using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Ufp;

namespace VulpesX.DAL.General
{
    public interface IPERSBOLLRepository
    {
        PERSBOLL? Get(string persoce);
    }

    public class PERSBOLLUfpRepository : RepositoryBase, IPERSBOLLRepository
    {
        public PERSBOLLUfpRepository(IConnectionFactory factory) : base(factory)
        {

        }

        public PERSBOLL? Get(string persoce)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<PERSBOLL>(
                    "SELECT * FROM PERSBOLL WHERE persoce = @persoce",
                    new { persoce = persoce })
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }
    }
}
