namespace UnitTests;

using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfCommandAggregator.Core;

[TestClass]
public class CommandContainerTests
{
    [TestMethod]
    public void ConstructorWithoutSettingsTest()
    {
        ICommand command = new RelayCommand(new Action<object?>(o => { }));
        CommandContainer container = new CommandContainer(command);

        Assert.IsNotNull(container.Settings);
        Assert.AreEqual(0, container.Settings.Count);
    }

    [TestMethod]
    public void ConstructorWithSettingsTest()
    {
        ICommand command = new RelayCommand(new Action<object?>(o => { }));
        Dictionary<string, object?> settings = new Dictionary<string, object?>
        {
            { "A", true },
            { "B", false }
        };

        CommandContainer container = new CommandContainer(command, settings);

        Assert.IsNotNull(container.Settings);
        Assert.AreEqual(2, container.Settings.Count);
    }

    [TestMethod]
    public void SettingsIndexerTest()
    {
        ICommand command = new RelayCommand(new Action<object?>(o => { }));
        Dictionary<string, object?> settings = new Dictionary<string, object?>
        {
            { "A", true },
            { "B", false }
        };
        CommandContainer container = new CommandContainer(command, settings);
        Assert.IsTrue((bool)container["A"]!);
        Assert.IsFalse((bool)container["B"]!);
        Assert.IsNull(container["C"]);
    }
}
