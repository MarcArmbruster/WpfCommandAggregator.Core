namespace UnitTests
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WpfCommandAggregator.Core;

    [TestClass]
    public class CommandAggregatorFactoryTests
    {
        [TestInitialize]
        public void Init()
        {
            CommandAggregatorFactory.ClearRegistration();
        }

        [TestMethod]
        public void GetNewCommandAggregatorDefaultTest()
        {
            var defaultAggregator = CommandAggregatorFactory.GetNewCommandAggregator();

            Assert.IsNotNull(defaultAggregator);
            Assert.IsTrue(defaultAggregator.GetType().Equals(typeof(CommandAggregator)));
        }

        [TestMethod]
        public void GetNewCommandAggregatorCustomTest()
        {
            CommandAggregatorFactory.RegisterAggreagtorImplementation<FakeAggregator>();
            var customAggregator = CommandAggregatorFactory.GetNewCommandAggregator();
            CommandAggregatorFactory.UnregisterAggreagtorImplementation<FakeAggregator>();

            Assert.IsNotNull(customAggregator);
            Assert.IsTrue(customAggregator.GetType().Equals(typeof(FakeAggregator)));
        }

        [TestMethod]
        public void GetNewCommandAggregatorCustomWithContainerTest()
        {
            ICommandContainer container = new CommandContainer(new RelayCommand(new Action<object>(o => { })));
            container.Settings.Add("A", true);

            CommandAggregatorFactory.RegisterAggreagtorImplementation<FakeAggregator>();
            var customAggregator = CommandAggregatorFactory.GetNewCommandAggregator(
                new List<KeyValuePair<string, ICommandContainer>> 
                { 
                    new KeyValuePair<string, ICommandContainer>("x", container) 
                });
            CommandAggregatorFactory.UnregisterAggreagtorImplementation<FakeAggregator>();

            Assert.IsNotNull(customAggregator);
            Assert.IsTrue(customAggregator.GetType().Equals(typeof(FakeAggregator)));
            Assert.IsNotNull(customAggregator.GetCommandContainer("x"));
        }
    }

    internal class FakeAggregator : CommandAggregator
    {       
    }
}