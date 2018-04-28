using System;
using System.Collections.Generic;

namespace Rocket.API.Commands
{
    /// <summary>
    ///     Parameters of a command.
    /// </summary>
    /// <remarks>
    ///     When a command was entered as "/mycommand test 5 b", this class will handle and represent "test", "a" and "b".
    /// </remarks>
    public interface ICommandParameters : IEnumerable<string>
    {
        /// <summary>
        ///     Returns the n. command parameter.<br />
        ///     Index must be less than <see cref="Length">length</see> and not negative.<br /><br />
        ///     This property will never return null.
        ///     <seealso cref="P:System.Collections.ArrayList.Item(System.Int32)" />
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     when <i>index</i> is equal or greater than
        ///     <see cref="Length">length</see> or negative.
        /// </exception>
        /// <param name="index">The zero-based index of the parameter.</param>
        /// <returns></returns>
        string this[int index] { get; }

        /// <summary>
        ///     The length (amount) of the parameters.
        ///     <br /><br />
        ///     <b>Example:</b>
        ///     If the command was entered as "/mycommand test 5 b", it would return "3".
        /// </summary>
        int Length { get; }

        /// <summary>
        ///     Gets the parameter value at the given index. The value will parsed to the given type. <br />
        ///     Types like <i>IPlayer</i>, <i>IOnlinePlayer</i>, etc. are supported.
        ///     <br /><br />
        ///     <b>Example:</b>
        ///     Assume the command was entered as "/mycommand test 5 b". <br />
        ///     <code>Get&lt;string&gt;(0)</code> would be equal to "test" (string). <br />
        ///     <code>Get&lt;int&gt;(1)</code> would be equal to 5 (int). <br />
        ///     <code>Get&lt;string&gt;(1)</code> would be equal to "5" (string). <br />
        ///     <code>Get&lt;string&gt;(2)</code> would be equal to "b" (string).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     when <i>index</i> is equal or greater than
        ///     <see cref="Length">length</see> or negative.
        /// </exception>
        /// <typeparam name="T">The type to parse the parameter as.</typeparam>
        /// <param name="index">The zero-based parameter index.</param>
        /// <returns>The parsed parameter value.</returns>
        T Get<T>(int index);

        /// <summary>
        ///     <inheritdoc cref="Get{T}(int)" />
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <inheritdoc cref="Get{T}(int)" />
        /// </exception>
        /// <param name="index">
        ///     <inheritdoc cref="Get{T}(int)" />
        /// </param>
        /// <param name="type">The type to parse the parameter as.</param>
        /// <returns>
        ///     <inheritdoc cref="Get{T}(int)" />
        /// </returns>
        object Get(int index, Type type);

        /// <summary>
        ///     <inheritdoc cref="Get{T}(int)" />
        /// </summary>
        /// <typeparam name="T">
        ///     <inheritdoc cref="Get{T}(int)" />
        /// </typeparam>
        /// <param name="index">
        ///     <inheritdoc cref="Get{T}(int)" />
        /// </param>
        /// <param name="defaultValue">The default return value.</param>
        /// <returns>
        ///     the parsed parameter value if the given index was valid and the parameter could be parsed to the given type;
        ///     otherwise <i>defaultValue</i>.
        /// </returns>
        T Get<T>(int index, T defaultValue);

        /// <summary>
        ///     <inheritdoc cref="Get{T}(int, T)" />
        /// </summary>
        /// <param name="index">
        ///     <inheritdoc cref="Get{T}(int, T)" />
        /// </param>
        /// <param name="type">The type to parse the parameters as.</param>
        /// <param name="defaultValue">The default return value.</param>
        /// <returns>
        ///     <inheritdoc cref="Get{T}(int, T)" />
        /// </returns>
        object Get(int index, Type type, object defaultValue);

        /// <summary>
        ///     Tries to get and parse a parameter. See <see cref="Get{T}(int)" />.
        /// </summary>
        /// <typeparam name="T">
        ///     <inheritdoc cref="Get{T}(int)" />
        /// </typeparam>
        /// <param name="index">
        ///     <inheritdoc cref="Get{T}(int)" />
        /// </param>
        /// <param name="value">The parsed parameter value.</param>
        /// <returns>
        ///     <b>true</b> if the given index was valid and the parameter could be parsed to the given type; otherwise
        ///     <b>false</b>.
        /// </returns>
        bool TryGet<T>(int index, out T value);

        /// <summary>
        ///     <inheritdoc cref="TryGet{T}" />
        /// </summary>
        /// <param name="index">
        ///     <inheritdoc cref="TryGet{T}" />
        /// </param>
        /// <param name="type">The type to parse the parameters as.</param>
        /// <param name="value">The parsed parameter value.</param>
        /// <returns>
        ///     <inheritdoc cref="TryGet{T}" />
        /// </returns>
        bool TryGet(int index, Type type, out object value);

        /// <summary>
        ///     Gets the parameters as string array.
        /// </summary>
        /// <returns>the parameters as string array.</returns>
        string[] ToArray();

        /// <summary>
        ///     Gets the parameters as string list.
        /// </summary>
        /// <returns>the parameters as string list.</returns>
        List<string> ToList();
    }
}