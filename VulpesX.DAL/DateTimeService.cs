using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL._ConnectionFactory;
using VulpesX.Shared.Generics;

namespace VulpesX.DAL
{
    public class DateTimeService : RepositoryBase
    {
        public DateTimeService(IConnectionFactory factory) : base(factory)
        {
        }

        public DateTime GetDatabaseServerDateTime()
        {
            try
            {
                using var connection = GetOpenConnection();

                return connection.ExecuteScalar<DateTimeOffset?>("SELECT SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time';")?.DateTime ?? DateTime.Now;
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return DateTime.Now;
            }
        }

        public ObservableCollection<GenericIntIDDescription> GetMonthsList()
        {
            return new ObservableCollection<GenericIntIDDescription>(){
            new GenericIntIDDescription() { ID = 1, Description = "Gennaio" },
            new GenericIntIDDescription() { ID = 2, Description = "Febbraio" },
            new GenericIntIDDescription() { ID = 3, Description = "Marzo" },
            new GenericIntIDDescription() { ID = 4, Description = "Aprile" },
            new GenericIntIDDescription() { ID = 5, Description = "Maggio" },
            new GenericIntIDDescription() { ID = 6, Description = "Giugno" },
            new GenericIntIDDescription() { ID = 7, Description = "Luglio" },
            new GenericIntIDDescription() { ID = 8, Description = "Agosto" },
            new GenericIntIDDescription() { ID = 9, Description = "Settembre" },
            new GenericIntIDDescription() { ID = 10, Description = "Ottobre" },
            new GenericIntIDDescription() { ID = 11, Description = "Novembre" },
            new GenericIntIDDescription() { ID = 12, Description = "Dicembre" }
        };
        }
    }
}
