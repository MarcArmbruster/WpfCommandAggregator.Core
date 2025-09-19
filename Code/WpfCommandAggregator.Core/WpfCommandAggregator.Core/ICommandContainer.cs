namespace WpfCommandAggregator.Core;

using System.Collections.Generic;
using System.Windows.Input;

/// <summary>
/// Interface for a container encepsulating the command itself and additional settings.   
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
///   <description>Nov/07/2019</description>
///   </item>
///   <item>
///   <term><b>Remarks:</b></term>
///   <description>Initial version.</description>
///   </item>
///   </list>
/// </remarks>
public interface ICommandContainer
{
    /// <summary>
    /// Gets or sets the command itself.
    /// </summary>
    ICommand Command { get; set; }

    /// <summary>
    /// Gets the dictionary of command settings.
    /// </summary>
    Dictionary<string, object?> Settings { get; }

    /// <summary>
    /// Indexer access to the settings.
    /// </summary>
    /// <param name="settingKey"></param>
    /// <returns>The value of the setting.</returns>
    object? this[string settingKey] { get; }
}