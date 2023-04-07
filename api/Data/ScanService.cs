using RoomScannerWeb.Data.Models;

namespace RoomScannerWeb.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class ScanService : IScanService
    {
        public event EventHandler<ScanResultModel>? OnScanEvent;
        public event Action? OnClearEvent;
        
        
        private readonly List<ScanResultModel> _scanHistories;

        public ScanService()
        {
            _scanHistories = new List<ScanResultModel>();
        }

        public ScanResultModel TriggerScan()
        {
            var scanResult = ScanResultModel.GetRandom();

            InsertScanResult(scanResult);

            return scanResult;
        }

        public void InsertScanResult(ScanResultModel scanResult)
        {
            _scanHistories.Add(scanResult);
            OnScanEvent?.Invoke(this, scanResult);
        }

        public IEnumerable<ScanResultModel> GetScanResultModels()
        {
            return _scanHistories;
        }

        public void ClearScanHistory()
        {
            _scanHistories.Clear();
            OnClearEvent?.Invoke();
        }
    }
}
