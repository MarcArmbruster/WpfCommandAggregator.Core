namespace WpfCommandAggregator.Core;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

/// <summary>
/// Base View Model
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
public abstract class BaseVm : INotifyPropertyChanged
{
    /// <summary>
    /// Global (static) storage for property dependencies.
    /// </summary>
    private static ConcurrentDictionary<Type, Dictionary<string, List<string>>> propertyDependencies = [];

    /// <summary>
    /// Dictionary to hold private values of bindable properties.
    /// </summary>
    private Dictionary<string, object?> values = [];

    /// <summary>
    /// Property to suppress notifications.
    /// Set to true if you want to supress all notifications.
    /// Default is false (active notifications).
    /// </summary>
    [JsonIgnore]
    public bool SuppressNotifications { get; set; }

    /// <summary>
    /// This property is used by the attached property 'WindowCloser'. 
    /// If this attached property is attached to a window, this WindowResult property can be used to close the window 
    /// from this current view model by setting value to true or false. A null value will not close the window.
    /// </summary>
    [JsonIgnore]
    public bool? WindowResult
    {
        get => this.GetPropertyValue<bool?>();
        set => this.SetPropertyValue<bool?>(value);
    }

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseVm"/> class.
    /// </summary>
    protected BaseVm()
    {
        this.InitCommands();
        this.ReadPropertyDependencies();
    }

    #endregion Constructor     

    #region WPF Command Aggregator

    /// <summary>
    /// Gets the command aggregate.
    /// </summary>
    /// <value>
    /// The command aggregate.
    /// </value>
    [JsonIgnore]
    public ICommandAggregator CmdAgg { get; } = CommandAggregatorFactory.GetNewCommandAggregator();

    /// <summary>
    /// Initializes the commands - has to be overridden in derived classes.
    /// This is the place to register your view model specific commands.
    /// </summary>
    protected virtual void InitCommands() { }

    #endregion WPF Command Aggregator

    #region Extended INotifyPropertyChanged Implementation

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Notifies that the property has changed - fires the event.
    /// </summary>
    /// <param name="propertyName">Optional: property name (dervied from caller by default).</param>
    public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        this.RaisePropertyChangedEvent(propertyName);
        this.RaisePropertyChangedEventForDependendProperties(propertyName);
    }

    /// <summary>
    /// Sets the value and raises the property changed event (includes dependend properties).
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="property">The target.</param>
    /// <param name="value">The new value.</param>
    /// <param name="propertyName">Name of the property - automatically resolved by the framework if not explicitly defined.</param>
    protected virtual void SetPropertyValue<T>(ref T property, T value, [CallerMemberName] string? propertyName = null)
    {
        bool notify = !Equals(property, value) && !this.SuppressNotifications;
        property = value;

        if (notify)
        {
            this.RaisePropertyChangedEvent(propertyName);
            this.RaisePropertyChangedEventForDependendProperties(propertyName);
        }
    }

    /// <summary>
    /// Sets the value and raises the property changed event (includes dependend properties).
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="property">The target.</param>
    /// <param name="value">The new value.</param>
    /// <param name="preSetAction">An Action that will execute before set and notification is done.</param>
    /// <param name="postSetAction">An Action that will execute after set and notification is done.</param>
    /// <param name="propertyName">Optional: property name (dervied from caller by default).</param>
    protected virtual void SetPropertyValue<T>(
        ref T property, 
        T value, 
        Action? preSetAction, 
        Action? postSetAction, 
        [CallerMemberName] string? propertyName = null)
    {
        preSetAction?.Invoke();
        this.SetPropertyValue<T>(ref property, value, propertyName);
        postSetAction?.Invoke();
    }

    /// <summary>
    /// Sets the value by using the automatic private values storage and raises the property changed event (includes dependend properties) - if effective value is changed.
    /// </summary>
    /// <remarks>
    /// Using automatic values storage - DO NOT combine with private fields fior bindable properties in view models.
    /// </remarks>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="value">The new value.</param>
    /// <param name="propertyName">Name of the property - automatically resolved by the framework if not explicitly defined.</param>
    protected void SetPropertyValue<T>(T value, [CallerMemberName] string propertyName = "")
    {
        bool notify = !Equals(value, GetPropertyValue<T>(propertyName)) && !this.SuppressNotifications;

        this.values[propertyName] = value;

        if (notify)
        {
            this.RaisePropertyChangedEvent(propertyName);
            this.RaisePropertyChangedEventForDependendProperties(propertyName);
        }
    }

    /// <summary>
    /// Sets the value by using the automatic private values storage and raises the property changed event (includes dependend properties) - if effective value is changed.
    /// </summary>
    /// <remarks>
    /// Using automatic values storage - DO NOT combine with private fields fior bindable properties in view models.
    /// </remarks>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="preSetAction">An Action that will execute before set and notification is done.</param>
    /// <param name="postSetAction">An Action that will execute after set and notification is done.</param>
    /// <param name="value">The new value.</param>
    /// <param name="propertyName">Name of the property - automatically resolved by the framework if not explicitly defined.</param>
    protected void SetPropertyValue<T>(
        T value, 
        Action? preSetAction, 
        Action? postSetAction, 
        [CallerMemberName] string propertyName = "")
    {
        preSetAction?.Invoke();
        this.SetPropertyValue(value, propertyName);
        postSetAction?.Invoke();
    }

    /// <summary>
    /// Gets the property value by using the automatic private values storage.
    /// </summary>
    /// <remarks>
    /// Using automatic values storage - DO NOT combine with private fields fior bindable properties in view models.
    /// </remarks>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="propertyName">Name of the property - automatically resolved by the framework if not explicitly defined.</param>
    /// <returns>The value of the property. If not found, the default value of the type will be returned.</returns>
    protected T? GetPropertyValue<T>([CallerMemberName] string propertyName = "")
    {
        if (!this.values.ContainsKey(propertyName))
        {
            this.values[propertyName] = default(T);
        }

        object? value = this.values[propertyName];
        if (value == null)
        {
            return default(T);
        }

        return (T)value;
    }

    /// <summary>
    /// Raises the property changed event.
    /// </summary>
    /// <param name="propertyName"></param>
    private void RaisePropertyChangedEvent(string? propertyName)
    {
        if (this.SuppressNotifications 
            || PropertyChanged == null
            || string.IsNullOrWhiteSpace(propertyName))
        {
            return;
        }

        var eventArgs = new PropertyChangedEventArgs(propertyName);
        PropertyChanged(this, eventArgs);
    }

    /// <summary>
    /// Raises the property changed event for all dependend properties.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    private void RaisePropertyChangedEventForDependendProperties(string? propertyName)
    {
        var dependencies = propertyDependencies[this.GetType()];
        var keysOfDependencies = dependencies.Where(d => d.Value.Contains(propertyName ?? string.Empty)).Select(d => d.Key);
        keysOfDependencies?.ToList()?.ForEach(depName => this.RaisePropertyChangedEvent(depName));
    }

    /// <summary>
    /// Reads the property dependencies and sores it in the static dictionary.
    /// </summary>
    private void ReadPropertyDependencies()
    {
        Type type = this.GetType();
        if (propertyDependencies.ContainsKey(type))
        {
            return;
        }

        propertyDependencies[type] = [];

        PropertyInfo[] propertyInfos = type.GetProperties();
        foreach (var propInfo in propertyInfos)
        {
            var attributes = propInfo.GetCustomAttributes(true);
            foreach (DependsOnAttribute attribute in attributes.Where(a => a is DependsOnAttribute))
            {
                var names = attribute.PropertyNames.ToList();
                propertyDependencies[type].Add(propInfo.Name, names);
            }
        }
    }

    #endregion Extended INotifyPropertyChanged Implementation
}