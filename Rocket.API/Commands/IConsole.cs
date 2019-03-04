using System;
using System.Drawing;
using Rocket.API.Logging;
using Rocket.API.Player;
using Rocket.API.User;
using Color = System.Drawing.Color;

namespace Rocket.API.Commands
{
    /// <summary>
    ///     This <see cref="IUser">user</see> is used when executing commands from console.
    ///     <para>
    ///         Altough plugins could use it to call commands programatically, it is recommended that they implement their own
    ///         user.
    ///     </para>
    /// </summary>
    public interface IConsole : IUser
    {
        /// <inheritdoc cref="Console.WriteLine(string, object[])" />
        /// <param name="format">The format.</param>
        /// <param name="bindings">The bindings.</param>
        void WriteLine(string format, params object[] bindings);

        /// <inheritdoc cref="Console.WriteLine(string, object[])" />
        /// <param name="level">The log level.</param>
        /// <param name="format">The format.</param>
        /// <param name="bindings">The bindings.</param>
        void WriteLine(LogLevel level, string format, params object[] bindings);

        /// <inheritdoc cref="Console.WriteLine(string, object[])" />
        /// <param name="color">The color to use.</param>
        /// <param name="level">The log level.</param>
        /// <param name="format">The format.</param>
        /// <param name="bindings">The bindings.</param>
        void WriteLine(LogLevel level, string format, Color? color = null, params object[] bindings);

        /// <inheritdoc cref="Console.WriteLine(string, object[])" />
        /// <param name="color">The color to use.</param>
        /// <param name="format">The format.</param>
        /// <param name="bindings">The bindings.</param>
        void WriteLine(string format, Color? color = null, params object[] bindings);

        /// <inheritdoc cref="Console.Write(string, object[])" />
        /// <param name="color">The color to use.</param>
        /// <param name="format">The format.</param>
        /// <param name="bindings">The bindings.</param>
        void Write(string format, Color? color = null, params object[] bindings);

        /// <inheritdoc cref="Console.Write(string, object[])" />
        void Write(string format, params object[] bindings);
    }
}