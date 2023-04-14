using SQLite;

namespace RoomScannerWeb.Data.Entitites
{
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
