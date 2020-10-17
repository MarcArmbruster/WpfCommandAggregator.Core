namespace WpfCommandAggregator.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Factory class for new command aggregator instances.
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
    public class CommandAggregatorFactory
    {
        /// <summary>
        /// Type of the optional external command aggregator type.
        /// </summary>
        private static Type externalAggregatorType = null;

        /// <summary>
        /// Registers the given, custom command aggregator type.
        /// </summary>
        /// <typeparam name="T">Type of custom aggregator implementation.</typeparam>        
        public static void RegisterAggreagtorImplementation<T>() where T : ICommandAggregator
        {
            externalAggregatorType = typeof(T);
        }

        /// <summary>
        /// Unregister the given, custom command aggregator type.
        /// </summary>
        /// <typeparam name="T">Type of own aggregator implementation.</typeparam>
        public static void UnregisterAggreagtorImplementation<T>() where T : ICommandAggregator
        {
            externalAggregatorType = null;
        }

        /// <summary>
        /// Clears any custom aggregator registration.
        /// </summary>
        public static void ClearRegistration()
        {
            externalAggregatorType = null;
        }

        /// <summary>
        /// Gets the new command aggregator.
        /// </summary>
        /// <returns></returns>
        public static ICommandAggregator GetNewCommandAggregator()
        {
            if (externalAggregatorType != null)
            {
                ICommandAggregator aggregator = Activator.CreateInstance(externalAggregatorType) as ICommandAggregator;
                if (aggregator == null)
                {
                    throw new InvalidCastException("The registered aggregator type could not be treated as a valid command aggregator");
                }

                return aggregator;
            }

            return new CommandAggregator();
        }

        /// <summary>
        /// Gets the new command aggregator.
        /// </summary>
        /// <param name="commandContainers">The command containers.</param>
        /// <returns>The command aggregator.</returns>
        public static ICommandAggregator GetNewCommandAggregator(IEnumerable<KeyValuePair<string, ICommandContainer>> commandContainers)
        {
            if (externalAggregatorType != null)
            {
                ICommandAggregator aggregator = Activator.CreateInstance(externalAggregatorType) as ICommandAggregator;
                if (aggregator == null)
                {
                    throw new InvalidCastException("Registered aggregator type could not handled as a valid command aggregator");
                }

                foreach (var cmd in commandContainers)
                {
                    aggregator.AddOrSetCommand(cmd.Key, cmd.Value);
                }

                return aggregator;
            }

            return new CommandAggregator(commandContainers);
        }
    }
}