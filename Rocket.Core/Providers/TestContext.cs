using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
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

        public PlayerStatistics GetStatsForPlayer(string playerID)
        {
            return PlayerStatistics.FirstOrDefault(c => c.PlayerID == playerID); //linq example
        }
    }

    [Table(Name = "VehicleStats")] // You dont need to include Name, it will get from class if you dont
    public class VehicleStatistics : DatabaseTable
    {
        [Column]
        public virtual string VehicleName
        {
            set { Set(value, nameof(VehicleName)); }
            get { return Get<string>(nameof(VehicleName)); }
        }

        [Column(IsPrimaryKey = true, Name = "Vehicle_ID")]
        public virtual ushort VehicleID
        {
            set { Set(value, nameof(VehicleID)); }
            get { return Get<ushort>(nameof(VehicleID)); }
        }

        [Column]
        public virtual int TotalDriven
        {
            set { Set(value, nameof(TotalDriven)); }
            get { return Get<int>(nameof(TotalDriven)); }
        }
    }

    [Table]
    public class PlayerStatistics : DatabaseTable
    {
        [Column]
        public virtual string PlayerName
        {
            set { Set(value, nameof(PlayerName)); }
            get { return Get<string>(nameof(PlayerName)); }
        }

        [Column(IsPrimaryKey = true, Name = "Player_ID")] // IsPrimary makes also unique key
        public virtual string PlayerID
        {
            set { Set(value, nameof(PlayerID)); }
            get { return Get<string>(nameof(PlayerID)); }
        }

        [Column(DbType = "INT(4) NOT NULL")] // you can specify db type manually
        public virtual int MetersWalked
        {
            set { Set(value, nameof(MetersWalked));}
            get { return Get<int>(nameof(MetersWalked)); }
        }

        [Column]
        public virtual int Kills
        {
            set { Set(value, nameof(Kills)); }
            get { return Get<int>(nameof(Kills)); }
        }

        [Column]
        public virtual int Deaths
        {
            set { Set(value, nameof(Deaths)); }
            get { return Get<int>(nameof(Deaths)); }
        }
    }
}