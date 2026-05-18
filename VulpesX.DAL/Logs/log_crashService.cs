using VulpesX.DAL;
using VulpesX.DAL.General;

namespace VulpesX.DAL.Logs;

public interface Ilog_crashRepository
{
    public string INSERT_QUERY { get; }
    bool Insert(log_crash Model);
}

public class log_crashRepository : RepositoryBase, Ilog_crashRepository
{
    public log_crashRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public string INSERT_QUERY => "INSERT INTO log_crash (app_domain,istant,client_time,client_name,message,stack_trace,inner_message,inner_stack_trace) VALUES(@app_domain,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@client_time,@client_name,@message,@stack_trace,@inner_message,@inner_stack_trace)";
    public bool Insert(log_crash Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Execute(INSERT_QUERY, Model);

            if (result > 0)
            {
                return true;
            }
            else
            {
                ErrorHandler.Validation(Constants.INSERT_VIOLATION);
                return false;
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }
}