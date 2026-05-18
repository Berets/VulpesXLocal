
using Microsoft.Extensions.DependencyInjection;

namespace VulpesX.DAL.CRM;

public interface IOFFED00FRepository
{
    ObservableCollection<OFFED00F>? GetList(string CompanyID, int Year, int Number);

    OFFED00F? Get(string oftsoci, int OFTANNO, int OFTNUOR, int OFDRIGA);

    bool Exists(string oftsoci, int OFTANNO, int OFTNUOR, int OFDRIGA);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(OFFED00F Model);

    bool Update(OFFED00F Model);

    bool UpdateAll(OFFET00F Head, ObservableCollection<OFFED00F> Rows);

    bool Delete(OFFED00F Model);

    string? Validate(OFFED00F Model, bool IsInsert);

    string? ValidateModel(ObservableCollection<OFFED00F>? Rows);
    #endregion
}

public class OFFED00FRepository : RepositoryBase, IOFFED00FRepository
{
    public OFFED00FRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<OFFED00F>? GetList(string CompanyID, int Year, int Number)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<OFFED00F>(
                @"SELECT * FROM OFFED00F
                        WHERE oftsoci = @cid AND OFTANNO = @yea AND OFTNUOR = @num
                        ORDER BY OFDRIGA", new { cid = CompanyID, yea = Year, num = Number });

            return new ObservableCollection<OFFED00F>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public OFFED00F? Get(string oftsoci, int OFTANNO, int OFTNUOR, int OFDRIGA)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<OFFED00F>(
                "SELECT * FROM OFFED00F WHERE oftsoci = @oftsoci AND OFTANNO = @OFTANNO AND OFTNUOR = @OFTNUOR AND OFDRIGA = @OFDRIGA",
                new { oftsoci = oftsoci, OFTANNO = OFTANNO, OFTNUOR = OFTNUOR, OFDRIGA = OFDRIGA })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string oftsoci, int OFTANNO, int OFTNUOR, int OFDRIGA)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM OFFED00F WHERE oftsoci = @oftsoci AND OFTANNO = @OFTANNO AND OFTNUOR = @OFTNUOR AND OFDRIGA = @OFDRIGA",
                new { oftsoci = oftsoci, OFTANNO = OFTANNO, OFTNUOR = OFTNUOR, OFDRIGA = OFDRIGA }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO OFFED00F (oftsoci,OFTANNO,OFTNUOR,OFDRIGA,OFDCODA,OFDQTAV,OFDTQTA,OFDPREZ,OFDSCO1,OFDSCO2,OFDMAGG,OFDTPRE,OFDTSC1,OFDTSC2,OFDTMAG,OFDALIV,OFDASSF,OFDDACO,OFDRIFC,OFDGRUP,OFDCONT,OFDSCTO,OFDSCO3,OFDTSC3,ofdunim,ofdanf,ofdnuf,ofdrif,OFDDARI,OFDNOTE,OFDSHOW,OFDSTA,OFDCOA1,OFDCOA2,OFDCOA1P,OFDCOA2P,OFDCOA1PT,OFDCOA2PT,OFDQTAEV,transformed,transform_user) OUTPUT INSERTED.rv VALUES(@oftsoci,@OFTANNO,@OFTNUOR,@OFDRIGA,@OFDCODA,@OFDQTAV,@OFDTQTA,@OFDPREZ,@OFDSCO1,@OFDSCO2,@OFDMAGG,@OFDTPRE,@OFDTSC1,@OFDTSC2,@OFDTMAG,@OFDALIV,@OFDASSF,@OFDDACO,@OFDRIFC,@OFDGRUP,@OFDCONT,@OFDSCTO,@OFDSCO3,@OFDTSC3,@ofdunim,@ofdanf,@ofdnuf,@ofdrif,@OFDDARI,@OFDNOTE,@OFDSHOW,@OFDSTA,@OFDCOA1,@OFDCOA2,@OFDCOA1P,@OFDCOA2P,@OFDCOA1PT,@OFDCOA2PT,@OFDQTAEV,@transformed,@transform_user)";
    public string UPDATE_QUERY => "UPDATE OFFED00F SET oftsoci = @oftsoci,OFTANNO = @OFTANNO,OFTNUOR = @OFTNUOR,OFDRIGA = @OFDRIGA,OFDCODA = @OFDCODA,OFDQTAV = @OFDQTAV,OFDTQTA = @OFDTQTA,OFDPREZ = @OFDPREZ,OFDSCO1 = @OFDSCO1,OFDSCO2 = @OFDSCO2,OFDMAGG = @OFDMAGG,OFDTPRE = @OFDTPRE,OFDTSC1 = @OFDTSC1,OFDTSC2 = @OFDTSC2,OFDTMAG = @OFDTMAG,OFDALIV = @OFDALIV,OFDASSF = @OFDASSF,OFDDACO = @OFDDACO,OFDRIFC = @OFDRIFC,OFDGRUP = @OFDGRUP,OFDCONT = @OFDCONT,OFDSCTO = @OFDSCTO,OFDSCO3 = @OFDSCO3,OFDTSC3 = @OFDTSC3,ofdunim = @ofdunim,ofdanf = @ofdanf,ofdnuf = @ofdnuf,ofdrif = @ofdrif,OFDDARI = @OFDDARI,OFDNOTE = @OFDNOTE,OFDSHOW = @OFDSHOW,OFDSTA = @OFDSTA,OFDCOA1 = @OFDCOA1,OFDCOA2 = @OFDCOA2,OFDCOA1P = @OFDCOA1P,OFDCOA2P = @OFDCOA2P,OFDCOA1PT = @OFDCOA1PT,OFDCOA2PT = @OFDCOA2PT,OFDQTAEV = @OFDQTAEV,transformed = @transformed,transform_user = @transform_user OUTPUT INSERTED.rv WHERE oftsoci = @oftsoci AND OFTANNO = @OFTANNO AND OFTNUOR = @OFTNUOR AND OFDRIGA = @OFDRIGA AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM OFFED00F OUTPUT DELETED.rv WHERE oftsoci = @oftsoci AND OFTANNO = @OFTANNO AND OFTNUOR = @OFTNUOR AND OFDRIGA = @OFDRIGA AND rv = @rv";
    public bool Insert(OFFED00F Model)
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

    public bool Update(OFFED00F Model)
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

    public bool UpdateAll(OFFET00F Head, ObservableCollection<OFFED00F> Rows)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                // update head
                connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IOFFET00FRepository>().UPDATE_QUERY, Head, transaction);

                connection.Execute("DELETE FROM OFFED00F WHERE oftsoci = @oftsoci AND OFTANNO = @oftanno AND OFTNUOR = @oftnuor",
                    new { oftsoci = Head.oftsoci, oftanno = Head.OFTANNO, oftnuor = Head.OFTNUOR },
                    transaction);

                foreach (var row in Rows)
                {
                    connection.Execute(INSERT_QUERY, row, transaction);
                }

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
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

    public bool Delete(OFFED00F Model)
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

    public string? Validate(OFFED00F Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.oftsoci) && IsInsert) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.SelectedProduct?.ID))
                {
                    if (Model.OFDTQTA != "O" || (Model.OFDTQTA == "O" && Model.SelectedRate?.assomaBool == true))
                    {
                        if (Model.OFDQTAV > 0)
                        {
                            if (!Model.SelectedProduct.QuantitaDefault.HasValue ||
                                (Model.SelectedProduct.QuantitaDefault.HasValue && Model.ofdunim == Model.SelectedProduct.UnitaIDAlt && (Model.OFDQTAV % 1 == 0 ? (Model.OFDQTAV * (Model.SelectedProduct.QuantitaDefault ?? 1)) % (Model.SelectedProduct.QuantitaDefault ?? 1) == 0 : (Model.OFDQTAV % (Model.SelectedProduct.QuantitaDefault ?? 1) == 0))) ||
                                (Model.SelectedProduct.QuantitaDefault.HasValue && Model.ofdunim != Model.SelectedProduct.UnitaIDAlt && (Model.OFDQTAV % (Model.SelectedProduct.QuantitaDefault ?? 1) == 0)))
                            {
                                if (!Model.OFDQTAEV.HasValue || (Model.OFDQTAEV.HasValue && Model.OFDQTAEV.Value <= Model.QuantityValue))
                                {
                                    if (Model.OFDPREZ > 0 || (Model.OFDPREZ == 0 && Model.OFDTQTA == "N"))
                                    {
                                        if (!string.IsNullOrWhiteSpace(Model.SelectedRate?.assali) && !string.IsNullOrWhiteSpace(Model.SelectedRate?.asscod))
                                        {
                                            if (!string.IsNullOrWhiteSpace(Model.SelectedGroup?.P1GRUP) &&
                                                !string.IsNullOrWhiteSpace(Model.SelectedAccount?.P2CONT) &&
                                                !string.IsNullOrWhiteSpace(Model.SelectedSubaccount?.P3SOTC))
                                            {
                                                if (((Model.OFDSCO1.HasValue && !string.IsNullOrWhiteSpace(Model.OFDTSC1)) ||
                                                    (!Model.OFDSCO1.HasValue && string.IsNullOrWhiteSpace(Model.OFDTSC1))) &&
                                                    ((Model.OFDSCO2.HasValue && !string.IsNullOrWhiteSpace(Model.OFDTSC2)) ||
                                                    (!Model.OFDSCO2.HasValue && string.IsNullOrWhiteSpace(Model.OFDTSC2))) &&
                                                    ((Model.OFDSCO3.HasValue && !string.IsNullOrWhiteSpace(Model.OFDTSC3)) ||
                                                    (!Model.OFDSCO3.HasValue && string.IsNullOrWhiteSpace(Model.OFDTSC3))))
                                                {
                                                    return null;
                                                }
                                                else
                                                { return "Se si seleziona uno sconto e' necessario impostarne anche il tipo altrimenti ometterli entrambi"; }
                                            }
                                            else
                                            { return "Il conto contabile è obbligatorio"; }
                                        }
                                        else
                                        { return "L'aliquota è obbligatoria"; }
                                    }
                                    else
                                    { return "Il prezzo è obbligatorio"; }
                                }
                                else
                                { return "C'è già una quantità evasa, la nuova quantità non può essere inferiore a quella già evasa"; }
                            }
                            else
                            { return $"La quantità digitata ({Model.OFDQTAV.ToString("N6")}) non è valida in quanto non è un multiplo della quantità per confezione presente ({(Model.SelectedProduct.QuantitaDefault ?? 1).ToString("N6")})"; }
                        }
                        else
                        { return "La quantita' è obbligatoria"; }
                    }
                    else
                    { return "In caso di omaggio l'aliquota deve essere una di quelle abilitate agli omaggi"; }
                }
                else
                { return "L'articolo è obbligatorio"; }
            }
            else
            { return "Il codice inserito è già in uso o non è valido"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public string? ValidateModel(ObservableCollection<OFFED00F>? Rows)
    {

        if (Rows != null && Rows.Count > 0)
        {
            string? validation = null;
            foreach (var row in Rows)
            {
                validation = Validate(row, false);
                if (!string.IsNullOrWhiteSpace(validation))
                    break;
            }
            if (string.IsNullOrWhiteSpace(validation))
            {
                return null;
            }
            else
            { return validation; }
        }
        else
        {
            return "E' necessario che siano presenti delle righe per confermare l'offerta";
        }

    }
    #endregion
}