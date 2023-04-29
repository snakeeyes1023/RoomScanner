namespace RoomScannerWeb.Data.Helpers
{
    /// <summary>
    /// Configuration du scan
    /// </summary>
    public class ScanSetting
    {
        public string Ip { get; set; } // ip du scan
        public float VariationOffset { get; set; } // à partir de quel variation on considère qu'il y a un changement dans la pièce
    }
}
