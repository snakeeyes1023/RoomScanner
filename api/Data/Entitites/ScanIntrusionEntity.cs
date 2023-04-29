using SQLite;

namespace RoomScannerWeb.Data.Entitites
{
    /// <summary>
    /// Entité pour les intrusions dans la boîte du scan
    /// </summary>
    public class ScanIntrusionEntity
    {
        #region Props

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime IntrusionDate { get; set; }

        #endregion


        #region Contructors

        public ScanIntrusionEntity(DateTime intrusionDate)
        {
            IntrusionDate = intrusionDate;
        }

        public ScanIntrusionEntity()
        {
            
        }

        #endregion
    }
}
