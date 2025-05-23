﻿namespace WpfCommandAggregator.Core;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Input;

/// <summary>
/// Command Aggregator interface.   
/// </summary>
/// <remarks>
///   <para><b>History</b></para>
///   <list type="table">
///   <item>
///   <term><b>Author:</b></term>
///   <description>Marc Armbruster</description>
///   </item>
///   <item>
///   <term><b>Date:</b></term>
///   <description>Oct/01/2019</description>
///   </item>
///   <item>
///   <term><b>Remarks:</b></term>
///   <description>Initial version.</description>
///   </item>
///   </list>
/// </remarks>
public interface ICommandAggregator
{
    /// <summary>
    /// Gets the <see cref="ICommand"/> with the specified key.
    /// </summary>
    /// <value>
    /// The <see cref="ICommand"/>.
    /// </value>
    /// <param name="key">The key.</param>
    /// <returns>The command.</returns>
    ICommandContainer this[string key] { get; }

    /// <summary>
    /// Gets a value indicating whether this instance has any command container.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance has any command container; otherwise, <c>false</c>.
    /// </value>
    bool HasAnyCommandContainer { get; }

    /// <summary>
    /// Adds the or set command definition.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="command container">The command container.</param>
    void AddOrSetCommand([NotNull] string key, ICommandContainer? commandContainer);

    /// <summary>
    /// Adds the or set command.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="command">The command.</param>
    void AddOrSetCommand([NotNull] string key, ICommand? command);

    /// <summary>
    /// Adds the or set command.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="command">The command.</param>
    void AddOrSetCommand([NotNull] string key, ICommand? command, Dictionary<string, object?>? settings);

    /// <summary>
    /// Adds the or set command.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="executeDelegate">The execute delegate.</param>
    /// <param name="canExecuteDelegate">The can execute delegate.</param>
    void AddOrSetCommand(
        [NotNull] string key, 
        Action<object?>? executeDelegate, 
        Predicate<object?>? canExecuteDelegate);

    /// <summary>
    /// Adds or set the command.
    /// </summary>
    /// <param name="key">The command key.</param>
    /// <param name="executeDelegate">The execute delegate.</param>
    /// <param name="canExecuteDelegate">The can execute delegate.</param>
    /// <param name="settings">The command settings.</param>
    void AddOrSetCommand(
        [NotNull] string key, 
        Action<object?>? executeDelegate, 
        Predicate<object?>? canExecuteDelegate, 
        Dictionary<string, object?>? settings);

    /// <summary>
    /// Count of registered commands.
    /// </summary>
    /// <returns>The number of registered commands.</returns>
    int Count();

    /// <summary>
    /// Executes the asynchronous.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="parameter">The parameter.</param>
    /// <returns>The task.</returns>
    Task ExecuteAsync([NotNull] string key, object? parameter = null);

    /// <summary>
    /// Checks if a command definition exists.
    /// </summary>
    /// <param name="key">The key.</param>
    bool Exists([NotNull] string key);

    /// <summary>
    /// Gets the command identified by the given key.
    /// </summary>
    /// <param name="key">The command key.</param>
    /// <returns>The command.</returns>
    ICommandContainer GetCommandContainer([NotNull] string key);

    /// <summary>
    /// Determines whether key points to a null command container.
    /// </summary>
    /// <param name="key">The key.</param>
    bool HasNullCommandContainer([NotNull] string key);

    /// <summary>
    /// Determines whether key points to a null command.
    /// </summary>
    /// <param name="key">The key.</param>
    bool HasNullCommand([NotNull] string key);

    /// <summary>
    /// Removes the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    void Remove([NotNull] string key);

    /// <summary>
    /// Removes all commands.
    /// </summary>
    void RemoveAll();
}