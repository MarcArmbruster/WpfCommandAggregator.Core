namespace WpfCommandAggregator.Core
{
    using System.Collections.Generic;
    using System.Windows.Input;

    /// <summary>
    /// Container encepsulating the command itself and additional settings.   
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
    public class CommandContainer : ICommandContainer
    {
        /// <summary>
        /// Gets or sets the command itself.
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// Gets the dictionary of command settings.
        /// </summary>
        public Dictionary<string, object> Settings { get; private set; } = new Dictionary<string, object>();

        /// <summary>
        /// The constructor of the command container.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandContainer(ICommand command) : this(command, null)
        {
        }

        /// <summary>
        /// The constructor of the command container.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="settings">The optional settings.</param>
        public CommandContainer(ICommand command, Dictionary<string, object> settings)
        {
            this.Command = command;
            if (settings != null)
            {
                this.Settings = settings;
            }
        }

        /// <summary>
        /// Indexer access to the settings.
        /// </summary>
        /// <param name="settingKey"></param>
        /// <returns>The value of the setting.</returns>
        public object this[string settingKey]
        {
            get
            {
                if (this.Settings.ContainsKey(settingKey))
                {
                    return this.Settings[settingKey];
                }

                return null;
            }
            
        }
    }
}