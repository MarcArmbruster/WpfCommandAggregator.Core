﻿namespace UnitTests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WpfCommandAggregator.Core;

    [TestClass]
    public class BaseVmTests
    {
        /// <summary>
        /// Private test class -> testing object.
        /// </summary>
        private class BaseTestViewModel : BaseVm
        {
            private string defaultTestProperty = string.Empty;
            public string DefaultTestProperty
            {
                get => this.defaultTestProperty;
                set => this.SetPropertyValue(ref this.defaultTestProperty, value);
            }

            private string preAndPostActionTestProperty = string.Empty;
            public string PreAndPostActionTestProperty
            {
                get => this.preAndPostActionTestProperty;
                set => this.SetPropertyValue(
                    ref this.preAndPostActionTestProperty,
                    value,
                    () => this.PreSetResult = "before",
                    () => this.PostSetResult = "after");
            }

            internal string PreSetResult { get; private set; } = string.Empty;
            internal string PostSetResult { get; private set; } = string.Empty;

            protected override void InitCommands()
            {
                this.CmdAgg.AddOrSetCommand("TestCommand", new RelayCommand(new Action<object?>(p1 => { })));
            }
        }

        [TestMethod]
        public void AddAndExistsTest()
        {
            BaseTestViewModel testVm = new BaseTestViewModel();

            testVm.DefaultTestProperty = "Dummy";
            Assert.AreEqual("Dummy", testVm.DefaultTestProperty);

            Assert.IsTrue(testVm.CmdAgg.Exists("TestCommand"));
        }

        [TestMethod]
        public void PreAndPostSetActionTest()
        {
            BaseTestViewModel testVm = new BaseTestViewModel();

            testVm.PreAndPostActionTestProperty = "Something";
            Assert.AreEqual("before", testVm.PreSetResult);
            Assert.AreEqual("after", testVm.PostSetResult);
        }
    }
}
