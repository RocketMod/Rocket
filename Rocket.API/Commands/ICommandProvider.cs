using System.Collections.Generic;

namespace Rocket.API.Commands {
    public interface ICommandProvider {
        IEnumerable<ICommand> Commands { get; }
    }
}