using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models;
using VulpesX.Shared;

namespace VulpesX.DAL._ConnectionFactory
{
    public class SqlConnectionFactory : IConnectionFactory
    {
        public IDbConnection CreateConnection()
        {
            if (string.IsNullOrEmpty(UserContext.Instance.ConnectionString))
                throw new InvalidOperationException("Connection string not set. Login required.");

            return new SqlConnection(UserContext.Instance.ConnectionString);
        }
    }
}
