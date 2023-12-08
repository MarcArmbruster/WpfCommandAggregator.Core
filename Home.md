# WPF Command Aggregator.Core
The WpfCommandAggregator.Core is an MVVM package for an easier, faster and more comfortable use of MVVM and especially of commands in the environment of WPF.Core (WPF.NET).
This solution is very similar to the implementation known from the .net classic environment. However, the solution was extended by the concept of the ICommandContainer, so that a logical grouping of ICommand and related information such as label texts, colors, etc. is possible.


Latest stable version is available as a nuGet package (up from November 9th, 2019):<br/>
[nuGet](https://www.nuget.org/packages/WPFCommandAggregator.Core/)

See also other versions on nuget:<br/>
* [nuGet: UWP Command Aggregator](https://www.nuget.org/packages/UwpCommandAggregator/)
* [nuGet: WPF Command Aggregator](https://www.nuget.org/packages/WPFCommandAggregator/)

## Versions 
- 2.0.0.0
    - InitCommands method within BaseVm was set from abstract to virtual
    - Target Framework set to .NET 6 (or higher). 
    If .NET3.1 or .NET5 must be supported, you can still use version 1.1.0.0 
- 1.1.0.0
  - Upgrade to .net Core 3.1 (LTS version)
  - new: ObservableCollectionExt extends ObservableCollection with fast 
    AddRange, RemoveItems methods and also a Replace method
    The class is placed in the namespace System.Collections.ObjectModel.
  - new: Attached properties for managing focus and closing windows via view model properties.
- 1.0.1 : 
  - non-used public flag removed: 'AutoTriggerCommandNotification'
- 1.0.0 : 
  - WPF Command Aggregator.Core

            
## List of features

### CommandAggregator

Feature           |Since version
------------------|----------
CommandAggregator|1.0.0
Factory/Custom Aggregator|1.0.0
CommandContainer|1.0.0
RelayCommand|1.0.0
HierarchyCommands|1.0.0
DependsOn Attribute|1.0.0
Pre- and post action delegates|1.0.0


### Base ViewModel (BaseVm)
Feature           |Since version
------------------|----------
Included CommandAggregator instance|1.0.0
Notification supression (SuppressNotifications flag)|1.0.0
SetPropertyValue for backing fields|1.0.0
SetPropertyValue<> and GetPropertyValue<> for automatic value storage|1.0.0
DependsOn Attribute|1.0.0

### Additional Features
Feature           |Since version
------------------|----------
ObservableCollectionExt|1.1.0
AttachedProperty Focused|1.1.0
AttachedProperty WindowResult|1.1.0

# How it works
## The idea
Many years I worked on many WPF projects (MVVM) with many views and thereof with many command objects. 
It was quite boring to write similar code for every command definition. 
The well known RelayCommand (Josh Smith) helps a lot but there is still similar code for every command definition.
* private ICommand member
* public ICommand getter incl. creation logic

If you have to create views with a lot of functionality (many customers still like screens full of data and functionalities), 
for example ToolBars and ContextMenus also a lot of commands has to be defined.

This leads to a great amount of code lines with very similar structure. 

**Example (WITHOUT Command Aggregator):**
```C#
private ICommand printCommand;
public ICommand PrintCommand
{
     get
     {
          if (this.printCommand == null)
          {
              this.printCommand = new RelayCommand(p1 => Print(p1), p2 => CanPrint);
          }
          return this.printCommand;
      }
}
```

All we want to tell is: _PrintCommand_ executes the _Print_ method if it is allowed (_CanPrint_ property).

## The aggregator and the base view model
The WPFCommandAggregator.Core is a solution to reduce the command definitions to a very short and easy to read line of code within a view model class.

**Example (WITH Command Aggregator):**
```C#
this.CmdAgg.AddOrSetCommand("Print", new RelayCommand(p1 => Print(p1), p2 => CanPrint));
```
OR (including settings)
```C#
this.CmdAgg.AddOrSetCommand("Print", new RelayCommand(p1 => Print(p1), p2 => CanPrint), new Dictionary<string, object> { { "ButtonContent", "Print me!" }});
```
(many more overloads exists)

This is a reduction of about 10 lines (!!!!) and (in my opinion) a very well readable command definition.
How can this be achieved and how can we use it for bindings (XAML)?


So we have a functionality to collect commands within a CommandAggregator instance, but how can we use it - especially in bindings?

First, the BaseVm class provides a command aggregator instance and an abstract method called _InitCommands_. The CommandAggregator instance is created by a factory class. 
Thus we get in ViewModels (which inherit from BaseVm) the possibility to store all command definitions in the overloaded method _InitCommands_. Thus they are concentrated in one place in the class and can be easily maintained and extended.

The mentioned factory class for the CommandAggregator instance also offers the possibility to register an own and individual implementation of the ICommandAggregator interface - if required.

```C#
public abstract class BaseVm : INotifyPropertyChanged
{
   public ICommandAggregator CmdAgg { get; } 
     = CommandAggregatorFactory.GetNewCommandAggregator();

   protected abstract void InitCommands();

   protected BaseVm()
   {
        this.InitCommands();
   }

   // ... further elements of the base view model
}
```

The abstract _InitCommands_ method is the place where the commands will be registered/defined within each view model.
```C#
public class MainVm : BaseVm
{
   protected sealed override void InitCommands()
   {
      this.CmdAgg.AddOrSetCommand("Print", new RelayCommand(p1 => Print(p1), p2 => CanPrint));
   }

   // ... more view model code ...
}
```

**But there is still one problem left: How do we bind the commands?**<br/>
At this point an indexer can help us. Indexers can be used also in Bindings. 
Commands do not require TwoWay bindings, so a readonly indexer within the CommandAggregator class enables us to establish a command binding in XAML.

```C#
    // The indexer of the BaseVm class
    public ICommand this[string key] => this.GetCommandContainer(key);

    public ICommandContainer GetCommandContainer(string key)
    {
        if (this.commandContainers.Any(k => k.Key == key))
        {
            return this.commandContainers[key];
        }
        else
        {
            // Empty command (to avoid null reference exceptions)
            return new CommandContainer(key, new RelayCommand(p1 => { }));
        }
    }
```

## Easy usage

In XAML we can use the CommandAggregator instance of the view model like this:

```XAML
<Button Content="Print" 
        Command="{Binding CmdAgg[Print].Command}" />
```

Indexer binding works with square brackets and the name of the registered command - you do not need any quotation marks!

The commands are wrapped into an ICommandContainer. 
This container provides the possibility to add meta data, e.g. The caption to display on a WPF-Button.

Example (InitCommands method)
```C#
this.CmdAgg.AddOrSetCommand(
            "Print", 
            new RelayCommand(p1 => Print(p1), p2 => CanPrint),
            new Dictionary<string, object>{ { "ControlCaption", "Print me" } });
```

Example (Usage in XAML)
```XAML
<Button
    Command="{Binding CmdAgg[Print].Command}"
    Content="{Binding CmdAgg[Print][ControlCaption]}" />
<Button
```

By this approach I was able to reduce the lines of code for command definitions 
many thousand lines in larger projects - without loosing any functionality! 
Each command definition/registration is reduced to exact one C# statement.

# Further ICommand features

## Hierarchy command

This HierarchyCommand also implements the ICommand interface and derives from the RelayCommand class.
A HierarchyCommand can have child commands. Therefore the HierarchyCommand depends on the child commands. Fore example: a SaveAll command depends on the save commands of all child commands.
To configure the dependency of the HierarchyCommand startegies can be used for Execute and CanExecute:

**HierarchyCanExecuteStrategy**
* DependsOnMasterCommandOnly
* DependsOnAllChilds
* DependsOnAtLeastOneChild

**HierarchyExecuteStrategy**
* MasterCommandOnly
* AllChildsOnly
* MasterAndAllChilds

With these values many business cases can be realized.

**A short example:**
```C#
   // Adding a hierarchy command
   ICommand save1Cmd = new RelayCommand(p1 => Save1(), p2 => this.CanSave1);
   ICommand save2Cmd = new RelayCommand(p1 => Save2(), p2 => this.CanSave2);

   this.CmdAgg.AddOrSetCommand("SaveCmd1", save1Cmd);
   this.CmdAgg.AddOrSetCommand("SaveCmd2", save2Cmd);

   HierarchyCommand saveAllCmd = new HierarchyCommand(
                p1 => {  },  -- no Execute required due to HierarchyExecuteStrategy
                p2 => null,  -- CanExecute not required due to HierarchyCanExecuteStrategy
                HierarchyExecuteStrategy.AllChildsOnly,
                HierarchyCanExecuteStrategy.DependsOnAllChilds);

   saveAllCmd.AddChildsCommand(new List<ICommand> { save1Cmd, save2Cmd });
   this.CmdAgg.AddOrSetCommand("SaveAll", saveAllCmd); 
```

## Pre- and post action delegates

The RelayCommand was extended by two delegates "PreActionDelegate" and "PostActionDelegate".
This enables you to add logic to run before and after the command execution delegate runs.
Therefore these delegates enables you to "inject" cross cutting concerns without the need to change your execution delegate logic.

+A short example (performance messurement):+
```C#
            this.CmdAgg.AddOrSetCommand(
                                        "Print", 
                                        new RelayCommand(
                                            p1 => MessageBox.Show("Print called"), 
                                            p2 => true, 
                                            this.performanceChecker.Start, 
                                            this.performanceChecker.Stop));
```

The performanceChecker instance is only a wrapper class to start and stop a stopwatch instance and writes the ellapsed time to the debug window. You can use any Action delegate.

When you like to set/change/remove these actions dynamically you can use the corresponding methods of the RelayCommand class.
```C#
            RelayCommand cmd = this.CmdAgg["Print"].Command as RelayCommand;
            if (cmd != null)
            {
                 cmd.OverridePostActionDelegate(null);
                 cmd.OverridePreActionDelegate(null);
            }
```
# Further features of the ViewModel base class BaseVm

## The 'DependsOn' attribute

Sometimes properties of a view model class depends on others. A simlpe example is the calculation of the sum of two input values.
In a classic way the code of the view model for this purpose could look like that:
```C#
        public decimal? FirstInput
        {
            get => this.firstInput;
            set
            {
                this.firstInput = value;
                this.NotifyPropertyChanged(nameof(FirstInput));
                this.NotifyPropertyChanged(nameof(Result));
            }
        }

        public decimal? SecondInput
        {
            get => this.secondInput;
            set
            {
                this.secondInput = value;
                this.NotifyPropertyChanged(nameof(SecondInput));
                this.NotifyPropertyChanged(nameof(Result));
            }
        }

        public decimal? Result
        {
            get => this.firstInput + this.secondInput;
        }

```

Wouldn't it be better to define/see the dependencies between the properties at one place: in my opinion the Result property would be a very good place.
Therefore, with the DependsOn attribute and the optimzed BaseVM class this can be achieved easier and better to read:
```C#
        public decimal? FirstInput
        {
            get => this.firstInput;
            set => this.SetPropertyValue(ref this.firstInput, value);
        }

        public decimal? SecondInput
        {
            get => this.secondInput;
            set => this.SetPropertyValue(ref this.secondInput, value);
        }

        [DependsOn(nameof(FirstInput), nameof(SecondInput))]
        public decimal? Result
        {
            get => this.firstInput + this.secondInput;
        }
```
Now, the attribute defines the dependencies and the BaseVM class will do the rest for you (notifications).

## 'SetPropertyValue'

In some cases in could be helpful to additionally execute some 
code lines before or - normally - after the set and notification is done.
Therefore the possibility to additionally define two action delegates is implemented.
The following example shows it based on two simple methods of a performance checker instance.

```C#
        public bool ExampleProperty
        {
            get => this.exampleProperty;
            set => this.SetPropertyValue(
                ref this.exampleProperty, 
                value,
                () => this.performanceChecker.Start(),
                () => this.performanceChecker.Stop());
        }
```

Furthermore the BaseVm class has a new protected property called SuppressNotifications.
By default this ist set to false. If you want to suppress notifications (e.g. to temporarily improve performance by decoupling from UI)

```C#
    this.SuppressNotifications = true;
``` 

The command aggregator interface itself was also extended by a new overload for the AddOrSetCommand method.
The definition for always executable commands  can shortly defined by omitting the can execute delegate.

The definition
```C#	
    this.CmdAgg.AddOrSetCommand("Exit", new RelayCommand(p1 => MessageBox.Show("Exit called"), p2 => true));
```
can now be simplified with the following one:

```C#	
    this.CmdAgg.AddOrSetCommand("Exit", new RelayCommand(p1 => MessageBox.Show("Exit called")));
```


## Automatic values storage, custom aggregators and notification optimization

1. An optimization: Normally, a **notification** is always triggered for value assignments to a (bindable) property. With this implementation this only happens if the value has effectively changed - in other words, if the content has changed.   
2. A further feature: automatic values storage<br/>
The base ViewModel now has a **value storage** in which the values for the (bindable) properties are stored. This means that no more private fields have to be declared in the ViewModels. This advantage brings with it a small disadvantage: when reading the values a 'cast' is necessary. For properties that are read very frequently, you should therefore use this with caution to avoid performance disadvantages. Of course, you can continue to work with private fields in the future. A mixture of both concepts within a ViewModel is not recommended with regard to the readability of the code (but is possible without problems).<br/>
    ```C#	
    public bool CanSave
    {
        // using NO private field -> using automatic values storage (from base class).
        get => this.GetPropertyValue<bool>();
        set => this.SetPropertyValue<bool>(value);                          
    }
    ```
3.  Another feature: **custom command aggregator** implmentations<br/>
It is possible to use - if required - your own implementation of the Command Aggregator. For this purpose this implementation can be registered at the factory class. Until a possible deregistration the own implementation will be used by the factory. The condition is that the implementation has a parameterless constructor.<br/>
**register:** (to use an own/custom implementation)
    ```C#	
    CommandAggregatorFactory.RegisterAggreagtorImplementation<OwnAggregator>();
    // now the own/custom aggregator will be used!
    ```
    **unregister** (to change the factory behavior during runtime - otherwise not necessary):
    ```C#	
    CommandAggregatorFactory.UnregisterAggreagtorImplementation<OwnAggregator>();
    // now the default aggregator will be used again!
    ```

## Version 1.1.0.0: ObservableCollectionExt, attached properties Focus and WindowCloser

The version 1.1.0.0 contains three new features.

1. The class ObservableCollectionExt extends the well-known framework class ObservableCollection with fast methods to add and reduce multiple elements. A replace method has also been created that exchanges elements in the collection.<br/>   
    ```C#	
    public ObservableCollectionExt<Person> Persons
    {
        get { return this.GetPropertyValue<ObservableCollectionExt<Person>>(); }
        private set { this.SetPropertyValue<ObservableCollectionExt<Person>>(value); }
    }

    private void Test()
    {
        this.Persons.AddRange(this.allPersons);
        this.Persons.RemoveItems(this.allPersons);
        this.Persons.Replace(allPersons.First(), new Person { Name = "Gerhard", Age = 27 });
    }
    ```
2. Attached property to set/remove focus from a Control.<br/>

    ```XAML	
     <TextBox
                ap:Focused.Focused="{Binding Path=FirstValueHasFocus, UpdateSourceTrigger=PropertyChanged, Mode=TwoWay}" 
                Grid.Row="1"
                Grid.Column="0">
     </TextBox>
    ```
3.  The third new feature: is an attached property to close views from the view model.<br/>

    ```C#	
    private void CloseWindow()
    {
        // setting the value to true (or false) will close the window using this instance as its DataContext
        // and uses the attached property.
        // The property itself is defined in class BaseVm.
        this.WindowResult = true;
    }
    ```
    
    ```XAML	
    <Window
    x:Class="CommandAggregatorExample.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:ap="clr-namespace:WPFCommandAggregator.AttachedProperties;assembly=WPFCommandAggregator"
    ap:WindowCloser.WindowResult="{Binding WindowResult, UpdateSourceTrigger=PropertyChanged, Mode=TwoWay}"
    Title="Command Aggregator Example"
    Width="600"
    Height="600"
    WindowStartupLocation="CenterScreen"/>
    ```

(see also the example solution (source code))
