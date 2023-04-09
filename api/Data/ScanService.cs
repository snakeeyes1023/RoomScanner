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

            if (!scans.Any()) return;

            DeleteExcessScans(scans);

            scans = GetAllEntities().Skip(30).ToList();

            DeleteConsecutiveScansWithSameState(scans);

            OnDataHasChanged?.Invoke(this, EventArgs.Empty);
        }

        private void DeleteExcessScans(List<ScanResultEntity> scans)
        {
            if (scans.Count <= 1000) return;

            var scansToDelete = scans.Skip(1000).ToList();
            foreach (var scan in scansToDelete) _connection.Delete(scan);
        }

        private void DeleteConsecutiveScansWithSameState(List<ScanResultEntity> scans)
        {
            bool? lastState = null;
            foreach (var scan in scans)
            {
                if (lastState != null && scan.IsLocalEmpty == lastState) _connection.Delete(scan);
                lastState = scan.IsLocalEmpty;
            }
        }
    }
}
