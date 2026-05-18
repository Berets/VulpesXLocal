using VulpesX.DAL;

namespace VulpesX.Services.Logs;

public interface Ilog_srm_sendRepository
{
    public string INSERT_QUERY { get; }
    bool Insert(log_srm_send Model);
}

public class log_srm_sendRepository : RepositoryBase, Ilog_srm_sendRepository
{
    public log_srm_sendRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public string INSERT_QUERY => "INSERT INTO log_srm_send (company_id,istant,document_type,document_number,client_name,client_time,sent_to,sent_cc,sent_object,sent_from,sent_attachments,result,sendUserID) VALUES(@company_id,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@document_type,@document_number,@client_name,@client_time,@sent_to,@sent_cc,@sent_object,@sent_from,@sent_attachments,@result,@sendUserID)";
    public bool Insert(log_srm_send Model)
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
