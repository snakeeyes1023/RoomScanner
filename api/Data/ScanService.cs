using RoomScannerWeb.Data.Models;
using SQLite;
using System.Net;
using System;

namespace RoomScannerWeb.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="RoomScannerWeb.Data.IScanService" />
    public class ScanService : IScanService
    {
        public event EventHandler? OnDataHasChanged;

        private readonly SQLiteConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanService"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public ScanService(SQLiteConnection dbContext)
        {
            _connection = dbContext;
        }

        /// <summary>
        /// Triggers the scan.
        /// </summary>
        /// <returns></returns>
        public ScanResultEntity TriggerScan()
        {
            var scanResult = ScanResultEntity.GetRandom();

            InsertScanResult(scanResult);

            return scanResult;
        }

        /// <summary>
        /// Inserts the scan result.
        /// </summary>
        /// <param name="scanResult">The scan result.</param>
        public void InsertScanResult(ScanResultEntity scanResult)
        {
            _connection.Insert(scanResult);
            OnDataHasChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets the scan result models.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ScanResultEntity> GetAllEntities()
        {
            return _connection.Table<ScanResultEntity>().OrderByDescending(x => x.ScanDate)
                .ToList() ?? new List<ScanResultEntity>();
        }

        /// <summary>
        /// Clears the scan history.
        /// </summary>
        public void DeleteAll()
        {
            _connection.DeleteAll<ScanResultEntity>();
            OnDataHasChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Archives the data.
        /// </summary>
        public void ArchiveData()
        {
            var scans = GetAllEntities().ToList();

            bool? lastState = null;
            int totalProcess = 0;

            // supprimer tous les scans qui ont le même état que le précédent
            foreach (var scan in scans)
            {
                if (totalProcess > 30
                    && lastState != null
                    && scan.IsLocalEmpty == lastState)
                {
                    _connection.Delete(scan);
                }

                // Mettre à jour lastState avec l'état actuel
                lastState = scan.IsLocalEmpty;
                totalProcess++;
            }

            // Supprimer les plus anciens scans si on a plus de 1000 scan
            scans = GetAllEntities().ToList();
            
            if (scans.Count > 1000)
            {
                var scansToDelete = scans.Skip(1000).ToList();

                foreach (var scan in scansToDelete)
                {
                    _connection.Delete(scan);
                }
            }
            
            OnDataHasChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
