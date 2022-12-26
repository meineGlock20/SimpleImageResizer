using System.Windows.Input;

namespace SimpleImageResizer.Commands;

/// <summary>
/// Provides implementation for relaying commands for MVVM.
/// </summary>
public class Relay : ICommand
{
    private Action<object> execute;
    private Predicate<object> canExecute;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="execute">Method to execute.</param>
    public Relay(Action<object> execute)
        : this(execute, DefaultCanExecute)
    { }

    /// <summary>
    /// Constructor. Overloaded.
    /// </summary>
    /// <param name="execute">Method to execute.</param>
    /// <param name="canExecute">Can this be executed?.</param>
    public Relay(Action<object> execute, Predicate<object> canExecute)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
    }

    /// <inheritdoc/>
    public event EventHandler? CanExecuteChanged
    {
        add
        {
            CommandManager.RequerySuggested += value;
            CanExecuteChangedInternal += value;
        }

        remove
        {
            CommandManager.RequerySuggested -= value;
            CanExecuteChangedInternal -= value;
        }
    }

    private event EventHandler? CanExecuteChangedInternal;

    /// <summary>
    /// Determine if it can be executed.
    /// </summary>
    /// <param name="parameter">Parameter.</param>
    /// <returns>Bool.</returns>
    public bool CanExecute(object? parameter)
    {
        return canExecute != null && canExecute(parameter ?? false);
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="parameter">Parameter.</param>
    public void Execute(object? parameter)
    {
        execute(parameter ?? false);
    }

    /// <summary>
    /// Fires when CanExecute is changed.
    /// </summary>
    public void OnCanExecuteChanged()
    {
        EventHandler handler = CanExecuteChangedInternal ?? throw new InvalidOperationException(null);
        handler?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Clean up.
    /// </summary>
    public void Destroy()
    {
        canExecute = _ => false;
        execute = _ => { return; };
    }

    private static bool DefaultCanExecute(object parameter)
    {
        return true;
    }
}