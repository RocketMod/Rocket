using System.Collections.Generic;

namespace Rocket.API
{
    public interface IRocketPlayer
    {
        string Id { get; }
        string DisplayName { get; }
        bool IsAdmin { get; }
    }
}
