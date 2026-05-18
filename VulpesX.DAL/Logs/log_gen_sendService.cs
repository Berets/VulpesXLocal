
namespace VulpesX.DAL.Logs;

public interface Ilog_gen_sendRepository
{
    public string INSERT_QUERY { get; }
    bool Insert(log_gen_send Model);
}

public class log_gen_sendRepository : RepositoryBase, Ilog_gen_sendRepository
{
    public log_gen_sendRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public string INSERT_QUERY => "INSERT INTO log_gen_send (company_id,istant,document_type,tag,client_name,client_time,sent_to,sent_cc,sent_object,sent_from,sent_attachments,result,sendUserID) VALUES(@company_id,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@document_type,@tag,@client_name,@client_time,@sent_to,@sent_cc,@sent_object,@sent_from,@sent_attachments,@result,@sendUserID)";
    public bool Insert(log_gen_send Model)
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