using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.DAL.Tables.EnergyMonitor
{
    public interface IEM_DEVICE_PERIODRepository
    {
        ObservableCollection<EM_DEVICE_PERIOD>? GetList(string SocietaID, string DeviceID);

        EM_DEVICE_PERIOD? Get(string SocietaID, string DeviceID, DateTime Mese);

        bool Exists(string SocietaID, string DeviceID, DateTime Mese);

        #region CRUD
        string? Validate(EM_DEVICE_PERIOD Model, bool IsInsert);
        #endregion
    }

    public class EM_DEVICE_PERIODRepository : RepositoryBase, IEM_DEVICE_PERIODRepository
    {
        public EM_DEVICE_PERIODRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<EM_DEVICE_PERIOD>? GetList(string SocietaID, string DeviceID)
        {
            try
            {
            using var connection = GetOpenConnection();

                
                    var list = connection.Query<EM_DEVICE_PERIOD>(
                        @"SELECT * FROM EM_DEVICE_PERIOD
                        WHERE SocietaID = @SocietaID AND DeviceID = @DeviceID",
                        new { SocietaID = SocietaID, DeviceID = DeviceID });

                    return new ObservableCollection<EM_DEVICE_PERIOD>(list);
              
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public EM_DEVICE_PERIOD? Get(string SocietaID, string DeviceID, DateTime Mese)
        {
            try
            {
            using var connection = GetOpenConnection();

          
                    return connection.Query<EM_DEVICE_PERIOD>(
                        "SELECT * FROM EM_DEVICE_PERIOD WHERE SocietaID = @SocietaID AND DeviceID = @DeviceID AND Mese = @Mese",
                        new { SocietaID = SocietaID, DeviceID = DeviceID, Mese = Mese })
                        .FirstOrDefault();
               
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool Exists(string SocietaID, string DeviceID, DateTime Mese)
        {
            try
            {
            using var connection = GetOpenConnection();

       
                    return (int?)connection.ExecuteScalar(
                        "SELECT COUNT(*) FROM EM_DEVICE_PERIOD WHERE SocietaID = @SocietaID AND DeviceID = @DeviceID AND Mese = @Mese",
                        new { SocietaID = SocietaID, DeviceID = DeviceID, Mese = Mese }) > 0;
              
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        #region CRUD
        public string? Validate(EM_DEVICE_PERIOD Model, bool IsInsert)
        {
            try
            {
                if ((!string.IsNullOrEmpty(Model.SocietaID) && !string.IsNullOrEmpty(Model.DeviceID) &&
                    IsInsert && !Exists(Model.SocietaID, Model.DeviceID, Model.Mese)) || !IsInsert)
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
    }
}
