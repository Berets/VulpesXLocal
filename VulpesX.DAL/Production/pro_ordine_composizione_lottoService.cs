
namespace VulpesX.DAL.Production;

public interface Ipro_ordine_composizione_lottoRepository
{
    ObservableCollection<pro_ordine_composizione_lotto>? GetList();

    pro_ordine_composizione_lotto? Get(string SocietaID, string OrdineID, string ArticoloID, string RevisioneID, long ComposizioneID, long ID);

    bool Exists(string SocietaID, string OrdineID, string ArticoloID, string RevisioneID, long ComposizioneID, long ID);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(pro_ordine_composizione_lotto Model);

    bool Update(pro_ordine_composizione_lotto Model);

    bool Delete(pro_ordine_composizione_lotto Model);
    #endregion
}

public class pro_ordine_composizione_lottoRepository : RepositoryBase, Ipro_ordine_composizione_lottoRepository
{
    public pro_ordine_composizione_lottoRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<pro_ordine_composizione_lotto>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<pro_ordine_composizione_lotto>(
                "SELECT * FROM pro_ordine_composizione_lotto");

            return new ObservableCollection<pro_ordine_composizione_lotto>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public pro_ordine_composizione_lotto? Get(string SocietaID, string OrdineID, string ArticoloID, string RevisioneID, long ComposizioneID, long ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<pro_ordine_composizione_lotto>(
                "SELECT * FROM pro_ordine_composizione_lotto WHERE SocietaID = @SocietaID AND OrdineID = @OrdineID AND ArticoloID = @ArticoloID AND RevisioneID = @RevisioneID AND ComposizioneID = @ComposizioneID AND ID = @ID",
                new { SocietaID = SocietaID, OrdineID = OrdineID, ArticoloID = ArticoloID, RevisioneID = RevisioneID, ComposizioneID = ComposizioneID, ID = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string SocietaID, string OrdineID, string ArticoloID, string RevisioneID, long ComposizioneID, long ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM pro_ordine_composizione_lotto WHERE SocietaID = @SocietaID AND OrdineID = @OrdineID AND ArticoloID = @ArticoloID AND RevisioneID = @RevisioneID AND ComposizioneID = @ComposizioneID AND ID = @ID",
                new { SocietaID = SocietaID, OrdineID = OrdineID, ArticoloID = ArticoloID, RevisioneID = RevisioneID, ComposizioneID = ComposizioneID, ID = ID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO pro_ordine_composizione_lotto (SocietaID,OrdineID,ArticoloID,RevisioneID,ComposizioneID,ID,Lotto,Quantita,DataScadenza,LogAdded,LogAddedUserID,product_id) OUTPUT INSERTED.rv VALUES(@SocietaID,@OrdineID,@ArticoloID,@RevisioneID,@ComposizioneID,@ID,@Lotto,@Quantita,@DataScadenza,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@LogAddedUserID,@product_id)";
    public string UPDATE_QUERY => "UPDATE pro_ordine_composizione_lotto SET SocietaID = @SocietaID,OrdineID = @OrdineID,ArticoloID = @ArticoloID,RevisioneID = @RevisioneID,ComposizioneID = @ComposizioneID,ID = @ID,Lotto = @Lotto,Quantita = @Quantita,DataScadenza = @DataScadenza,LogAdded = @LogAdded,LogAddedUserID = @LogAddedUserID,product_id = @product_id OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND OrdineID = @OrdineID AND ArticoloID = @ArticoloID AND RevisioneID = @RevisioneID AND ComposizioneID = @ComposizioneID AND ID = @ID AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM pro_ordine_composizione_lotto OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND OrdineID = @OrdineID AND ArticoloID = @ArticoloID AND RevisioneID = @RevisioneID AND ComposizioneID = @ComposizioneID AND ID = @ID AND rv = @rv";
    public bool Insert(pro_ordine_composizione_lotto Model)
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
                ErrorHandler.Show(Constants.INSERT_VIOLATION);
                return false;
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Update(pro_ordine_composizione_lotto Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(UPDATE_QUERY, Model);
            if (result != null)
            {
                return true;
            }
            else
            {
                ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                return false;
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Delete(pro_ordine_composizione_lotto Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(DELETE_QUERY, Model);
            if (result > 0)
            {
                return true;
            }
            else
            {
                ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                return false;
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }
    #endregion
}