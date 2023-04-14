using SQLite;
using Microsoft.Extensions.Options;
using System.Net.Http;
using RoomScannerWeb.Data.Helpers;
using RoomScannerWeb.Data.Entitites;

namespace RoomScannerWeb.Data.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IScanService" />
    public class ScanService : IScanService
    {
        public event Action? OnDataHasChanged;
        public event Action? OnInstrusionsHasChanged;


        private readonly SQLiteConnection _connection;
        private readonly ScanSetting _scanSetting;

        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanService"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public ScanService(SQLiteConnection dbContext, IOptions<ScanSetting> scanSetting)
        {
            _connection = dbContext;
            _scanSetting = scanSetting.Value;
        }

        /// <summary>
        /// Triggers the scan.
        /// </summary>
        /// <returns>Respond body from arduino</returns>
        public async Task<string> TriggerScan()
        {
            var response = await client.PostAsync($"http://{_scanSetting.Ip}/trigger-scan", null);

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Inserts the scan result.
        /// </summary>
        /// <param name="scanResult">The scan result.</param>
        public void InsertScanResult(ScanResultEntity scanResult)
        {
            _connection.Insert(scanResult);
            OnDataHasChanged?.Invoke();
        }

        /// <summary>
        /// Inserts the intrusion.
        /// </summary>
        /// <param name="intrusion">The intrusion.</param>
        public void InsertIntrusion(ScanIntrusionEntity intrusion)
        {
            _connection.Insert(intrusion);

            if (GetAllIntrusionEntities() is IEnumerable<ScanIntrusionEntity> intrusions 
                && intrusions.Count() > 5)
            {
                _connection.Delete(intrusions.Last());
            }

            OnInstrusionsHasChanged?.Invoke();
        }


        /// <summary>
        /// Gets the scan result models.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ScanIntrusionEntity> GetAllIntrusionEntities()
        {
            return _connection.Table<ScanIntrusionEntity>().OrderByDescending(x => x.IntrusionDate)
                .ToList() ?? new List<ScanIntrusionEntity>();
        }

        /// <summary>
        /// Gets the scan result models.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ScanResultEntity> GetAllScanResultEntities()
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
            _connection.DeleteAll<ScanIntrusionEntity>();
            
            OnDataHasChanged?.Invoke();
            OnInstrusionsHasChanged?.Invoke();
        }

        /// <summary>
        /// Archives the data.
        /// </summary>
        public void ArchiveData()
        {
            var scans = GetAllScanResultEntities().ToList();

            if (!scans.Any()) return;

            DeleteExcessScans(scans);

            scans = GetAllScanResultEntities().Skip(30).ToList();

            DeleteConsecutiveScansWithSameState(scans);

            OnDataHasChanged?.Invoke();
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
