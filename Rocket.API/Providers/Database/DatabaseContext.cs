using System.ComponentModel;
using System.Data.Linq;

namespace Rocket.API.Providers.Database
{
    public class DatabaseContext : DataContext, INotifyPropertyChanging, INotifyPropertyChanged
    {
        public IDatabaseProvider Provider { get; }

        public DatabaseContext(IDatabaseProvider provider) : base(provider.Connection)
        {
            Provider = provider;
        }

        //Todo:
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}