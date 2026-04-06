using System;
using System.Threading;
using System.Windows.Input;

namespace SimpleImageResizer.Commands;

/// <summary>
/// Provides implementation for asynchronously relaying commands for MVVM.
/// </summary>
/// <remarks>
/// Avoids using a VOID ASYNC in the VM which is difficult to debug among other things.
/// </remarks>
public class RelayAsync(Func<object, Task> executeAsync, Func<object, bool>? canExecutePredicate = null) : ICommand
{
    private readonly Func<object, Task> execute = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
    private readonly Func<object, bool> canExecute = canExecutePredicate ?? (_ => true);

    private long isExecuting;

    /// <summary>
    /// Event handler. More info needed.
    /// </summary>
    public event EventHandler? CanExecuteChanged
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
    /// Forces CommandManager to raise the event.
    /// </summary>
    public static void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }

    /// <summary>
    /// Determines if the task can be executed.
    /// </summary>
    /// <param name="parameter">Parameter.</param>
    /// <returns>Bool.</returns>
    public bool CanExecute(object? parameter)
    {
        parameter ??= false;
        return Interlocked.Read(ref isExecuting) == 0 && canExecute(parameter);
    }

    /// <summary>
    /// Execute task using async.
    /// </summary>
    /// <param name="parameter">Parameter.</param>
    public async void Execute(object? parameter)
    {
        parameter ??= false;

        Interlocked.Exchange(ref isExecuting, 1);
        RaiseCanExecuteChanged();

        try
        {
            await execute(parameter);
        }
        catch
        {
            return;
        }
        finally
        {
            Interlocked.Exchange(ref isExecuting, 0);
            RaiseCanExecuteChanged();
        }
    }
}
