namespace UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Windows.Input;
    using WpfCommandAggregator.Core;

    [TestClass]
    public class RelayCommandTests
    {
        private class LocalTestingClass
        {
            internal string Test { get; set; }

            public LocalTestingClass()
            {
                this.Test = string.Empty;
            }
        }

        [TestMethod]
        public void DefaultTest()
        {
            LocalTestingClass testObject = new LocalTestingClass();
            ICommand cmd = new RelayCommand(p1 => testObject.Test = p1.ToString(), p2 => true);
            cmd.Execute("defaulttest");

            Assert.AreEqual("defaulttest", testObject.Test);
        }

        [TestMethod]
        public void PreActionTest()
        {

            LocalTestingClass testObject = new LocalTestingClass();

            Action preAction = new Action(() => testObject.Test = "start");

            ICommand cmd = new RelayCommand(p1 => testObject.Test += p1.ToString(), p2 => true, preAction, null);
            cmd.Execute("pretest");

            Assert.AreEqual("startpretest", testObject.Test);
        }

        [TestMethod]
        public void PostActionTest()
        {

            LocalTestingClass testObject = new LocalTestingClass();

            Action postAction = new Action(() => testObject.Test += "end");

            ICommand cmd = new RelayCommand(p1 => testObject.Test += p1.ToString(), p2 => true, null, postAction);
            cmd.Execute("pretest");

            Assert.AreEqual("pretestend", testObject.Test);
        }
    }
}