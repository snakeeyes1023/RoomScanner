using RoomScannerWeb.Data.Entitites;

namespace RoomScannerWeb.Data.Services
{
    public interface IScanService
    {
        event Action OnDataHasChanged;
        event Action OnInstrusionsHasChanged;

        IEnumerable<ScanResultEntity> GetAllScanResultEntities();
        IEnumerable<ScanIntrusionEntity> GetAllIntrusionEntities();

        void InsertScanResult(ScanResultEntity scanResult);
        void InsertIntrusion(ScanIntrusionEntity intrusion);

        void DeleteAll();

        Task<string> TriggerScan();
    }
}