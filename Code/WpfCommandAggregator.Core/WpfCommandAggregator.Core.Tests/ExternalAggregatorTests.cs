﻿namespace UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using WpfCommandAggregator.Core;

    [TestClass]
    public class ExternalAggregatorTests
    {
        [TestInitialize]
        public void Register()
        {
            CommandAggregatorFactory.RegisterAggreagtorImplementation<OwnAggregator>();
        }

        [TestCleanup]
        public void Unregister()
        {
            CommandAggregatorFactory.UnregisterAggreagtorImplementation<OwnAggregator>();
        }

        [TestMethod]
        public void TypeTest()
        {
            var aggregator = CommandAggregatorFactory.GetNewCommandAggregator();

            Assert.IsNotInstanceOfType(aggregator, typeof(CommandAggregator));
            Assert.IsInstanceOfType(aggregator, typeof(OwnAggregator));
        }
    }

    /// <summary>
    /// Test aggregator.
    /// </summary>
    internal class OwnAggregator : ICommandAggregator
    {
        public ICommandContainer this[string key] => throw new NotImplementedException();

        public bool HasAnyCommandContainer => throw new NotImplementedException();

        public void AddOrSetCommand([NotNull] string key, ICommand? command)
        {
            throw new NotImplementedException();
        }

        public void AddOrSetCommand([NotNull] string key, ICommand? command, Dictionary<string, object?>? settings)
        {
            throw new NotImplementedException();
        }

        public void AddOrSetCommand([NotNull] string key, ICommandContainer? commandContainer)
        {
            throw new NotImplementedException();
        }

        public void AddOrSetCommand([NotNull] string key, Action<object?>? executeDelegate, Predicate<object?>? canExecuteDelegate)
        {
            throw new NotImplementedException();
        }

        public void AddOrSetCommand([NotNull] string key, Action<object?>? executeDelegate)
        {
            throw new NotImplementedException();
        }

        public void AddOrSetCommand([NotNull] string key, Action<object?>? executeDelegate, Predicate<object?>? canExecuteDelegate, Dictionary<string, object?>? settings)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public Task ExecuteAsync([NotNull] string key, object? parameter = null)
        {
            throw new NotImplementedException();
        }

        public bool Exists([NotNull] string key)
        {
            throw new NotImplementedException();
        }

        public ICommandContainer GetCommandContainer([NotNull] string key)
        {
            throw new NotImplementedException();
        }

        public bool HasNullCommandContainer([NotNull] string key)
        {
            throw new NotImplementedException();
        }

        public bool HasNullCommand([NotNull] string key)
        {
            throw new NotImplementedException();
        }

        public void Remove([NotNull] string key)
        {
            throw new NotImplementedException();
        }

        public void RemoveAll()
        {
            throw new NotImplementedException();
        }        
    }
}