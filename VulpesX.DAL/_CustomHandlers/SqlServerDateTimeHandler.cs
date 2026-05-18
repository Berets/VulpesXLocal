using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.DAL._CustomHandlers
{
    public class SqlServerDateTimeHandler : SqlMapper.TypeHandler<DateTime?>
    {
        private static readonly DateTime MinSqlDate = new DateTime(1753, 1, 1);

        public override void SetValue(IDbDataParameter parameter, DateTime? value)
        {
            parameter.Value = value ?? MinSqlDate;
        }

        public override DateTime? Parse(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            var date = (DateTime)value;
            return date == MinSqlDate ? null : date;
        }
    }
}
