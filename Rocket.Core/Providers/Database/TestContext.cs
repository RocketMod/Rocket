using System.Data.Linq;
using System.Data.Linq.Mapping;
using Rocket.API.Providers.Database;

namespace Rocket.Core.Providers.Database
{
    //A database context contains all your tables
    public class TestContext : DatabaseContext
    {
        public TestContext(IDatabaseProvider provider) : base(provider)
        {

        }

        //These are your tables
        public virtual Table<PlayerStatistics> PlayerStatistics => GetTable<PlayerStatistics>();
        public virtual Table<VehicleStatistics> VehicleStatistics => GetTable<VehicleStatistics>();
        public override void OnDatabaseCreated()
        {
            
        }
    }

    [Table(Name = "VehicleStats")] // You dont need to include Name, it will get from class if you dont
    public class VehicleStatistics
    {
        [Column]
        public virtual string VehicleName { get; }

        [Column(IsPrimaryKey = true, Name = "Vehicle_ID")]
        public virtual string VehicleID { get; }

        [Column]
        public virtual int TotalDriven { get; }
    }

    [Table]
    public class PlayerStatistics
    {
        [Column]
        public virtual string PlayerName { get; }

        [Column(IsPrimaryKey = true, Name = "Player_ID")] // IsPrimary makes also unique key
        public virtual string PlayerID { get; }

        [Column(DbType = "INT(4) NOT NULL")] // you can specify db type manually
        public virtual int MetersWalked { get; }

        [Column]
        public virtual int Kills { get; }

        [Column]
        public virtual int Deaths { get; }
    }
}