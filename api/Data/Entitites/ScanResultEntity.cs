using RoomScannerWeb.Data.Models;
using SQLite;

namespace RoomScannerWeb.Data.Entitites
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

        public ScanResultEntity(int maxDistanceVariation, bool isLocalEmpty)
        {
            ScanDate = DateTime.Now;
            MaxDistanceVariation = maxDistanceVariation;
            IsLocalEmpty = isLocalEmpty;
        }

        public ScanResultEntity(ScanResultPost postedModel, bool isEmpty) : this(postedModel.MaximalVariationDistance, isEmpty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanResultEntity"/> class. [Seulement pour SQLite]
        /// </summary>
        public ScanResultEntity() { }

        #endregion
    }
}
