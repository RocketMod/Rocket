using System;

namespace Rocket.API.Commands
{
    /// <summary>
    ///     A sub command. See <see cref="ICommand.ChildCommands" />.
    ///     <para>Will not be automatically registered, must be returned by <see cref="ICommand.ChildCommands" />.</para>
    /// </summary>
    public interface ISubCommand : ICommand { }

    /// <summary>
    ///     A sub command. See <see cref="ICommand.ChildCommands" />.
    ///     <para>Will be automatically registered to <i>T</i> as child command.</para>
    /// </summary>
    /// <typeparam name="T">The parent command.</typeparam>
    [Obsolete("Not implemented yet.")] //todo
    public interface ISubCommand<T> : ISubCommand { }
}