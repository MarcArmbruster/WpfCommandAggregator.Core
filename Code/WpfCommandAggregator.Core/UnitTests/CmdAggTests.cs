namespace UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WpfCommandAggregator.Core;

    [TestClass]
    public class CmdAggTests
    {
        [TestMethod]
        public void AddAndExistsTest()
        {
            ICommandAggregator cmdAgg = CommandAggregatorFactory.GetNewCommandAggregator();

            cmdAgg.AddOrSetCommand("TestCommand1", new RelayCommand(p1 => { }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand2", new RelayCommand(p1 => { }, p2 => true));

            Action<object> act = new Action<object>(p0 => { });
            Predicate<object> pred = new Predicate<object>(p1 => { return true; });
            cmdAgg.AddOrSetCommand("TestCommand3", act, pred);

            Assert.IsTrue(cmdAgg.Exists("TestCommand1"));
            Assert.IsTrue(cmdAgg.Exists("TestCommand2"));
            Assert.IsTrue(cmdAgg.Exists("TestCommand3"));
            Assert.IsFalse(cmdAgg.Exists("TestCommand4"));
        }

        [TestMethod]
        public void CollectionConstructionTest()
        {
            ICommand cmd1 = new RelayCommand(p1 => { }, p2 => true);
            ICommand cmd2 = new RelayCommand(p1 => { }, p2 => true);

            var commandList = new List<KeyValuePair<string, ICommandContainer>>();
            commandList.Add(new KeyValuePair<string, ICommandContainer>("A", new CommandContainer(cmd1)));
            commandList.Add(new KeyValuePair<string, ICommandContainer>("B", new CommandContainer(cmd2)));
            ICommandAggregator cmdAgg = CommandAggregatorFactory.GetNewCommandAggregator(commandList);

            Assert.IsTrue(cmdAgg.Exists("A"));
            Assert.IsTrue(cmdAgg.Exists("B"));
            Assert.IsTrue(cmdAgg.Count() == 2);
        }

        [TestMethod]
        public void HasNullCommandContainerTest()
        {
            ICommandAggregator cmdAgg = CommandAggregatorFactory.GetNewCommandAggregator();

            cmdAgg.AddOrSetCommand("TestCommand1", (ICommandContainer)null);
            cmdAgg.AddOrSetCommand("TestCommand2", new RelayCommand(p1 => { }, p2 => true));

            Assert.IsTrue(cmdAgg.HasNullCommandContainer("TestCommand1"));
            Assert.IsFalse(cmdAgg.HasNullCommandContainer("TestCommand2"));
        }

        [TestMethod]
        public void HasNullCommandTest()
        {
            ICommandAggregator cmdAgg = CommandAggregatorFactory.GetNewCommandAggregator();

            cmdAgg.AddOrSetCommand("TestCommand1", (ICommandContainer)null);
            cmdAgg.AddOrSetCommand("TestCommand2", new RelayCommand(p1 => { }, p2 => true));

            Assert.IsTrue(cmdAgg.HasNullCommand("TestCommand1"));
            Assert.IsFalse(cmdAgg.HasNullCommand("TestCommand2"));
        }

        [TestMethod]
        public void CountTest()
        {
            ICommandAggregator cmdAgg = CommandAggregatorFactory.GetNewCommandAggregator();

            Assert.AreEqual(0, cmdAgg.Count());
            cmdAgg.AddOrSetCommand("TestCommand1", new RelayCommand(p1 => { }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand2", new RelayCommand(p1 => { }, p2 => true));

            Assert.AreEqual(2, cmdAgg.Count());
        }

        [TestMethod]
        public void RemoveTest()
        {
            ICommandAggregator cmdAgg = CommandAggregatorFactory.GetNewCommandAggregator();
            cmdAgg.AddOrSetCommand("TestCommand1", new RelayCommand(p1 => { }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand2", new RelayCommand(p1 => { }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand3", new RelayCommand(p1 => { }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand4", new RelayCommand(p1 => { }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand5", new RelayCommand(p1 => { }, p2 => true));
            Assert.IsTrue(cmdAgg.Count() == 5);

            cmdAgg.Remove("TestCommand3");
            Assert.IsTrue(cmdAgg.Count() == 4);
            Assert.IsTrue(cmdAgg.Exists("TestCommand1"));
            Assert.IsTrue(cmdAgg.Exists("TestCommand2"));
            Assert.IsFalse(cmdAgg.Exists("TestCommand3"));
            Assert.IsTrue(cmdAgg.Exists("TestCommand4"));
            Assert.IsTrue(cmdAgg.Exists("TestCommand5"));


            cmdAgg.RemoveAll();
            Assert.IsTrue(cmdAgg.Count() == 0);
            Assert.IsFalse(cmdAgg.Exists("TestCommand1"));
            Assert.IsFalse(cmdAgg.Exists("TestCommand2"));
            Assert.IsFalse(cmdAgg.Exists("TestCommand3"));
            Assert.IsFalse(cmdAgg.Exists("TestCommand4"));
            Assert.IsFalse(cmdAgg.Exists("TestCommand5"));
        }

        [TestMethod]
        public void GetCommandContainerAndIndexerAndExecuteTest()
        {
            ICommandAggregator cmdAgg = CommandAggregatorFactory.GetNewCommandAggregator();
            StringBuilder strBld = new StringBuilder(1000);

            cmdAgg.AddOrSetCommand("TestCommand1", new RelayCommand(p1 => { strBld.Append("1"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand2", new RelayCommand(p1 => { strBld.Append("2"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand3", new RelayCommand(p1 => { strBld.Append("3"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand4", new RelayCommand(p1 => { strBld.Append("4"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand5", new RelayCommand(p1 => { strBld.Append("5"); }, p2 => true));

            ICommand cmd5 = cmdAgg.GetCommandContainer("TestCommand5").Command;
            cmd5.Execute(null);
            Assert.AreEqual("5", strBld.ToString());

            strBld.Clear();
            ICommand cmd2 = cmdAgg.GetCommandContainer("TestCommand2").Command;
            cmd2.Execute(null);
            Assert.AreEqual("2", strBld.ToString());

            strBld.Clear();
            ICommand cmd1 = cmdAgg["TestCommand1"].Command;
            cmd1.Execute(null);
            Assert.AreEqual("1", strBld.ToString());
        }

        [TestMethod]
        public void ExecuteAsyncTest()
        {
            ICommandAggregator cmdAgg = CommandAggregatorFactory.GetNewCommandAggregator();
            StringBuilder strBld = new StringBuilder(1000);

            cmdAgg.AddOrSetCommand("TestCommand1", new RelayCommand(p1 => { strBld.Append("1"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand2", new RelayCommand(p1 => { strBld.Append("2"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand3", new RelayCommand(p1 => { strBld.Append("3"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand4", new RelayCommand(p1 => { strBld.Append("4"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand5", new RelayCommand(p1 => { strBld.Append("5"); }, p2 => true));

            Task t1 = cmdAgg.ExecuteAsync("TestCommand1");
            Task t2 = cmdAgg.ExecuteAsync("TestCommand2");
            Task t3 = cmdAgg.ExecuteAsync("TestCommand3");
            Task t4 = cmdAgg.ExecuteAsync("TestCommand4");
            Task t5 = cmdAgg.ExecuteAsync("TestCommand5");

            Task.WaitAll(t1, t2, t3, t4, t5);

            Assert.IsTrue(strBld.ToString().Contains("1"));
            Assert.IsTrue(strBld.ToString().Contains("2"));
            Assert.IsTrue(strBld.ToString().Contains("3"));
            Assert.IsTrue(strBld.ToString().Contains("4"));
            Assert.IsTrue(strBld.ToString().Contains("5"));
        }

        [TestMethod]
        public void CanExecuteTest()
        {
            ICommandAggregator cmdAgg = CommandAggregatorFactory.GetNewCommandAggregator();
            StringBuilder strBld = new StringBuilder(1000);

            cmdAgg.AddOrSetCommand("TestCommand1", new RelayCommand(p1 => { strBld.Append("1"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand2", new RelayCommand(p1 => { strBld.Append("2"); }, p2 => false));
            cmdAgg.AddOrSetCommand("TestCommand3", new RelayCommand(p1 => { strBld.Append("3"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand4", new RelayCommand(p1 => { strBld.Append("4"); }, p2 => false));
            cmdAgg.AddOrSetCommand("TestCommand5", new RelayCommand(p1 => { strBld.Append("5"); }, p2 => true));

            ICommand cmd1 = cmdAgg["TestCommand1"].Command;
            Assert.IsTrue(cmd1.CanExecute(null));

            ICommand cmd2 = cmdAgg["TestCommand2"].Command;
            Assert.IsFalse(cmd2.CanExecute(null));

            ICommand cmd3 = cmdAgg["TestCommand3"].Command;
            Assert.IsTrue(cmd3.CanExecute(null));

            ICommand cmd4 = cmdAgg["TestCommand4"].Command;
            Assert.IsFalse(cmd4.CanExecute(null));

            ICommand cmd5 = cmdAgg["TestCommand5"].Command;
            Assert.IsTrue(cmd5.CanExecute(null));
        }

        [TestMethod]
        public void CommandOverrideTest()
        {
            ICommandAggregator cmdAgg = CommandAggregatorFactory.GetNewCommandAggregator();
            StringBuilder strBld = new StringBuilder(1000);

            cmdAgg.AddOrSetCommand("TestCommand1", new RelayCommand(p1 => { strBld.Append("1"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand2", new RelayCommand(p1 => { strBld.Append("2"); }, p2 => false));

            cmdAgg.AddOrSetCommand("TestCommand1", new RelayCommand(p1 => { strBld.Append("3"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand2", p1 => { strBld.Append("4"); }, p2 => true);

            cmdAgg["TestCommand1"].Command.Execute(null);
            cmdAgg["TestCommand2"].Command.Execute(null);

            Assert.AreEqual("34", strBld.ToString());
        }
    }
}