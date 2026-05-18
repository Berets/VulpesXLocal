using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace VulpesX.DAL.Tables.EnergyMonitor
{
    public interface IEM_DEVICERepository
    {
        ObservableCollection<EM_DEVICE>? GetList(string CompanyID);

        EM_DEVICE? Get(string CompanyID, string ID);

        bool Exists(string CompanyID, string ID);

        #region CRUD
        bool Insert(EM_DEVICE Model, ObservableCollection<EM_DEVICE_PERIOD> Costs);

        bool Update(EM_DEVICE Model, ObservableCollection<EM_DEVICE_PERIOD> Costs);

        bool Delete(EM_DEVICE Model);

        string? Validate(EM_DEVICE Model, bool IsInsert);
        #endregion
    }

    public class EM_DEVICERepository : RepositoryBase, IEM_DEVICERepository
    {
        public EM_DEVICERepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<EM_DEVICE>? GetList(string CompanyID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<EM_DEVICE>(
                    "SELECT * FROM EM_DEVICE  WHERE SocietaID = @SocietaID", new { SocietaID = CompanyID });

                return new ObservableCollection<EM_DEVICE>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public EM_DEVICE? Get(string CompanyID, string ID)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<EM_DEVICE>(
                    "SELECT * FROM EM_DEVICE WHERE SocietaID = @SocietaID and ID = @ID",
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
                    "SELECT COUNT(*) FROM EM_DEVICE WHERE SocietaID = @SocietaID and ID = @ID",
                    new { SocietaID = CompanyID, ID = ID }) > 0;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        #region CRUD
        public bool Insert(EM_DEVICE Model, ObservableCollection<EM_DEVICE_PERIOD> Costs)
        {
            using var connection = GetOpenConnection();



            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var result = connection.Execute(
                        "INSERT INTO EM_DEVICE (SocietaID,ID,Descrizione,Topic,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID)" +
                        " OUTPUT INSERTED.rv VALUES(@SocietaID,@ID,@Descrizione,@Topic,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID)",
                        Model, transaction);

                    //EM_DEVICE_PERIOD
                    connection.Execute(@"DELETE FROM EM_DEVICE_PERIOD WHERE SocietaID = @cid AND DeviceID = @rid",
                        new { cid = Model.SocietaID, rid = Model.ID },
                        transaction);
                    if (Costs != null && Costs.Count > 0)
                    {
                        foreach (var item in Costs)
                        {
                            connection.Execute("INSERT INTO EM_DEVICE_PERIOD (SocietaID,DeviceID,Mese,Costo) VALUES(@SocietaID,@DeviceID,@Mese,@Costo)",
                                item, transaction);
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(ex.Message);
                    return false;
                }
            }

        }

        public bool Update(EM_DEVICE Model, ObservableCollection<EM_DEVICE_PERIOD> Costs)
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var result = connection.ExecuteScalar(
                        @"UPDATE EM_DEVICE SET LogAdded = @LogAdded, LogUpdated = @LogUpdated,LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID,LogCanceledUserID = @LogCanceledUserID,
                            Descrizione = @Descrizione, Topic=@Topic OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND rv = @rv",
                        Model, transaction);

                    //EM_DEVICE_PERIOD
                    connection.Execute(@"DELETE FROM EM_DEVICE_PERIOD WHERE SocietaID = @cid AND DeviceID = @rid",
                        new { cid = Model.SocietaID, rid = Model.ID },
                        transaction);
                    if (Costs != null && Costs.Count > 0)
                    {
                        foreach (var item in Costs)
                        {
                            connection.Execute("INSERT INTO EM_DEVICE_PERIOD (SocietaID,DeviceID,Mese,Costo) VALUES(@SocietaID,@DeviceID,@Mese,@Costo)",
                                item, transaction);
                        }
                    }

                    transaction.Commit();
                    return true;

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(ex.Message);
                    return false;
                }
            }

        }

        public bool Delete(EM_DEVICE Model)
        {
            try
            {
                using var connection = GetOpenConnection();



                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var result = connection.Execute(
                     "DELETE FROM EM_DEVICE OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND ID = @ID AND rv = @rv",
                     Model);

                        result = connection.Execute(@"DELETE FROM EM_DEVICE_PERIOD WHERE SocietaID = @cid AND DeviceID = @rid",
                       new { cid = Model.SocietaID, rid = Model.ID },
                       transaction);

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

        public string? Validate(EM_DEVICE Model, bool IsInsert)
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
}
