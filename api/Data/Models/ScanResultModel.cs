namespace RoomScannerWeb.Data.Models
{
    public class ScanResultModel
    {
        public ScanResultModel(bool isLocalEmpty, int maxDistanceVariation)
        {
            var random = new Random();
            Id = random.Next();
            ScanDate = DateTime.Now;
            MaxDistanceVariation = maxDistanceVariation;
            IsLocalEmpty = isLocalEmpty;
        }

        public ScanResultModel(ScanResultPost postedModel) : this(postedModel.IsLocalEmpty, postedModel.MaximalVariationDistance)
        {

        }

        public int Id { get; set; }
        public bool IsLocalEmpty { get; set; }
        public DateTime ScanDate { get; set; }
        public int MaxDistanceVariation { get; set; }

        public static ScanResultModel GetRandom()
        {
            var random = new Random();

            var instance = new ScanResultModel(random.Next() > int.MaxValue / 2, 0);

            return instance;
        }
    }
}
