using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace WpfCommandAggregator.Core
{
    /// <summary>
    /// Relay Command based on Josh Smith's implementation.
    /// Extended by additional null checks and renamings (M. Armbruster).
    /// </summary>   
    /// <remarks>
    ///   <para><b>History</b></para>
    ///   <list type="table">
    ///   <item>
    ///   <term><b>Author:</b></term>
    ///   <description>See Josh Smith's implementation!</description>
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
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// The execute delegate.
        /// </summary>
        protected Action<object> executeDelegate;

        /// <summary>
        /// The can execute delegate.
        /// </summary>
        protected Predicate<object> canExecuteDelegate;

        /// <summary>
        /// The pre action delegate.
        /// </summary>
        protected Action preActionDelegate;

        /// <summary>
        /// The post action delegate.
        /// </summary>
        public Action postActionDelegate;

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("Execute delegate is required for a command!");
            }

            this.executeDelegate = execute;
            this.canExecuteDelegate = canExecute;
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        /// <param name="preAction">The pre action.</param>
        /// <param name="postAction">The post action.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute, Action preAction, Action postAction)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("Execute delegate is required for a command!");
            }

            this.executeDelegate = execute;
            this.canExecuteDelegate = canExecute;
            this.preActionDelegate = preAction;
            this.postActionDelegate = postAction;
        }

        private void RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// The CanExecute method. Calls the given CanExecute delegate.
        /// </summary>
        /// <param name="parameter">The CanExecute parameter value.</param>
        /// <returns>
        /// True, if command can be executed; false otheriwse.
        /// </returns>
        public virtual bool CanExecute(object parameter)
        {
            return this.canExecuteDelegate == null ? true : this.canExecuteDelegate(parameter);
        }

        /// <summary>
        /// The Execute method. Calls the given Execute delegate.
        /// </summary>
        /// <param name="parameter">The Execute parameter value.</param>
        public virtual void Execute(object parameter)
        {
            if (preActionDelegate != null)
            {
                preActionDelegate.Invoke();
            }

            if (this.executeDelegate != null)
            {
                this.executeDelegate(parameter);
            }

            if (postActionDelegate != null)
            {
                postActionDelegate.Invoke();
            }

            this.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Overrides the pre action delegate.
        /// </summary>
        /// <param name="preAction">The pre action.</param>
        public void OverridePreActionDelegate(Action preAction)
        {
            this.preActionDelegate = preAction;
            this.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Overrides the post action delegate.
        /// </summary>
        /// <param name="postAction">The post action.</param>
        public void OverridePostActionDelegate(Action postAction)
        {
            this.postActionDelegate = postAction;
            this.RaiseCanExecuteChanged();
        }
    }
}
