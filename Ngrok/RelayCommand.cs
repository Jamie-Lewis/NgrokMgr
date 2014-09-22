using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ngrok
{
  /// <summary>
  /// An ICommand that allows you to pass in the Execute/CanExecute methods as constructor parameters
  /// </summary>
  public class RelayCommand : ICommand
  {
    readonly Action<object> _execute;
    readonly Predicate<object> _canExecute;

    /// <summary>
    /// Initialise with only an execution method. This will always return true for CanExecute
    /// </summary>
    /// <param name="execute">The delegate to execute when the command is called</param>
    public RelayCommand(Action<object> execute)
      : this(execute, null)
    {

    }

    /// <summary>
    /// Initialise specifiying both the execution, and can-execute delegates
    /// </summary>
    /// <param name="execute">The execution delegate</param>
    /// <param name="canExecute">The delegate responsible for checking execution is allowed</param>
    public RelayCommand(Action<object> execute, Predicate<object> canExecute)
    {
      if (execute == null)
        throw new ArgumentNullException("execute");
      _execute = execute;
      _canExecute = canExecute;
    }

    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
      return _canExecute == null || _canExecute(parameter);
    }

    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    public event EventHandler CanExecuteChanged
    {
      add
      {
        CommandManager.RequerySuggested += value;
      }
      remove
      {
        CommandManager.RequerySuggested -= value;
      }
    }
    /// <summary>
    /// The method called when the command is to be executed
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
    public void Execute(object parameter)
    {
      _execute(parameter);
    }
  }

}
