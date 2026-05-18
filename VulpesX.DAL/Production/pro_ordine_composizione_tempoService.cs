using Chilkat;
using Org.BouncyCastle.Ocsp;
using VulpesX.Models.Models.Production;

namespace VulpesX.DAL.Production;

public interface Ipro_ordine_composizione_tempoRepository
{
    ObservableCollection<pro_ordine_composizione_tempo>? GetListByOrder(string SocietaID, string OrdineID);

    ObservableCollection<pro_ordine_composizione_tempo>? Get(string SocietaID, string OrdineID, string ArticoloID, string RevisioneID, long ComposizioneID);

    pro_ordine_composizione_tempo? Get(string SocietaID, string OrdineID, string ArticoloID, long ComposizioneID, long ProgressivoID);

    bool Exists(string SocietaID, string OrdineID, string ArticoloID, long ComposizioneID, long ProgressivoID);

    ObservableCollection<pro_ordine_composizione_tempo>? GetOperatore(string SocietaID, string OperatoreID, DateTime Dal, DateTime Al);

    ObservableCollection<pro_ordine_composizione_tempo>? GetRisorsa(string SocietaID, string RisorsaID, DateTime Dal, DateTime Al);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(pro_ordine_composizione_tempo Model);

    bool Update(pro_ordine_composizione_tempo Model);

    bool Delete(pro_ordine_composizione_tempo Model);

    string? Validate(pro_ordine_composizione_tempo Model, bool IsInsert);
    #endregion

    #region Ufp
    ObservableCollection<ProductionModel.ProductionTimeUfpModel>? GetProductionTime(string SocietaID, string Commessa);
    #endregion
}

public class pro_ordine_composizione_tempoRepository : RepositoryBase, Ipro_ordine_composizione_tempoRepository
{
    public pro_ordine_composizione_tempoRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<pro_ordine_composizione_tempo>? GetListByOrder(string SocietaID, string OrdineID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return new ObservableCollection<pro_ordine_composizione_tempo>(connection.Query<pro_ordine_composizione_tempo, tab_produzione_risorsa, tab_produzione_operatore, tab_produzione_causale, pro_ordine_composizione_tempo>(
                $@"SELECT c.*, ris.Descrizione, ope.Descrizione, cau.Descrizione FROM pro_ordine_composizione_tempo as c 
                        LEFT OUTER JOIN tab_produzione_risorsa AS ris ON c.SocietaID = ris.SocietaID AND c.RisorsaID = ris.ID 
                        LEFT OUTER JOIN tab_produzione_operatore AS ope ON c.SocietaID = ope.SocietaID AND c.OperatoreID = ope.ID 
                        LEFT OUTER JOIN tab_produzione_causale AS cau ON c.SocietaID = cau.SocietaID AND c.CausaleID = cau.ID 
                        WHERE c.SocietaID = @SocietaID AND c.OrdineID = @OrdineID",
                   (tempo, risorsa, operatore, causale) =>
                   {
                       tempo.OperatoreDescrizione = operatore?.Descrizione;
                       tempo.RisorsaDescrizione = risorsa?.Descrizione;
                       tempo.CausaleDescrizione = causale?.Descrizione;

                       return tempo;
                   },
                new { SocietaID = SocietaID, OrdineID = OrdineID },
                splitOn: "Descrizione,Descrizione,Descrizione,Descrizione"));

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<pro_ordine_composizione_tempo>? Get(string SocietaID, string OrdineID, string ArticoloID, string RevisioneID, long ComposizioneID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var tempi = new ObservableCollection<pro_ordine_composizione_tempo>(connection.Query<pro_ordine_composizione_tempo, tab_produzione_risorsa, tab_produzione_operatore, tab_produzione_causale, pro_ordine_composizione, pro_ordine_composizione_tempo>(
                $@"SELECT c.*, ris.Descrizione, ope.Descrizione, cau.Descrizione, poc.* FROM pro_ordine_composizione_tempo as c 
                        LEFT OUTER JOIN tab_produzione_risorsa AS ris ON c.SocietaID = ris.SocietaID AND c.RisorsaID = ris.ID 
                        LEFT OUTER JOIN tab_produzione_operatore AS ope ON c.SocietaID = ope.SocietaID AND c.OperatoreID = ope.ID 
                        LEFT OUTER JOIN tab_produzione_causale AS cau ON c.SocietaID = cau.SocietaID AND c.CausaleID = cau.ID  
                        LEFT OUTER JOIN pro_ordine_composizione as poc ON c.SocietaID = poc.SocietaID AND c.OrdineID = poc.OrdineID AND c.ArticoloID = poc.ArticoloID AND c.RevisioneID = poc.RevisioneID AND c.ComposizioneID = poc.ComposizioneID
                        WHERE c.SocietaID = @SocietaID AND c.OrdineID = @OrdineID AND c.ArticoloID = @ArticoloID AND c.RevisioneID = @RevisioneID AND c.ComposizioneID = @ComposizioneID 
                        ORDER BY c.Data",
                   (tempo, risorsa, operatore, causale, composizione) =>
                  {
                      tempo.OperatoreDescrizione = operatore?.Descrizione;
                      tempo.RisorsaDescrizione = risorsa?.Descrizione;
                      tempo.CausaleDescrizione = causale?.Descrizione;
                      tempo.QuantitaSparata = composizione.Quantita ?? 0;
                      return tempo;
                  },
                new { SocietaID = SocietaID, OrdineID = OrdineID, ArticoloID = ArticoloID, RevisioneID = RevisioneID, ComposizioneID = ComposizioneID },
                splitOn: "Descrizione,Descrizione,Descrizione,Descrizione,SocietaID"));

            foreach (var tim in tempi.Where(o => (o.Box ?? 0) > 0 && o.TipoID != Constants._TEMPOID_VERSAMENTO))
            {
                var versata = connection.Query<decimal>("SELECT QuantitaVersata FROM pro_ordine_composizione_tempo WHERE SocietaID = @SocietaID AND OrdineID = @OrdineID AND ArticoloID = @ArticoloID AND RevisioneID = @RevisioneID AND Box = @Box AND TipoID = @TipoID AND Data < @Data ORDER BY Data DESC", new { SocietaID = tim.SocietaID, OrdineID = tim.OrdineID, ArticoloID = tim.ArticoloID, RevisioneID = tim.RevisioneID, Box = tim.Box, TipoID = Constants._TEMPOID_VERSAMENTO, Data = tim.Data }).FirstOrDefault();

                tim.QuantitaSparata = versata;
            }

            return new ObservableCollection<pro_ordine_composizione_tempo>(tempi);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public pro_ordine_composizione_tempo? Get(string SocietaID, string OrdineID, string ArticoloID, long ComposizioneID, long ProgressivoID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<pro_ordine_composizione_tempo>(
                "SELECT * FROM pro_ordine_composizione_tempo WHERE SocietaID = @SocietaID AND OrdineID = @OrdineID AND ArticoloID = @ArticoloID AND ComposizioneID = @ComposizioneID AND ProgressivoID = @ProgressivoID",
                new { SocietaID = SocietaID, OrdineID = OrdineID, ArticoloID = ArticoloID, ComposizioneID = ComposizioneID, ProgressivoID = ProgressivoID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string SocietaID, string OrdineID, string ArticoloID, long ComposizioneID, long ProgressivoID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM pro_ordine_composizione_tempo WHERE SocietaID = @SocietaID AND OrdineID = @OrdineID AND ArticoloID = @ArticoloID AND ComposizioneID = @ComposizioneID AND ProgressivoID = @ProgressivoID",
                new { SocietaID = SocietaID, OrdineID = OrdineID, ArticoloID = ArticoloID, ComposizioneID = ComposizioneID, ProgressivoID = ProgressivoID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public ObservableCollection<pro_ordine_composizione_tempo>? GetOperatore(string SocietaID, string OperatoreID, DateTime Dal, DateTime Al)
    {
        try
        {
            using var connection = GetOpenConnection();


            return new ObservableCollection<pro_ordine_composizione_tempo>(connection.Query<pro_ordine_composizione_tempo, tab_produzione_risorsa, tab_produzione_operatore, tab_produzione_causale, pro_ordine_composizione_tempo>(
                $@"SELECT c.*, ris.Descrizione, ope.Descrizione, cau.Descrizione FROM pro_ordine_composizione_tempo as c 
                        LEFT OUTER JOIN tab_produzione_risorsa AS ris ON c.SocietaID = ris.SocietaID AND c.RisorsaID = ris.ID 
                        LEFT OUTER JOIN tab_produzione_operatore AS ope ON c.SocietaID = ope.SocietaID AND c.OperatoreID = ope.ID 
                        LEFT OUTER JOIN tab_produzione_causale AS cau ON c.SocietaID = cau.SocietaID AND c.CausaleID = cau.ID 
                        WHERE c.SocietaID = @SocietaID AND c.OperatoreID = @OperatoreID AND c.Data >= @Dal AND c.Data <= @Al 
                        ORDER BY c.Data desc",
                   (tempo, risorsa, operatore, causale) =>
                   {
                       tempo.OperatoreDescrizione = operatore?.Descrizione;
                       tempo.RisorsaDescrizione = risorsa?.Descrizione;
                       tempo.CausaleDescrizione = causale?.Descrizione;

                       return tempo;
                   },
                new { SocietaID = SocietaID, OperatoreID = OperatoreID, Dal = Dal, Al = Al },
                splitOn: "Descrizione,Descrizione,Descrizione,Descrizione"));

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<pro_ordine_composizione_tempo>? GetRisorsa(string SocietaID, string RisorsaID, DateTime Dal, DateTime Al)
    {
        try
        {
            using var connection = GetOpenConnection();


            return new ObservableCollection<pro_ordine_composizione_tempo>(connection.Query<pro_ordine_composizione_tempo, tab_produzione_risorsa, tab_produzione_operatore, tab_produzione_causale, pro_ordine_composizione_tempo>(
                $@"SELECT c.*, ris.Descrizione, ope.Descrizione, cau.Descrizione FROM pro_ordine_composizione_tempo as c 
                        LEFT OUTER JOIN tab_produzione_risorsa AS ris ON c.SocietaID = ris.SocietaID AND c.RisorsaID = ris.ID 
                        LEFT OUTER JOIN tab_produzione_operatore AS ope ON c.SocietaID = ope.SocietaID AND c.OperatoreID = ope.ID 
                        LEFT OUTER JOIN tab_produzione_causale AS cau ON c.SocietaID = cau.SocietaID AND c.CausaleID = cau.ID 
                        WHERE c.SocietaID = @SocietaID AND c.RisorsaID = @RisorsaID AND c.Data >= @Dal AND c.Data <= @Al 
                        ORDER BY c.Data desc",
                   (tempo, risorsa, operatore, causale) =>
                   {
                       tempo.OperatoreDescrizione = operatore?.Descrizione;
                       tempo.RisorsaDescrizione = risorsa?.Descrizione;
                       tempo.CausaleDescrizione = causale?.Descrizione;

                       return tempo;
                   },
                new { SocietaID = SocietaID, RisorsaID = RisorsaID, Dal = Dal, Al = Al },
                splitOn: "Descrizione,Descrizione,Descrizione,Descrizione"));

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO pro_ordine_composizione_tempo (SocietaID,OrdineID,ArticoloID,ComposizioneID,ProgressivoID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID,Data,TipoID,RisorsaID,OperatoreID,CausaleID,Durata,DurataSospensione,EProcessata,QuantitaFase,QuantitaVersata,QuantitaScartata,Note,Lotto) OUTPUT INSERTED.rv VALUES(@SocietaID,@OrdineID,@ArticoloID,@ComposizioneID,@ProgressivoID,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID,@Data,@TipoID,@RisorsaID,@OperatoreID,@CausaleID,@Durata,@DurataSospensione,@EProcessata,@QuantitaFase,@QuantitaVersata,@QuantitaScartata,@Note,@Lotto)";
    public string UPDATE_QUERY => "UPDATE pro_ordine_composizione_tempo SET SocietaID = @SocietaID,OrdineID = @OrdineID,ArticoloID = @ArticoloID,ComposizioneID = @ComposizioneID,ProgressivoID = @ProgressivoID,LogAdded = @LogAdded,LogUpdated = @LogUpdated,LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID,LogCanceledUserID = @LogCanceledUserID,Data = @Data,TipoID = @TipoID,RisorsaID = @RisorsaID,OperatoreID = @OperatoreID,CausaleID = @CausaleID,Durata = @Durata,DurataSospensione = @DurataSospensione,EProcessata = @EProcessata,QuantitaFase = @QuantitaFase,QuantitaVersata = @QuantitaVersata,QuantitaScartata = @QuantitaScartata,Note = @Note,Lotto = @Lotto OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND OrdineID = @OrdineID AND ArticoloID = @ArticoloID AND ComposizioneID = @ComposizioneID AND ProgressivoID = @ProgressivoID AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM pro_ordine_composizione_tempo OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND OrdineID = @OrdineID AND ArticoloID = @ArticoloID AND ComposizioneID = @ComposizioneID AND ProgressivoID = @ProgressivoID AND rv = @rv";
    public bool Insert(pro_ordine_composizione_tempo Model)
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

    public bool Update(pro_ordine_composizione_tempo Model)
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

    public bool Delete(pro_ordine_composizione_tempo Model)
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

    public string? Validate(pro_ordine_composizione_tempo Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.SocietaID) && IsInsert && !Exists(Model.SocietaID, Model.OrdineID, Model.ArticoloID, Model.ComposizioneID, Model.ProgressivoID)) || !IsInsert)
            {
                return null;
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

    #region Ufp
    public ObservableCollection<ProductionModel.ProductionTimeUfpModel>? GetProductionTime(string SocietaID, string Commessa)
    {
        throw new NotImplementedException();
    }
    #endregion
}

public class pro_ordine_composizione_tempoUfpRepository : RepositoryBase, Ipro_ordine_composizione_tempoRepository
{
    public pro_ordine_composizione_tempoUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<pro_ordine_composizione_tempo>? GetListByOrder(string SocietaID, string OrdineID)
    {
        throw new NotImplementedException();
    }

    public ObservableCollection<pro_ordine_composizione_tempo>? Get(string SocietaID, string OrdineID, string ArticoloID, string RevisioneID, long ComposizioneID)
    {
        throw new NotImplementedException();
    }

    public pro_ordine_composizione_tempo? Get(string SocietaID, string OrdineID, string ArticoloID, long ComposizioneID, long ProgressivoID)
    {
        throw new NotImplementedException();
    }

    public bool Exists(string SocietaID, string OrdineID, string ArticoloID, long ComposizioneID, long ProgressivoID)
    {
        throw new NotImplementedException();
    }

    public ObservableCollection<pro_ordine_composizione_tempo>? GetOperatore(string SocietaID, string OperatoreID, DateTime Dal, DateTime Al)
    {
        throw new NotImplementedException();
    }

    public ObservableCollection<pro_ordine_composizione_tempo>? GetRisorsa(string SocietaID, string RisorsaID, DateTime Dal, DateTime Al)
    {
        throw new NotImplementedException();
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO pro_ordine_composizione_tempo (SocietaID,OrdineID,ArticoloID,ComposizioneID,ProgressivoID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID,Data,TipoID,RisorsaID,OperatoreID,CausaleID,Durata,DurataSospensione,EProcessata,QuantitaFase,QuantitaVersata,QuantitaScartata,Note,Lotto) OUTPUT INSERTED.rv VALUES(@SocietaID,@OrdineID,@ArticoloID,@ComposizioneID,@ProgressivoID,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID,@Data,@TipoID,@RisorsaID,@OperatoreID,@CausaleID,@Durata,@DurataSospensione,@EProcessata,@QuantitaFase,@QuantitaVersata,@QuantitaScartata,@Note,@Lotto)";
    public string UPDATE_QUERY => "UPDATE pro_ordine_composizione_tempo SET SocietaID = @SocietaID,OrdineID = @OrdineID,ArticoloID = @ArticoloID,ComposizioneID = @ComposizioneID,ProgressivoID = @ProgressivoID,LogAdded = @LogAdded,LogUpdated = @LogUpdated,LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID,LogCanceledUserID = @LogCanceledUserID,Data = @Data,TipoID = @TipoID,RisorsaID = @RisorsaID,OperatoreID = @OperatoreID,CausaleID = @CausaleID,Durata = @Durata,DurataSospensione = @DurataSospensione,EProcessata = @EProcessata,QuantitaFase = @QuantitaFase,QuantitaVersata = @QuantitaVersata,QuantitaScartata = @QuantitaScartata,Note = @Note,Lotto = @Lotto OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND OrdineID = @OrdineID AND ArticoloID = @ArticoloID AND ComposizioneID = @ComposizioneID AND ProgressivoID = @ProgressivoID AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM pro_ordine_composizione_tempo OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND OrdineID = @OrdineID AND ArticoloID = @ArticoloID AND ComposizioneID = @ComposizioneID AND ProgressivoID = @ProgressivoID AND rv = @rv";
    public bool Insert(pro_ordine_composizione_tempo Model)
    {
        throw new NotImplementedException();
    }

    public bool Update(pro_ordine_composizione_tempo Model)
    {
        throw new NotImplementedException();
    }

    public bool Delete(pro_ordine_composizione_tempo Model)
    {
        throw new NotImplementedException();
    }

    public string? Validate(pro_ordine_composizione_tempo Model, bool IsInsert)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Ufp
    public ObservableCollection<ProductionModel.ProductionTimeUfpModel>? GetProductionTime(string SocietaID, string Commessa)
    {
        try
        {
            using var connection = GetOpenConnection();

            var retValue = new List<ProductionModel.ProductionTimeUfpModel>();

            var productions = connection.Query<ProductionModel.ProductionTimeUfpModel>(
               $@"SELECT 
                    c.*, 
                    c.tetiav as TipoID,
                    c.TEQCON as QuantitaVersata,
                    c.TEQNCO as QuantitaScartata,
                    po.OPQPRO as QuantitaFase,
                    ava.avades as TipoDescrizione,
                    c.tedata as Data,
                    c.temacc as RisorsaID,
                    mac.macdes as RisorsaDescrizione,
                    c.tematr as OperatoreID,
                    mat.matnom as OperatoreDescrizione,
                    pl.*,
                    pl.PRREPA as RepartoID,
                    rep.repdes as RepartoDescrizione,
                    fas.lavdes as FaseDescrizione,
                    po.*,
                    po.opcomme as Commessa
                    FROM PROD_TEMPI AS c 
                    LEFT OUTER JOIN PROD_ORDINI as po ON c.TESOCI = po.OPSOCI AND c.TEANNP = po.OPANNP AND c.TEORDP = po.OPNUOP
                    LEFT OUTER JOIN PROD_LANCI as pl ON c.TESOCI = pl.PRSOCB AND c.TEANNP = pl.PRANNP AND c.TEORDP = pl.PRORDP AND c.TENSEQ = pl.PRNSEQ
                    LEFT OUTER JOIN PROD_REPARTI as rep ON rep.repsoc = pl.PRSOCB AND rep.repcod = pl.PRREPA
                    LEFT OUTER JOIN PROD_FASI as fas ON fas.lavsoc = pl.PRSOCB AND fas.lavcod = pl.PRFASE
                    LEFT OUTER JOIN PROD_MACCHINE as mac ON mac.macsoc = pl.PRSOCB AND mac.maccod = c.temacc
                    LEFT OUTER JOIN PROD_MATRICOLE as mat ON mat.matsoc = pl.PRSOCB AND mat.matcod = c.tematr
                    LEFT OUTER JOIN PROD_CAUS_AVANZAMENTI as ava ON ava.avasoc = c.TESOCI AND ava.avatip = c.TETIAV
                    WHERE po.OPSOCI = @SocietaID AND po.OPCOMME = @Commessa
                    order by c.TENSEQ, c.TEDATA",
                new[] { typeof(pro_ordine_composizione_tempo), typeof(pro_ordine_composizione), typeof(pro_ordine) }
           , (objects) =>
           {
               var tempo = (objects[0] as pro_ordine_composizione_tempo);
               var lancio = objects[1] as pro_ordine_composizione;
               var ordine = objects[2] as pro_ordine;

               var composizione = new ProductionModel.ProductionTimeUfpModel
               {
                   SocietaID = tempo.TESOCI,
                   Anno = tempo.TEANNP ?? 0,
                   ID = tempo.TEORDP ?? 0,
                   Sequenza = tempo.TENSEQ ?? 0,
                   Progressivo = tempo.TERIGA ?? 0,
                   Data = tempo.Data,
                   TipoID = tempo.TipoID,
                   TipoDescrizione = tempo.TipoDescrizione,
                   QuantitaOrdine = tempo.QuantitaFase,
                   QuantitaVersata = tempo.QuantitaVersata,
                   QuantitaScartata = tempo.QuantitaScartata,
                   RepartoID = lancio.RepartoID,
                   RepartoDescrizione = lancio.RepartoDescrizione,
                   FaseID = lancio.PRFASE,
                   FaseDescrizione = lancio.FaseDescrizione,
                   OperatoreID = tempo.OperatoreID,
                   OperatoreDescrizione = tempo.OperatoreDescrizione,
                   RisorsaID = tempo.RisorsaID,
                   RisorsaDescrizione = tempo.RisorsaDescrizione,
                   EProcessata = tempo.teprocessed == "1",
               };

               return composizione;
           },
          new { SocietaID, Commessa },
           splitOn: "PRSOCB,OPSOCI");

            string lastSequenza = string.Empty;
            ProductionModel.ProductionTimeUfpModel? lastProduzione = null;
            TimeSpan totalePiazzamento = new TimeSpan();
            TimeSpan totaleProduzione = new TimeSpan();

            foreach (var seq in productions.GroupBy(g => g.SequenzaFull))
            {
                if (!string.IsNullOrEmpty(lastSequenza) && lastSequenza != seq.Key)
                {
                    retValue.Add(new ProductionModel.ProductionTimeUfpModel
                    {
                        SocietaID = lastProduzione.SocietaID,
                        Anno = lastProduzione.Anno,
                        ID = lastProduzione.ID,
                        Sequenza = lastProduzione.Sequenza,
                        IsTotal = true,
                        TipoDescrizione = "TOTALE",
                        Data = lastProduzione.Data,
                        TotalePiazzamento = totalePiazzamento,
                        TotaleProduzione = totaleProduzione,
                        QuantitaScartata = lastProduzione.QuantitaScartata,
                        QuantitaVersata = lastProduzione.QuantitaVersata,
                        QuantitaOrdine = lastProduzione.QuantitaOrdine,
                        TempoProduzionePezzo = ((lastProduzione.QuantitaVersata ?? 0) > 0) ? new TimeSpan(0, (int)(totaleProduzione.TotalMinutes / (double)(lastProduzione.QuantitaVersata ?? 0)), 0) : new TimeSpan(),
                    });

                    totalePiazzamento = new TimeSpan();
                    totaleProduzione = new TimeSpan();
                }

                DateTime? lastData = null;
                foreach (var tim in seq.OrderBy(o => o.Data))
                {
                    if (lastData != null && tim.EProcessata)
                    {
                        tim.Durata = tim.Data - lastData;

                        if (tim.TipoID == "7")
                            totalePiazzamento += tim.Durata ?? new TimeSpan();
                        else
                            totaleProduzione += tim.Durata ?? new TimeSpan();
                    }

                    lastData = tim.Data;
                    retValue.Add(tim);
                }

                lastSequenza = seq.Key;
                lastProduzione = seq.LastOrDefault();
            }

            if (!string.IsNullOrEmpty(lastSequenza) && productions.Any())
            {
                retValue.Add(new ProductionModel.ProductionTimeUfpModel
                {
                    SocietaID = lastProduzione.SocietaID,
                    Anno = lastProduzione.Anno,
                    ID = lastProduzione.ID,
                    Sequenza = lastProduzione.Sequenza,
                    IsTotal = true,
                    TipoDescrizione = "TOTALE",
                    Data = lastProduzione.Data,
                    TotalePiazzamento = totalePiazzamento,
                    TotaleProduzione = totaleProduzione,
                    QuantitaScartata = lastProduzione.QuantitaScartata,
                    QuantitaVersata = lastProduzione.QuantitaVersata,
                    QuantitaOrdine = lastProduzione.QuantitaOrdine,
                    TempoProduzionePezzo = ((lastProduzione.QuantitaVersata ?? 0) > 0) ? new TimeSpan(0, (int)(totaleProduzione.TotalMinutes / (double)(lastProduzione.QuantitaVersata ?? 0)), 0) : new TimeSpan(),
                });
            }

            return new ObservableCollection<ProductionModel.ProductionTimeUfpModel>(retValue);
        }
        catch (Exception exc)
        {
            ErrorHandler.Show(exc.Message);
            return null;
        }
    }
    #endregion
}