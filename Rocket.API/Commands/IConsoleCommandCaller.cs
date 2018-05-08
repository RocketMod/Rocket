using Rocket.API.User;
using System.Drawing;

namespace Rocket.API.Commands
{
    /// <summary>
    ///     This <see cref="IUser">command caller</see> is used when executing commands from console.
    ///     <para>Altough plugins could use it to call commands programatically, it is recommended that they implement their own
    ///     command caller.</para>
    /// </summary>
    public interface IConsole : IUser 
    {
        void WriteLine(string format, Color color, object[] bindings);
        void Write(string format, Color color, object[] bindings);
    }
}