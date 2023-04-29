using SQLite;
using Microsoft.Extensions.Options;
using System.Net.Http;
using RoomScannerWeb.Data.Helpers;
using RoomScannerWeb.Data.Entitites;

namespace RoomScannerWeb.Data.Services
{
    /// <summary>
    /// Service pour gérer un scanneur.
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
        /// Initialise une nouvelle instance de la classe <see cref="ScanService"/>.
        /// </summary>
        /// <param name="dbContext">Le contexte de la base de données.</param>
        /// <param name="scanSetting">Les paramètres de configuration du scanneur.</param>
        public ScanService(SQLiteConnection dbContext, IOptions<ScanSetting> scanSetting)
        {
            _connection = dbContext;
            _scanSetting = scanSetting.Value;
        }

        /// <summary>
        /// Déclenche un scan sur le scanneur.
        /// </summary>
        /// <returns>Le corps de la réponse de l'Arduino.</returns>
        public async Task<string> TriggerScan()
        {
            var response = await client.PostAsync($"http://{_scanSetting.Ip}/trigger-scan", null);

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Insère un résultat de scan dans la base de données.
        /// </summary>
        /// <param name="scanResult">Le résultat du scan.</param>
        public void InsertScanResult(ScanResultEntity scanResult)
        {
            _connection.Insert(scanResult);
            OnDataHasChanged?.Invoke();
        }

        /// <summary>
        /// Insère une intrusion dans la base de données.
        /// </summary>
        /// <param name="intrusion">L'intrusion.</param>
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
        /// Obtient toutes les intrusions enregistrées dans la base de données, triées par date décroissante.
        /// </summary>
        /// <returns>Les intrusions enregistrées dans la base de données.</returns>
        public IEnumerable<ScanIntrusionEntity> GetAllIntrusionEntities()
        {
            return _connection.Table<ScanIntrusionEntity>().OrderByDescending(x => x.IntrusionDate)
                .ToList() ?? new List<ScanIntrusionEntity>();
        }

        /// <summary>
        /// Obtient tous les résultats de scan enregistrés dans la base de données, triés par date décroissante.
        /// </summary>
        /// <returns>Les résultats de scan enregistrés dans la base de données.</returns>
        public IEnumerable<ScanResultEntity> GetAllScanResultEntities()
        {
            return _connection.Table<ScanResultEntity>().OrderByDescending(x => x.ScanDate)
                .ToList() ?? new List<ScanResultEntity>();
        }


        /// <summary>
        /// Supprimer tous les entitées liés au scan dans la base de donnée
        /// </summary>
        public void DeleteAll()
        {
            _connection.DeleteAll<ScanResultEntity>();
            _connection.DeleteAll<ScanIntrusionEntity>();
            
            OnDataHasChanged?.Invoke();
            OnInstrusionsHasChanged?.Invoke();
        }

        /// <summary>
        /// Archive les données actuelle pour libérer de la place
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

        /// <summary>
        /// Garde seulement les 1000 premier scan
        /// </summary>
        /// <param name="scans">Les scans.</param>
        private void DeleteExcessScans(List<ScanResultEntity> scans)
        {
            if (scans.Count <= 1000) return;

            var scansToDelete = scans.Skip(1000).ToList();
            foreach (var scan in scansToDelete) _connection.Delete(scan);
        }

        /// <summary>
        /// Supprime les scans collés avec le même status 
        /// </summary>
        /// <param name="scans">Les scans.</param>
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
