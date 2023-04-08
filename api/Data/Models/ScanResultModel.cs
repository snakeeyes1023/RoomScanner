using SQLite;

namespace RoomScannerWeb.Data.Models
{
    public class ScanResultEntity
    {
        #region Props 
        
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public bool IsLocalEmpty { get; set; }
        public DateTime ScanDate { get; set; }
        public int MaxDistanceVariation { get; set; }

        #endregion

        #region Contructors
        
        public ScanResultEntity(bool isLocalEmpty, int maxDistanceVariation)
        {
            ScanDate = DateTime.Now;
            MaxDistanceVariation = maxDistanceVariation;
            IsLocalEmpty = isLocalEmpty;
        }

        public ScanResultEntity(ScanResultPost postedModel) : this(postedModel.IsLocalEmpty, postedModel.MaximalVariationDistance) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanResultEntity"/> class. [Seulement pour SQLite]
        /// </summary>
        public ScanResultEntity() { }

        #endregion

        public static ScanResultEntity GetRandom()
        {
            var random = new Random();

            var instance = new ScanResultEntity(random.Next() > int.MaxValue / 2, 0);

            return instance;
        }
    }
}
