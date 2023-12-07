namespace WpfCommandAggregator.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// Command Aggregator class.
    /// Maintaining a bag of ICommand objects, identified by a string key including get and set methods, added by an indexer access (for easy binding usage).
    /// GetMethod never returns null. If command key does not exist or command is null a dummy command/delegate is returned (doing nothing).
    /// This avoids null reference exceptions.
    /// The method 'Exists' can be used to check for the availability of a command.
    /// The method 'HasNullCommand' can be used for an explicit check for null command.
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
    public class CommandAggregator : ICommandAggregator
    {
        #region Private Members

        /// <summary>
        /// The private (thread save) collection of command containers.
        /// </summary>
        private readonly ConcurrentDictionary<string, ICommandContainer> commandContainers;

        #endregion Private Members

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAggregator"/> class.
        /// </summary>
        public CommandAggregator()
        {
            this.commandContainers = new ConcurrentDictionary<string, ICommandContainer>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAggregator"/> class.
        /// </summary>
        /// <param name="commandContainers">The command containerss.</param>
        public CommandAggregator(IEnumerable<KeyValuePair<string, ICommandContainer>> commandContainers) : this()
        {
            if (this.commandContainers != null)
            {
                foreach (KeyValuePair<string, ICommandContainer> item in commandContainers.Where(i => i.Key != null && i.Value != null))
                {
                    this.AddOrSetCommand(item.Key, item.Value);
                }
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="CommandAggregator"/> class.
        /// </summary>
        ~CommandAggregator()
        {
            this.commandContainers.Clear();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the <see cref="ICommand"/> with the specified key.
        /// Indexer access is important for usage in XAML DataBindings.
        /// Please Note: indexers can only be used for OneWay-Mode in
        /// XAML-Bindings (read only).
        /// </summary>
        /// <value>
        /// The <see cref="ICommand"/>.
        /// </value>
        /// <param name="key">The command key.</param>
        /// <returns>The command for the given key (Empty command if not found/exists).</returns>
        [JsonIgnore]
        public ICommandContainer this[string key] => this.GetCommandContainer(key);

        /// <summary>
        /// Gets a value indicating whether this instance has any command container.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has any command; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore] 
        public bool HasAnyCommandContainer => this.commandContainers.Any();

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds or set the command.
        /// </summary>
        /// <param name="key">The command key.</param>
        /// <param name="command">The command.</param>
        public void AddOrSetCommand(string key, ICommandContainer command)
        {
            if (string.IsNullOrEmpty(key) == false)
            {
                if (this.commandContainers.Any(k => k.Key == key))
                {
                    this.commandContainers[key] = command;
                }
                else
                {
                    this.commandContainers.AddOrUpdate(key, command, (exkey, excmd) => command);
                }
            }
        }

        /// <summary>
        /// Adds or set the command.
        /// </summary>
        /// <param name="key">The command key.</param>
        /// <param name="command">The command.</param>
        public void AddOrSetCommand(string key, ICommand command)
        {
            if (string.IsNullOrEmpty(key) == false)
            {
                if (this.commandContainers.Any(k => k.Key == key))
                {
                    this.commandContainers[key] = new CommandContainer(command);
                }
                else
                {
                    var commandDefinition = new CommandContainer(command);
                    this.commandContainers.AddOrUpdate(key, commandDefinition, (exkey, excmd) => commandDefinition);
                }
            }
        }

        /// <summary>
        /// Adds or set the command.
        /// </summary>
        /// <param name="key">The command key.</param>
        /// <param name="command">The command.</param>
        /// <param name="settings">The command settings.</param>
        public void AddOrSetCommand(string key, ICommand command, Dictionary<string, object> settings)
        {
            if (string.IsNullOrEmpty(key) == false)
            {
                if (this.commandContainers.Any(k => k.Key == key))
                {
                    this.commandContainers[key] = new CommandContainer(command, settings);
                }
                else
                {
                    var commandDefinition = new CommandContainer(command, settings);
                    this.commandContainers.AddOrUpdate(key, commandDefinition, (exkey, excmd) => commandDefinition);
                }
            }
        }

        /// <summary>
        /// Adds or set the command.
        /// </summary>
        /// <param name="key">The command key.</param>
        /// <param name="executeDelegate">The execute delegate.</param>
        /// <param name="canExecuteDelegate">The can execute delegate.</param>
        public void AddOrSetCommand(string key, Action<object> executeDelegate, Predicate<object> canExecuteDelegate)
        {
            if (string.IsNullOrEmpty(key) == false)
            {
                if (this.commandContainers.Any(k => k.Key == key))
                {
                    ICommandContainer commandDefinition 
                        = new CommandContainer(new RelayCommand(executeDelegate, canExecuteDelegate));
                    
                    this.commandContainers[key] = commandDefinition;
                }
                else
                {
                    ICommandContainer commandDefinition 
                        = new CommandContainer(new RelayCommand(executeDelegate, canExecuteDelegate));
                    
                    this.commandContainers.AddOrUpdate(key, commandDefinition, (exkey, excmd) => commandDefinition);
                }
            }
        }

        /// <summary>
        /// Adds or set the command.
        /// </summary>
        /// <param name="key">The command key.</param>
        /// <param name="executeDelegate">The execute delegate.</param>
        /// <param name="canExecuteDelegate">The can execute delegate.</param>
        /// <param name="settings">The command settings.</param>
        public void AddOrSetCommand(string key, Action<object> executeDelegate, Predicate<object> canExecuteDelegate, Dictionary<string, object> settings)
        {
            if (string.IsNullOrEmpty(key) == false)
            {
                if (this.commandContainers.Any(k => k.Key == key))
                {
                    ICommandContainer commandDefinition
                        = new CommandContainer(new RelayCommand(executeDelegate, canExecuteDelegate), settings);

                    this.commandContainers[key] = commandDefinition;
                }
                else
                {
                    ICommandContainer commandDefinition
                        = new CommandContainer(new RelayCommand(executeDelegate, canExecuteDelegate), settings);

                    this.commandContainers.AddOrUpdate(key, commandDefinition, (exkey, excmd) => commandDefinition);
                }
            }
        }

        /// <summary>
        /// Counts the registered commands.
        /// </summary>
        /// <returns>Number of registered commands.</returns>
        public int Count()
        {
            return this.commandContainers.Count;
        }

        /// <summary>
        /// Executes the command asynchronous.
        /// IMPORTANT: This assumes the possible asynchronous call/execution of the action delegate!
        /// </summary>
        /// <param name="key">The command key.</param>
        /// <param name="parameter">The optional parameter.</param>
        /// <returns>
        /// The created Task.
        /// </returns>
        /// <exception cref="CommandNotDefinedException">Command with such a key is actually not registered.</exception>
        public Task ExecuteAsync(string key, object parameter = null)
        {
            if (this.Exists(key) == false)
            {
                return Task.Factory.StartNew(() => { });
            }

            return Task.Factory.StartNew(() => this.GetCommandContainer(key).Command.Execute(parameter));
        }

        /// <summary>
        /// Checks the existance of a command for the given key.
        /// </summary>
        /// <param name="key">The command key.</param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            return this.commandContainers.Any(k => k.Key == key);
        }

        /// <summary>
        /// Gets the command container. If command container not exists, a ontainer with a dummy Action delegate will be returned (doing nothing).
        /// </summary>
        /// <param name="key">The command key.</param>
        /// <returns>The command for the given key (Empty command if not found/exists).</returns>
        public ICommandContainer GetCommandContainer(string key)
        {
            if (this.commandContainers.Any(k => k.Key == key))
            {
                return this.commandContainers[key];
            }
            else
            {
                // Empty command (to avoid null reference exceptions)
                return new CommandContainer(new RelayCommand(p1 => { }));
            }
        }

        /// <summary>
        /// Determines whether the ICommandContainer corresponding the specified key is null.
        /// </summary>
        /// <param name="key">The command key.</param>
        /// <returns>True if ICommand is null, false otherwise.</returns>
        /// <exception cref="CommandNotDefinedException">Command with this key is not registered, yet.</exception>
        public bool HasNullCommandContainer(string key)
        {
            if (this.Exists(key) == false)
            {
                return false;
            }

            return this.commandContainers.FirstOrDefault(k => k.Key == key).Value == null;
        }

        /// <summary>
        /// Determines whether the ICommandContainer corresponding the specified key is null.
        /// </summary>
        /// <param name="key">The command key.</param>
        /// <returns>True if ICommand is null, false otherwise.</returns>
        /// <exception cref="CommandNotDefinedException">Command with this key is not registered, yet.</exception>
        public bool HasNullCommand(string key)
        {
            if (this.Exists(key) == false)
            {
                return false;
            }

            var container = this.commandContainers.First(k => k.Key == key);
            return container.Value?.Command == null;
        }

        /// <summary>
        /// Removes the ICommand corresponding the specified key.
        /// </summary>
        /// <param name="key">The command key.</param>
        public void Remove(string key)
        {
            if (this.commandContainers.Any(k => k.Key == key))
            {
                ICommandContainer oldCommandDefinition;
                this.commandContainers.TryRemove(key, out oldCommandDefinition);
            }
        }

        /// <summary>
        /// Removes all ICommands = Clear-Functionality.
        /// </summary>
        public void RemoveAll()
        {
            if (this.commandContainers != null)
            {
                this.commandContainers.Clear();
            }
        }
        #endregion Methods
    }
}