using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.DAL
{
    public abstract class RepositoryBase
    {
        protected readonly IConnectionFactory _factory;

        protected RepositoryBase(IConnectionFactory factory)
        {
            _factory = factory;
        }

        protected IDbConnection GetOpenConnection()
        {
            var connection = _factory.CreateConnection();

            if (connection == null)
                throw new Exception(Constants.CONNECTION_CREATION_ERROR);

            connection.Open();
            return connection;
        }
    }
}
