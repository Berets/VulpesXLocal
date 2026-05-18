namespace VulpesX.DAL.Tables.Productions;

public interface Itab_produzione_risorsaRepository
{
    ObservableCollection<tab_produzione_risorsa>? GetList(string CompanyID);

    ObservableCollection<tab_produzione_risorsa>? GetListFullInfo(string CompanyID);

    ObservableCollection<tab_produzione_risorsa>? GetListFromReparto(string CompanyID, string RepartoID);

    ObservableCollection<tab_produzione_risorsa>? GetListFromOperatore(string CompanyID, string OperatoreID);

    ObservableCollection<tab_produzione_risorsa>? GetListFromCausale(string CompanyID, string CausaleID);

    tab_produzione_risorsa? Get(string CompanyID, string ID);

    bool Exists(string CompanyID, string ID);

    #region CRUD
    bool Insert(tab_produzione_risorsa Model, ObservableCollection<tab_produzione_risorsa_sorgenti> Sources, ObservableCollection<tab_produzione_risorsa_costo> Costs);

    bool Update(tab_produzione_risorsa Model, ObservableCollection<tab_produzione_risorsa_sorgenti> Sources, ObservableCollection<tab_produzione_risorsa_costo> Costs);

    bool Delete(tab_produzione_risorsa Model);

    string? Validate(tab_produzione_risorsa Model, bool IsInsert);
    #endregion
}

public class tab_produzione_risorsaRepository : RepositoryBase, Itab_produzione_risorsaRepository
{
    public tab_produzione_risorsaRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<tab_produzione_risorsa>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<tab_produzione_risorsa>(
                "SELECT * FROM tab_produzione_risorsa  WHERE SocietaID = @SocietaID", new { SocietaID = CompanyID });

            return new ObservableCollection<tab_produzione_risorsa>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<tab_produzione_risorsa>? GetListFullInfo(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();



            var list = connection.Query<tab_produzione_risorsa>($@"select  ris.*, utt.ID as OrdineID, utt.Data, COALESCE(utt.TipoID,'0') TipoID, utt.abers1 as ClienteDescrizione, utt.OperatoreDescrizione, utt.RepartoDescrizione, utt.ArticoloDescrizione, utt.RevisioneID from tab_produzione_risorsa as ris
                                                                                                                            OUTER APPLY (SELECT TOP(1) ut.TipoID ,ut.Data, ut_o.ID, ut_a.abers1, ope.Descrizione as OperatoreDescrizione, rep.Descrizione as RepartoDescrizione, art.Descrizione as ArticoloDescrizione, ut_c.RevisioneID as RevisioneID from pro_ordine_composizione_tempo as ut
			                                                                                                                inner join pro_ordine ut_o on ut_o.SocietaID = ut.SocietaID AND ut_o.ID = ut.OrdineID 
                                                                                                                            inner join pro_ordine_composizione as ut_c on ut_c.SocietaID = ut.SocietaID AND ut_c.OrdineID = ut.OrdineID AND ut_c.ArticoloID = ut.ArticoloID AND ut_c.RevisioneID = ut.RevisioneID AND ut_c.ComposizioneID = ut.ComposizioneID
                                                                                                                            inner join tab_produzione_operatore as ope on ope.SocietaID = ut.SocietaID AND ope.ID = ut.OperatoreID
                                                                                                                            inner join tab_produzione_reparto as rep on rep.SocietaID = ut.SocietaID AND rep.ID = ut_c.RepartoID
                                                                                                                            inner join tab_articolo as art on art.SocietaID = ut_c.SocietaID AND art.ID = ut_c.ArticoloID
			                                                                                                                inner join ABE ut_a on ut_a.abecod = ut_o.ClienteID
			                                                                                                                WHERE ut.SocietaID = ris.SocietaID AND ut.RisorsaID = ris.ID order by ut.Data desc) utt
                                                                                                                            where ris.SocietaID = @SocietaID",
             new[] { typeof(tab_produzione_risorsa), typeof(string), typeof(string), typeof(DateTime?), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) }
                , (objects) =>
                {
                    var risorsa = (tab_produzione_risorsa)objects[0];

                    risorsa.UltimaComposizione = new pro_ordine_composizione
                    {
                        SocietaID = CompanyID,
                        ArticoloID = string.Empty,
                        OrdineID = (objects[1] as string) ?? string.Empty,
                        UltimoTempoData = objects[2] as DateTime?,
                        UltimoTempoTipoID = objects[3] as string,
                        ClienteDescrizione = objects[4] as string,
                        UltimoTempoOperatoreDescrizione = objects[5] as string,
                        RepartoDescrizione = objects[6] as string,
                        ArticoloDescrizione = objects[7] as string,
                        RevisioneID = (objects[8] as string) ?? string.Empty,
                    };

                    return risorsa;
                },
            new { SocietaID = CompanyID },
                splitOn: "SocietaID, OrdineID,Data, TipoID, ClienteDescrizione, OperatoreDescrizione,RepartoDescrizione,ArticoloDescrizione,RevisioneID");

            return new ObservableCollection<tab_produzione_risorsa>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return new ObservableCollection<tab_produzione_risorsa>();
        }
    }

    public ObservableCollection<tab_produzione_risorsa>? GetListFromReparto(string CompanyID, string RepartoID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<tab_produzione_risorsa>(
                @"SELECT r.* FROM tab_produzione_reparto_risorsa AS m
                        INNER JOIN tab_produzione_risorsa AS r ON r.SocietaID = m.SocietaID AND r.ID = m.RisorsaID
                        WHERE m.SocietaID = @SocietaID and m.RepartoID = @ID", new { SocietaID = CompanyID, ID = RepartoID });

            return new ObservableCollection<tab_produzione_risorsa>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<tab_produzione_risorsa>? GetListFromOperatore(string CompanyID, string OperatoreID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<tab_produzione_risorsa>(
                @"SELECT ris.*, COALESCE(utt.TipoID,'0') TipoID,utt.abers1, utt.Data,utt.ID,utt.Descrizione, utt.RevisioneID, utt.Reparto FROM tab_produzione_operatore_risorsa AS m
                        INNER JOIN tab_produzione_risorsa AS ris ON ris.SocietaID = m.SocietaID AND ris.ID = m.RisorsaID
                        OUTER APPLY (SELECT TOP(1) ut.TipoID ,ut.Data, ut_o.ID, ut_a.abers1,ut_p.Descrizione, ut_c.RevisioneID, ut_r.Descrizione as Reparto from pro_ordine_composizione_tempo as ut
			            inner join pro_ordine ut_o on ut_o.SocietaID = ut.SocietaID AND ut_o.ID = ut.OrdineID 
		                inner join pro_ordine_composizione ut_c on ut_c.SocietaID = ut.SocietaID AND ut_c.OrdineID = ut.OrdineID AND ut_c.ArticoloID = ut.ArticoloID AND ut_c.RevisioneID = ut.RevisioneID AND ut_C.ComposizioneID = ut.ComposizioneID
			            inner join tab_articolo ut_p on ut_p.SocietaID = ut_c.SocietaID AND ut_p.ID = ut_c.ArticoloID 
						inner join tab_produzione_reparto ut_r on ut_r.SocietaID = ut_c.SocietaID AND ut_r.ID = ut_c.RepartoID
			            inner join ABE ut_a on ut_a.abecod = ut_o.ClienteID
                        WHERE ut.SocietaID = ris.SocietaID AND ut.RisorsaID = ris.ID order by ut.Data desc) utt
                        WHERE m.SocietaID = @SocietaID and m.OperatoreID = @ID", new[] {
                        typeof(tab_produzione_risorsa),
                        typeof(string),
                        typeof(string),
                        typeof(DateTime),
                        typeof(string),
                        typeof(string),
                        typeof(string),
                        typeof(string) }, obj =>
                    {
                        var risorsa = (tab_produzione_risorsa)obj[0];

                        risorsa.UltimaComposizione = new pro_ordine_composizione
                        {
                            SocietaID = CompanyID,
                            ArticoloID = string.Empty,
                            RevisioneID = string.Empty,
                            OrdineID = obj[4] != null ? (obj[4] as string) ?? string.Empty : "0",
                            UltimoTempoTipoID = obj[1] != null ? obj[1] as string : "0",
                            ClienteDescrizione = obj[2] != null ? obj[2] as string : "0",
                            RepartoDescrizione = obj[7] != null ? obj[7] as string : "0",
                            ArticoloDescrizione = obj[5] != null ? obj[5] as string : "0",
                        };
                        return risorsa;
                    },
            new { SocietaID = CompanyID, ID = OperatoreID },
                splitOn: "SocietaID, TipoID, abers1, Data,ID,Descrizione,RevisioneID,Reparto");

            return new ObservableCollection<tab_produzione_risorsa>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<tab_produzione_risorsa>? GetListFromCausale(string CompanyID, string CausaleID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<tab_produzione_risorsa>(
                @"SELECT r.* FROM tab_produzione_causale_risorsa AS m
                        INNER JOIN tab_produzione_risorsa AS r ON r.SocietaID = m.SocietaID AND r.ID = m.RisorsaID
                        WHERE m.SocietaID = @SocietaID and m.CausaleID = @ID", new { SocietaID = CompanyID, ID = CausaleID });

            return new ObservableCollection<tab_produzione_risorsa>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public tab_produzione_risorsa? Get(string CompanyID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<tab_produzione_risorsa>(
                "SELECT * FROM tab_produzione_risorsa WHERE SocietaID = @SocietaID and ID = @ID",
                new { SocietaID = CompanyID, ID = ID })
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
                "SELECT COUNT(*) FROM tab_produzione_risorsa WHERE SocietaID = @SocietaID and ID = @ID",
                new { SocietaID = CompanyID, ID = ID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(tab_produzione_risorsa Model, ObservableCollection<tab_produzione_risorsa_sorgenti> Sources, ObservableCollection<tab_produzione_risorsa_costo> Costs)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var result = connection.Execute(
                        "INSERT INTO tab_produzione_risorsa (SocietaID,ID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID,Descrizione,PiazzamentoDefault,EInfinita,EPiazzamento,ECompleta,EVersamento,ETempoAlPezzo,EPiuOperatori,EPiuPezzi,EAutomatica,EFornitore)" +
                        " OUTPUT INSERTED.rv VALUES(@SocietaID,@ID,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID,@Descrizione,@PiazzamentoDefault,@EInfinita,@EPiazzamento,@ECompleta,@EVersamento,@ETempoAlPezzo,@EPiuOperatori,@EPiuPezzi,@EAutomatica,@EFornitore)",
                        Model, transaction);

                    //tab_produzione_risorsa_sorgenti
                    connection.Execute(@"DELETE FROM tab_produzione_risorsa_sorgenti WHERE SocietaID = @cid AND RisorsaID = @rid",
                        new { cid = Model.SocietaID, rid = Model.ID },
                        transaction);
                    if (Sources != null && Sources.Count > 0)
                    {
                        foreach (var item in Sources)
                        {
                            connection.Execute("INSERT INTO tab_produzione_risorsa_sorgenti (SocietaID,RisorsaID,ID,Descrizione,Singolo) OUTPUT INSERTED.rv VALUES(@SocietaID,@RisorsaID,@ID,@Descrizione,@Singolo)",
                                item, transaction);
                        }
                    }

                    //tab_produzione_risorsa_costo
                    connection.Execute(@"DELETE FROM tab_produzione_risorsa_costo WHERE SocietaID = @cid AND RisorsaID = @rid",
                        new { cid = Model.SocietaID, rid = Model.ID },
                        transaction);
                    if (Costs != null && Costs.Count > 0)
                    {
                        foreach (var item in Costs)
                        {
                            connection.Execute("INSERT INTO tab_produzione_risorsa_costo (SocietaID,RisorsaID,Periodo,CostoOrario) OUTPUT INSERTED.rv VALUES(@SocietaID,@RisorsaID,@Periodo,@CostoOrario)",
                                item, transaction);
                        }
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

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Update(tab_produzione_risorsa Model, ObservableCollection<tab_produzione_risorsa_sorgenti> Sources, ObservableCollection<tab_produzione_risorsa_costo> Costs)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                var result = connection.ExecuteScalar(
                    "UPDATE tab_produzione_risorsa SET LogAdded = @LogAdded,LogUpdated = @LogUpdated,LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID,LogCanceledUserID = @LogCanceledUserID,Descrizione = @Descrizione," +
                    "PiazzamentoDefault = @PiazzamentoDefault, EInfinita = @EInfinita, EPiazzamento = @EPiazzamento,ECompleta = @ECompleta ,EVersamento = @EVersamento ,ETempoAlPezzo = @ETempoAlPezzo ,EPiuOperatori = @EPiuOperatori ,EPiuPezzi = @EPiuPezzi,EAutomatica = @EAutomatica ,EFornitore = @EFornitore  " +
                    "OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND rv = @rv",
                    Model, transaction);

                //tab_produzione_risorsa_sorgenti
                connection.Execute(@"DELETE FROM tab_produzione_risorsa_sorgenti WHERE SocietaID = @cid AND RisorsaID = @rid",
                    new { cid = Model.SocietaID, rid = Model.ID },
                    transaction);
                if (Sources != null && Sources.Count > 0)
                {
                    foreach (var item in Sources)
                    {
                        connection.Execute("INSERT INTO tab_produzione_risorsa_sorgenti (SocietaID,RisorsaID,ID,Descrizione,Singolo) OUTPUT INSERTED.rv VALUES(@SocietaID,@RisorsaID,@ID,@Descrizione,@Singolo)",
                            item, transaction);
                    }
                }

                //tab_produzione_risorsa_costo
                connection.Execute(@"DELETE FROM tab_produzione_risorsa_costo WHERE SocietaID = @cid AND RisorsaID = @rid",
                    new { cid = Model.SocietaID, rid = Model.ID },
                    transaction);
                if (Costs != null && Costs.Count > 0)
                {
                    foreach (var item in Costs)
                    {
                        connection.Execute("INSERT INTO tab_produzione_risorsa_costo (SocietaID,RisorsaID,Periodo,CostoOrario) OUTPUT INSERTED.rv VALUES(@SocietaID,@RisorsaID,@Periodo,@CostoOrario)",
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

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Delete(tab_produzione_risorsa Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var result = connection.ExecuteScalar(
                        "DELETE FROM tab_produzione_risorsa OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND ID = @ID AND rv = @rv",
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

    public string? Validate(tab_produzione_risorsa Model, bool IsInsert)
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
}