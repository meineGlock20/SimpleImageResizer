using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleImageResizer.Commands;

/// <summary>
/// Provides implementation for asynchronously relaying commands for MVVM.
/// </summary>
/// <remarks>
/// Avoids using a VOID ASYNC in the VM which is difficult to debug among other things.
/// </remarks>
public class RelayAsync : ICommand
{
    private readonly Func<object, Task> execute;
    private readonly Func<object, bool> canExecute;

    private long isExecuting;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="execute">Task to execute.</param>
    /// <param name="canExecute">Bool to determine if it can be executed.</param>
    public RelayAsync(Func<object, Task> execute, Func<object, bool>? canExecute = null)
    {
        this.execute = execute;
        this.canExecute = canExecute ?? (o => true);
    }

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
