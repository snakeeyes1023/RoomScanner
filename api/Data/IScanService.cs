using RoomScannerWeb.Data.Models;

namespace RoomScannerWeb.Data
{
    public interface IScanService
    {
        event EventHandler? OnDataHasChanged;

        void InsertScanResult(ScanResultEntity scanResult);
        ScanResultEntity TriggerScan();
        IEnumerable<ScanResultEntity> GetAllEntities();
        void DeleteAll();
    }
}