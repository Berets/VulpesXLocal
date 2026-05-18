
using VulpesX.Models.Reports.Production;

namespace VulpesX.DAL.Tables.Productions;

public interface Itab_produzione_operatoreRepository
{
    ObservableCollection<tab_produzione_operatore>? GetList(string CompanyID);

    ObservableCollection<tab_produzione_operatore>? GetListFullInfo(string CompanyID);

    tab_produzione_operatore? Get(string CompanyID, string ID);

    tab_produzione_operatore? GetFromBadge(string CompanyID, string Badge);

    bool Exists(string CompanyID, string ID);

    bool ExistsBadge(string CompanyID, string Badge);

    #region CRUD
    bool Insert(tab_produzione_operatore Model, ObservableCollection<tab_produzione_operatore_costo> Costs);

    bool Update(tab_produzione_operatore Model, ObservableCollection<tab_produzione_operatore_costo> Costs);

    bool Delete(tab_produzione_operatore Model);

    string? Validate(tab_produzione_operatore Model, bool IsInsert);
    #endregion

    #region Printing
    BadgeOperatoreReport? PrintBadge(tab_produzione_operatore Model);
    #endregion
}

public class tab_produzione_operatoreRepository : RepositoryBase, Itab_produzione_operatoreRepository
{
    public tab_produzione_operatoreRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<tab_produzione_operatore>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();



            var list = connection.Query<tab_produzione_operatore>("SELECT * FROM tab_produzione_operatore WHERE SocietaID = @SocietaID", new { SocietaID = CompanyID });

            return new ObservableCollection<tab_produzione_operatore>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<tab_produzione_operatore>? GetListFullInfo(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();



            var list = connection.Query<tab_produzione_operatore>($@"select  ope.*, utt.ID as OrdineID, utt.Data, COALESCE(utt.TipoID,'0') TipoID, utt.abers1 as ClienteDescrizione, utt.RisorsaDescrizione, utt.RepartoDescrizione, utt.ArticoloDescrizione, utt.RevisioneID from tab_produzione_operatore as ope
                                                                                                                            OUTER APPLY (SELECT TOP(1) ut.TipoID ,ut.Data, ut_o.ID, ut_a.abers1, ris.Descrizione as RisorsaDescrizione, rep.Descrizione as RepartoDescrizione, art.Descrizione as ArticoloDescrizione, ut_c.RevisioneID as RevisioneID from pro_ordine_composizione_tempo as ut
			                                                                                                                inner join pro_ordine ut_o on ut_o.SocietaID = ut.SocietaID AND ut_o.ID = ut.OrdineID 
                                                                                                                            inner join pro_ordine_composizione as ut_c on ut_c.SocietaID = ut.SocietaID AND ut_c.OrdineID = ut.OrdineID AND ut_c.ArticoloID = ut.ArticoloID AND ut_c.RevisioneID = ut.RevisioneID AND ut_c.ComposizioneID = ut.ComposizioneID
                                                                                                                            inner join tab_produzione_risorsa as ris on ris.SocietaID = ut.SocietaID AND ris.ID = ut.RisorsaID
                                                                                                                            inner join tab_produzione_reparto as rep on rep.SocietaID = ut.SocietaID AND rep.ID = ut_c.RepartoID
                                                                                                                            inner join tab_articolo as art on art.SocietaID = ut_c.SocietaID AND art.ID = ut_c.ArticoloID
			                                                                                                                inner join ABE ut_a on ut_a.abecod = ut_o.ClienteID
			                                                                                                                WHERE ut.SocietaID = ope.SocietaID AND ut.OperatoreID = ope.ID order by ut.Data desc) utt
                                                                                                                            where ope.SocietaID = @SocietaID",
             new[] { typeof(tab_produzione_operatore), typeof(string), typeof(string), typeof(DateTime?), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) }
                , (objects) =>
                {
                    var operatore = (tab_produzione_operatore)objects[0];

                    operatore.UltimaComposizione = new pro_ordine_composizione
                    {
                        ArticoloID = string.Empty,
                        SocietaID = CompanyID,
                        OrdineID = (objects[1] as string) ?? string.Empty,
                        UltimoTempoData = objects[2] as DateTime?,
                        UltimoTempoTipoID = objects[3] as string,
                        ClienteDescrizione = objects[4] as string,
                        UltimoTempoOperatoreDescrizione = objects[5] as string,
                        RepartoDescrizione = objects[6] as string,
                        ArticoloDescrizione = objects[7] as string,
                        RevisioneID = (objects[8] as string) ?? string.Empty,
                    };

                    return operatore;
                },
            new { SocietaID = CompanyID },
                splitOn: "SocietaID, OrdineID,Data, TipoID, ClienteDescrizione, RisorsaDescrizione,RepartoDescrizione,ArticoloDescrizione,RevisioneID");

            return new ObservableCollection<tab_produzione_operatore>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return new ObservableCollection<tab_produzione_operatore>();
        }
    }

    public tab_produzione_operatore? Get(string CompanyID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<tab_produzione_operatore>("SELECT * FROM tab_produzione_operatore WHERE SocietaID = @SocietaID and ID = @ID",
                new { SocietaID = CompanyID, ID = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public tab_produzione_operatore? GetFromBadge(string CompanyID, string Badge)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<tab_produzione_operatore>("SELECT * FROM tab_produzione_operatore WHERE SocietaID = @SocietaID and Badge = @Badge",
                new { SocietaID = CompanyID, Badge = Badge })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string CompanyID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM tab_produzione_operatore WHERE SocietaID = @SocietaID and ID = @ID",
                new { SocietaID = CompanyID, ID = ID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public bool ExistsBadge(string CompanyID, string Badge)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM tab_produzione_operatore WHERE SocietaID = @SocietaID and Badge = @Badge",
                new { SocietaID = CompanyID, Badge = Badge }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(tab_produzione_operatore Model, ObservableCollection<tab_produzione_operatore_costo> Costs)
    {
        using var connection = GetOpenConnection();



        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                var result = connection.Execute("INSERT INTO tab_produzione_operatore (SocietaID,ID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID," +
                        "LogUpdatedUserID,LogCanceledUserID,Descrizione,Badge,UtenteID) OUTPUT INSERTED.rv VALUES(@SocietaID,@ID,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID,@Descrizione,@Badge,@UtenteID)", Model, transaction);

                //tab_produzione_reparto_risorsa
                result = connection.Execute(
                    "DELETE FROM tab_produzione_operatore_risorsa WHERE SocietaID = @SocietaID AND OperatoreID = @ID",
                    new { SocietaID = Model.SocietaID, ID = Model.ID }, transaction);

                foreach (var item in Model.OperatoreRisorse ?? new ObservableCollection<tab_produzione_risorsa>())
                {

                    result = connection.Execute(
                    "INSERT INTO tab_produzione_operatore_risorsa (SocietaID,OperatoreID,RisorsaID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID)" +
                    " OUTPUT INSERTED.rv VALUES(@SocietaID,@OperatoreID,@RisorsaID,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID)",
                    new tab_produzione_operatore_risorsa { SocietaID = Model.SocietaID, OperatoreID = Model.ID, RisorsaID = item.ID, LogAdded = DateTime.Now }, transaction);
                }

                //tab_produzione_operatore_costo
                connection.Execute(@"DELETE FROM tab_produzione_operatore_costo WHERE SocietaID = @cid AND OperatoreID = @rid",
                    new { cid = Model.SocietaID, rid = Model.ID },
                    transaction);
                if (Costs != null && Costs.Count > 0)
                {
                    foreach (var item in Costs)
                    {
                        connection.Execute("INSERT INTO tab_produzione_operatore_costo (SocietaID,OperatoreID,Periodo,CostoOrario) OUTPUT INSERTED.rv VALUES(@SocietaID,@OperatoreID,@Periodo,@CostoOrario)",
                            item, transaction);
                    }
                }

                if (result > 0)
                {
                    transaction.Commit();
                    return true;
                }
                else
                {
                    transaction.Rollback();
                    ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                    return false;
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

    }

    public bool Update(tab_produzione_operatore Model, ObservableCollection<tab_produzione_operatore_costo> Costs)
    {
        using var connection = GetOpenConnection();



        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                var result = connection.ExecuteScalar(
                    "UPDATE tab_produzione_operatore SET LogAdded = @LogAdded,LogUpdated = @LogUpdated,LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID,LogCanceledUserID = @LogCanceledUserID,Descrizione = @Descrizione,Badge = @Badge,UtenteID = @UtenteID OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND rv = @rv",
                    Model, transaction);

                //pulisce tab_produzione_reparto_risorsa
                result = connection.Execute(
                    "DELETE FROM tab_produzione_operatore_risorsa WHERE SocietaID = @SocietaID AND OperatoreID = @ID",
                    new { SocietaID = Model.SocietaID, ID = Model.ID }, transaction);

                foreach (var item in Model.OperatoreRisorse ?? new ObservableCollection<tab_produzione_risorsa>())
                {

                    result = connection.Execute(
                    "INSERT INTO tab_produzione_operatore_risorsa (SocietaID,OperatoreID,RisorsaID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID)" +
                    " OUTPUT INSERTED.rv VALUES(@SocietaID,@OperatoreID,@RisorsaID,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID)",
                    new tab_produzione_operatore_risorsa { SocietaID = Model.SocietaID, OperatoreID = Model.ID, RisorsaID = item.ID, LogAdded = DateTime.Now }, transaction);
                }

                //tab_produzione_operatore_costo
                connection.Execute(@"DELETE FROM tab_produzione_operatore_costo WHERE SocietaID = @cid AND OperatoreID = @rid",
                    new { cid = Model.SocietaID, rid = Model.ID },
                    transaction);
                if (Costs != null && Costs.Count > 0)
                {
                    foreach (var item in Costs)
                    {
                        connection.Execute("INSERT INTO tab_produzione_operatore_costo (SocietaID,OperatoreID,Periodo,CostoOrario) OUTPUT INSERTED.rv VALUES(@SocietaID,@OperatoreID,@Periodo,@CostoOrario)",
                            item, transaction);
                    }
                }

                if (result != null)
                {
                    transaction.Commit();
                    return true;
                }
                else
                {
                    transaction.Rollback();
                    ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                    return false;
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

    }

    public bool Delete(tab_produzione_operatore Model)
    {
        try
        {
            using var connection = GetOpenConnection();



            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var result = connection.ExecuteScalar(
                         "DELETE FROM tab_produzione_operatore OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND ID = @ID AND rv = @rv",
                        Model, transaction);

                    //pulisce tab_produzione_reparto_risorsa
                    result = connection.Execute(
                        "DELETE FROM tab_produzione_operatore_risorsa WHERE SocietaID = @SocietaID AND OperatoreID = @ID",
                        new { SocietaID = Model.SocietaID, ID = Model.ID }, transaction);

                    //tab_produzione_operatore_costo
                    connection.Execute(@"DELETE FROM tab_produzione_operatore_costo WHERE SocietaID = @cid AND OperatoreID = @rid",
                        new { cid = Model.SocietaID, rid = Model.ID },
                        transaction);

                    if (result != null)
                    {
                        transaction.Commit();
                        return true;
                    }
                    else
                    {
                        transaction.Rollback();
                        ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(ex.Message);
                    return false;
                }
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public string? Validate(tab_produzione_operatore Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Model.ID) && IsInsert && !Exists(Model.SocietaID, Model.ID) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.Descrizione) && Model.Descrizione.Length <= 255)
                {
                    return null;
                }
                else
                { return "La descrizione è obbligatoria e può contenere al massimo 255 caratteri"; }
            }
            else
            { return "Il codice inserito è già in uso o non è valido"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion

    #region Printing
    public BadgeOperatoreReport? PrintBadge(tab_produzione_operatore Model)
    {
        try
        {
            return new BadgeOperatoreReport()
            {
                Operatore = Model,
            };
        }
        catch (Exception exc)
        {
            ErrorHandler.Show(exc.Message);
            return null;
        }
    }
    #endregion
}