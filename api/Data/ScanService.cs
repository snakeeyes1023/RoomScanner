using RoomScannerWeb.Data.Models;
using SQLite;

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
            // keep only the last 100 scans
            var scans = GetAllEntities().ToList();

            if (scans.Count > 100)
            {
                var scansToDelete = scans.Skip(100).ToList();
                foreach (var scan in scansToDelete)
                {
                    _connection.Delete(scan);
                }
            }

            OnDataHasChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
