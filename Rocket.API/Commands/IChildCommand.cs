using System;

namespace Rocket.API.Commands
{
    /// <summary>
    ///     A child Command. See <see cref="ICommand.ChildCommands" />.
    ///     <para>Will not be automatically registered, must be returned by <see cref="ICommand.ChildCommands" />.</para>
    /// </summary>
    public interface IChildCommand : ICommand { }

    /// <summary>
    ///     A child Command. See <see cref="ICommand.ChildCommands" />.
    ///     <para>Will be automatically registered to <i>T</i> as child command.</para>
    /// </summary>
    /// <typeparam name="T">The parent command.</typeparam>
    [Obsolete("Not implemented yet.")] //todo
    public interface IChildCommand<T> : IChildCommand { }
}