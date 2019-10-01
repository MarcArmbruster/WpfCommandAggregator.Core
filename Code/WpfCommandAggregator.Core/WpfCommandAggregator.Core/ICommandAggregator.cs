namespace WpfCommandAggregator.Core
{
    using System;
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
        ICommand this[string key] { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has any command.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has any command; otherwise, <c>false</c>.
        /// </value>
        bool HasAnyCommand { get; }

        /// <summary>
        /// Adds the or set command.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="command">The command.</param>
        void AddOrSetCommand(string key, ICommand command);

        /// <summary>
        /// Adds the or set command.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="executeDelegate">The execute delegate.</param>
        /// <param name="canExecuteDelegate">The can execute delegate.</param>
        void AddOrSetCommand(string key, Action<object> executeDelegate, Predicate<object> canExecuteDelegate);

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
        Task ExecuteAsync(string key, object parameter = null);

        /// <summary>
        /// Checks if a command exists.
        /// </summary>
        /// <param name="key">The key.</param>
        bool Exists(string key);

        /// <summary>
        /// Gets the command identified by the given key.
        /// </summary>
        /// <param name="key">The command key.</param>
        /// <returns>The command.</returns>
        ICommand GetCommand(string key);

        /// <summary>
        /// Determines whether key points to a null command.
        /// </summary>
        /// <param name="key">The key.</param>
        bool HasNullCommand(string key);

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        void Remove(string key);

        /// <summary>
        /// Removes all commands.
        /// </summary>
        void RemoveAll();
    }
}