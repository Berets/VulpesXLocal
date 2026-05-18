namespace VulpesX.DAL.Tables.Accounting;

public interface IAliquoteRepository
{

    ObservableCollection<ASSOGGETAMENTI>? GetList();

    ASSOGGETAMENTI? Get(string Assoggettamento, string Aliquota);

    ASSOGGETAMENTI? GetFirstAliquota(string Aliquota);

    ASSOGGETAMENTI? GetFirstByNature(string Nature);

    bool Exists(string Assoggettamento, string Aliquota);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ASSOGGETAMENTI Model);

    bool Update(ASSOGGETAMENTI Model);

    bool Delete(ASSOGGETAMENTI Model);

    string? Validate(ASSOGGETAMENTI Model, bool IsInsert);
    #endregion
}

public class AliquoteRepository : RepositoryBase, IAliquoteRepository
{
    public AliquoteRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ASSOGGETAMENTI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ASSOGGETAMENTI>(
                @"SELECT a.*, n.FETICod + ' ' + FETIDes AS NaturaFullDescriptionSearchable FROM ASSOGGETAMENTI AS a
                        LEFT JOIN FE_IVADOC AS n ON n.FETICod = a.assnatufe");

            return new ObservableCollection<ASSOGGETAMENTI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ASSOGGETAMENTI? Get(string Assoggettamento, string Aliquota)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ASSOGGETAMENTI>(
                "SELECT * FROM ASSOGGETAMENTI WHERE asscod = @asscod AND assali = @assali",
                new { asscod = Assoggettamento, assali = Aliquota })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ASSOGGETAMENTI? GetFirstAliquota(string Aliquota)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ASSOGGETAMENTI>(
                @"SELECT * FROM ASSOGGETAMENTI 
                        WHERE assali = @assali
                        ORDER BY asscod",
                new { assali = Aliquota })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    public ASSOGGETAMENTI? GetFirstByNature(string Nature)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ASSOGGETAMENTI>(
                @"SELECT * FROM ASSOGGETAMENTI 
                        WHERE assnatufe = @nature
                        ORDER BY asscod",
                new { nature = Nature })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string Assoggettamento, string Aliquota)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ASSOGGETAMENTI WHERE asscod = @asscod AND assali = @assali",
                new { asscod = Assoggettamento, assali = Aliquota }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ASSOGGETAMENTI (asscod,assali,assdes,asscli,assfor,asspin,assoma,asspla,asstif,asstic,asself,asselc,asstipiva,asscomiva,asssplpay,assnatufe,assomacod,assomaali,assventcod,assventali) OUTPUT INSERTED.rv VALUES(@asscod,@assali,@assdes,@asscli,@assfor,@asspin,@assoma,@asspla,@asstif,@asstic,@asself,@asselc,@asstipiva,@asscomiva,@asssplpay,@assnatufe,@assomacod,@assomaali,@assventcod,@assventali)";
    public string UPDATE_QUERY => "UPDATE ASSOGGETAMENTI SET asscod = @asscod,assali = @assali,assdes = @assdes,asscli = @asscli,assfor = @assfor,asspin = @asspin,assoma = @assoma,asspla = @asspla,asstif = @asstif,asstic = @asstic,asself = @asself,asselc = @asselc,asstipiva = @asstipiva,asscomiva = @asscomiva,asssplpay = @asssplpay,assnatufe = @assnatufe,assomacod = @assomacod,assomaali = @assomaali,assventcod = @assventcod,assventali = @assventali OUTPUT INSERTED.rv WHERE asscod = @asscod AND assali = @assali AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ASSOGGETAMENTI OUTPUT DELETED.rv WHERE asscod = @asscod AND assali = @assali AND rv = @rv";
    public bool Insert(ASSOGGETAMENTI Model)
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

    public bool Update(ASSOGGETAMENTI Model)
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

    public bool Delete(ASSOGGETAMENTI Model)
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

    public string? Validate(ASSOGGETAMENTI Model, bool IsInsert)
    {
        try
        {
            if ((!Model.assomaBool && Model.SelectedRate == null) || (Model.assomaBool && Model.SelectedRate != null))
            {
                if ((!string.IsNullOrEmpty(Model.asscod) && !string.IsNullOrEmpty(Model.assali) && IsInsert && !Exists(Model.asscod, Model.assali)) || !IsInsert)
                {
                    if (Model.asscod.Length == 1 && Model.assali.Length > 0 && Model.assali.Length <= 2)
                    {
                        if (!string.IsNullOrWhiteSpace(Model.assdes))
                        {
                            if (Model.asspin >= 0 && Model.asspin <= 100)
                            {
                                return null;
                            }
                            else
                            { return "La percentuale di indetraibilità deve essere compresa tra 0 e 100"; }
                        }
                        else
                        { return "La descrizione è obbligatoria"; }
                    }
                    else
                    { return "L'assoggettamento può contenere massimo 1 carattere, l'aliquota 2"; }
                }
                else
                { return "I codici inseriti sono già in uso o non sono validi"; }
            }
            else
            { return "Se l'aliquota è abilitata agli omaggi è necessario selezionare un'aliquota per la contabilizzazione, altrimenti l'aliquota non è necessaria"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}