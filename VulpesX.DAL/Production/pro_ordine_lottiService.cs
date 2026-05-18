
using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.Auth;
using VulpesX.DAL.General;
using VulpesX.DAL.Store;
using VulpesX.Models.Models.Production;
using VulpesX.Models.Reports.Production;

namespace VulpesX.DAL.Production;

public interface Ipro_ordine_lottiRepository
{
    ObservableCollection<pro_ordine_lotti>? GetList(string CompanyID);

    ObservableCollection<pro_ordine_lotti>? GetListByProductOrderID(string CompanyID, string ProductOrderID);

    pro_ordine_lotti? Get(string SocietaID, string OrdineID, string ID);

    pro_ordine_lotti? GetFull(string CompanyID, string OrderID, string ID);

    bool Exists(string SocietaID, string OrdineID, string ID);

    #region Tracking
    FullLotTracking? Track(pro_ordine_lotti Data);
    #endregion

    #region Report
    FinalLotLabelReport? PrintLabel(pro_ordine_lotti Data);
    #endregion

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(pro_ordine_lotti Model);

    bool Update(pro_ordine_lotti Model);

    bool Delete(pro_ordine_lotti Model);

    string? Validate(pro_ordine_lotti Model, bool IsInsert);
    #endregion
}

public class pro_ordine_lottiRepository : RepositoryBase, Ipro_ordine_lottiRepository
{
    public pro_ordine_lottiRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<pro_ordine_lotti>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<pro_ordine_lotti, pro_ordine_composizione_tempo, tab_articolo, tab_produzione_risorsa, pro_ordine_lotti>(
                @"SELECT l.*, t.*, p.*, r.* FROM pro_ordine_lotti AS l
                        INNER JOIN pro_ordine_composizione_tempo AS t ON t.SocietaID = l.SocietaID AND t.OrdineID = l.OrdineID AND t.Lotto = l.ID
                        INNER JOIN tab_articolo AS p ON p.SocietaID = t.SocietaID AND p.ID = t.ArticoloID
                        INNER JOIN tab_produzione_risorsa AS r ON r.SocietaID = t.SocietaID AND r.ID = t.RisorsaID
                        WHERE l.SocietaID = @cid
                        ORDER BY l.ID DESC",
                (lot, tmp, prd, prs) => { lot.ProductionTime = tmp; lot.Product = prd; lot.ProductionResource = prs; return lot; },
                new { cid = CompanyID }, splitOn: "SocietaID,SocietaID,SocietaID");

            return new ObservableCollection<pro_ordine_lotti>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<pro_ordine_lotti>? GetListByProductOrderID(string CompanyID, string ProductOrderID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<pro_ordine_lotti, pro_ordine_composizione_tempo, tab_articolo, tab_produzione_risorsa, tab_produzione_operatore, pro_ordine_lotti>(
                @"SELECT l.*, t.*, p.*, r.*, o.* FROM pro_ordine_lotti AS l
                        INNER JOIN pro_ordine_composizione_tempo AS t ON t.SocietaID = l.SocietaID AND t.OrdineID = l.OrdineID AND t.Lotto = l.ID
                        INNER JOIN tab_articolo AS p ON p.SocietaID = t.SocietaID AND p.ID = t.ArticoloID
                        INNER JOIN tab_produzione_risorsa AS r ON r.SocietaID = t.SocietaID AND r.ID = t.RisorsaID
                        INNER JOIN tab_produzione_operatore AS o ON o.SocietaID = l.SocietaID AND o.ID = l.addedUserID
                        WHERE l.SocietaID = @cid AND l.OrdineID=@poid
                        ORDER BY l.ID DESC",
                (lot, tmp, prd, prs, ope) =>
                {
                    lot.ProductionTime = tmp;
                    lot.Product = prd;
                    lot.ProductionResource = prs;
                    lot.Operator = ope;
                    return lot;
                },
                new { cid = CompanyID, poid = ProductOrderID }, splitOn: "SocietaID,SocietaID,SocietaID,SocietaID");

            return new ObservableCollection<pro_ordine_lotti>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public pro_ordine_lotti? Get(string SocietaID, string OrdineID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<pro_ordine_lotti>(
                "SELECT * FROM pro_ordine_lotti WHERE SocietaID = @SocietaID AND OrdineID = @OrdineID AND ID = @ID",
                new { SocietaID = SocietaID, OrdineID = OrdineID, ID = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public pro_ordine_lotti? GetFull(string CompanyID, string OrderID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<pro_ordine_lotti, pro_ordine_composizione_tempo, tab_articolo, tab_produzione_risorsa, pro_ordine_lotti>(
                @"SELECT l.*, t.*, p.*, r.* FROM pro_ordine_lotti AS l
                        INNER JOIN pro_ordine_composizione_tempo AS t ON t.SocietaID = l.SocietaID AND t.OrdineID = l.OrdineID AND t.Lotto = l.ID
                        INNER JOIN tab_articolo AS p ON p.SocietaID = t.SocietaID AND p.ID = t.ArticoloID
                        INNER JOIN tab_produzione_risorsa AS r ON r.SocietaID = t.SocietaID AND r.ID = t.RisorsaID
                        WHERE l.SocietaID = @cid AND l.OrdineID = @oid AND l.ID=@id
                        ORDER BY l.ID DESC",
                (lot, tmp, prd, prs) => { lot.ProductionTime = tmp; lot.Product = prd; lot.ProductionResource = prs; return lot; },
                new { cid = CompanyID, oid = OrderID, id = ID }, splitOn: "SocietaID,SocietaID,SocietaID").FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string SocietaID, string OrdineID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM pro_ordine_lotti WHERE SocietaID = @SocietaID AND OrdineID = @OrdineID AND ID = @ID",
                new { SocietaID = SocietaID, OrdineID = OrdineID, ID = ID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region Tracking
    public FullLotTracking? Track(pro_ordine_lotti Data)
    {
        try
        {
            var ordineLottiRepo = VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_lottiRepository>();
            var ordineRepo = VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordineRepository>();
            var ordineComposizioneTempoRepo = VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_composizione_tempoRepository>();
            var ordineHistoryRepo = VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_historyRepository>();

            var storeStockEngageRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>();
            var storeStockLotRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_lotsRepository>();


            var result = new FullLotTracking()
            {
                Lot = Data
            };
            // linked lots, excluding main
            result.LinkedLots = ordineLottiRepo.GetListByProductOrderID(Data.SocietaID, Data.OrdineID)?.ToList();
            // production order
            result.ProductionOrder = ordineRepo.Get(Data.SocietaID, Data.OrdineID);
            // production history
            result.ProductionHistory = ordineHistoryRepo.GetListByOrder(Data.SocietaID, Data.OrdineID)?.ToList();
            // production times
            result.ProductionTimes = ordineComposizioneTempoRepo.GetListByOrder(Data.SocietaID, Data.OrdineID)?.ToList();
            // engages
            result.Engages = storeStockEngageRepo.GetListByOrderID(Data.SocietaID, Data.OrdineID)?.ToList();
            // lot info
            result.LotInfo = storeStockLotRepo.GetSimpleListByLot(Data.SocietaID, Data.ID)?.ToList();
            return result;
        }
        catch (Exception exc)
        {
            ErrorHandler.Show(exc.Message);
            return null;
        }
    }
    #endregion

    #region Report
    public FinalLotLabelReport? PrintLabel(pro_ordine_lotti Data)
    {
        try
        {
            var companyRepo = VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>();
            var aziendaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>();

            var socbase = companyRepo.Get(Data.SocietaID)!;
            return new FinalLotLabelReport()
            {
                Data = Data,
                CompanyInfo = aziendaRepo.Get(Data.SocietaID),
                LogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{socbase.SOCUID}/{StorageHelper.CUSTOM_FOLDER}logo.png")
            };
        }
        catch (Exception exc)
        {
            ErrorHandler.Show(exc.Message);
            return null;
        }
    }
    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO pro_ordine_lotti (SocietaID,OrdineID,ID,Descrizione,ExpireDate,added,addedUserID,updated,updatedUserID,canceled,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@SocietaID,@OrdineID,@ID,@Descrizione,@ExpireDate,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@addedUserID,@updated,@updatedUserID,@canceled,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE pro_ordine_lotti SET SocietaID = @SocietaID,OrdineID = @OrdineID,ID = @ID,Descrizione = @Descrizione,ExpireDate = @ExpireDate,added = @added,addedUserID = @addedUserID,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',updatedUserID = @updatedUserID,canceled = @canceled,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND OrdineID = @OrdineID AND ID = @ID AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM pro_ordine_lotti OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND OrdineID = @OrdineID AND ID = @ID AND rv = @rv";
    public bool Insert(pro_ordine_lotti Model)
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

    public bool Update(pro_ordine_lotti Model)
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

    public bool Delete(pro_ordine_lotti Model)
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

    public string? Validate(pro_ordine_lotti Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.ID) && IsInsert && !Exists(Model.SocietaID, Model.OrdineID, Model.ID)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.Descrizione))
                {
                    return null;
                }
                else
                { return "La descrizione è obbligatoria"; }
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