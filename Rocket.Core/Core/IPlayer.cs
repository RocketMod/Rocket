using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.Core
{
    public interface IPlayer : IComparable<IPlayer>
    {
        string UniqueID { get; }
        string DisplayName { get; }
        string IsAdmin { get; }
    }
}
