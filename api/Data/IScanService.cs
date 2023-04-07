using RoomScannerWeb.Data.Models;

namespace RoomScannerWeb.Data
{
    public interface IScanService
    {
        event EventHandler<ScanResultModel>? OnScanEvent;
        event Action? OnClearEvent;

        void InsertScanResult(ScanResultModel scanResult);
        ScanResultModel TriggerScan();
        IEnumerable<ScanResultModel> GetScanResultModels();
        void ClearScanHistory();
    }
}