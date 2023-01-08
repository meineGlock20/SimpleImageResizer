using SimpleImageResizer.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleImageResizer.ViewModels;

public class BatchWindowViewModel : BaseModel, INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>> errorList = new();

    private string? directory;
    private bool subDirectories;

    /// <summary>
    /// Constructor.
    /// </summary>
    public BatchWindowViewModel()
    {
        CommandProcess = new Commands.Relay(Process, p => !HasErrors && !string.IsNullOrWhiteSpace(Directory));
        CommandChooseDirectory = new Commands.Relay(ChooseDirectory, p => true);
    }

    /// <summary>
    /// Gets or sets a value indicating an Action that will close the window.
    /// </summary>
    /// <remarks>
    /// See the code behind for implementation.
    /// </remarks>
    public Action? CloseAction { get; set; }

    public ICommand CommandProcess { get; set; }
    public ICommand CommandChooseDirectory { get; set; }

    private void Process(object o)
    {
        Core.BatchImaging.DirectoryToProcess = Directory;
        Core.BatchImaging.IncludeSubDirectories = SubDirectories;
        CloseAction?.Invoke();
    }

    private void ChooseDirectory(object o)
    {
        string? d = Core.DirectoryBrowser.GetDirectory(Localize.BatchWindow.SelectDirectoryTitle);
        if (!string.IsNullOrWhiteSpace(d))
        {
            Directory = d;
        }
    }

    public string? Directory
    {
        get => directory;
        set
        {
            directory = value;
            ValidateProperties();
            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(HasErrors));
        }
    }

    public bool SubDirectories
    {
        get => subDirectories;
        set
        {
            subDirectories = value;
            Core.BatchImaging.IncludeSubDirectories = SubDirectories;
            NotifyPropertyChanged();
        }
    }

    /* ERROR HANDLING */

    /// <summary>
    /// Gets a value indicating whether there are any errors in the error list.
    /// </summary>
    public bool HasErrors => errorList.Any();

    /// <summary>
    /// Event fires when a validation error occurs for a property or an entire entity.
    /// </summary>
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <summary>
    /// Validates passed properties for errors and if any, adds them to the dictionary of errorList.
    /// </summary>
    /// <param name="propertyName">Property to check for errors.</param>
    private void ValidateProperties([CallerMemberName] string propertyName = "")
    {
        ClearErrors(propertyName);

        switch (propertyName)
        {
            case nameof(Directory):
                {
                    if (string.IsNullOrWhiteSpace(Directory) ||
                        !System.IO.Directory.Exists(Directory))
                    {
                        AddError(propertyName, Localize.BatchWindow.ValidDirectoryRequired);
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Adds an error to the dictionary.
    /// </summary>
    /// <param name="propertyName">Property name.</param>
    /// <param name="error">Error.</param>
    private void AddError(string propertyName, string error)
    {
        if (!errorList.ContainsKey(propertyName))
        {
            errorList[propertyName] = new List<string>();
        }

        if (!errorList[propertyName].Contains(error))
        {
            errorList[propertyName].Add(error);
            OnErrorsChanged(propertyName);
        }

        NotifyPropertyChanged(nameof(HasErrors));
    }

    /// <summary>
    /// Gets a list of errors by property.
    /// </summary>
    /// <param name="propertyName">Property name.</param>
    /// <returns>A list of errors for a property or empty if none exist.</returns>
    public IEnumerable GetErrors([CallerMemberName] string? propertyName = "")
    {
        if (errorList.TryGetValue(propertyName ?? "", out List<string>? value))
            return value;
        else
            return Enumerable.Empty<string>();
    }

    /// <summary>
    /// Clears any errors from the dictionary for the passed property from ValidateProperties.
    /// </summary>
    /// <param name="propertyName"></param>
    private void ClearErrors(string propertyName)
    {
        errorList.Remove(propertyName);
        NotifyPropertyChanged(nameof(HasErrors));
    }

    /// <summary>
    /// Whenever the error condition of a property changes, invoke the DataErrorsChangedEventArgs.
    /// Called from AddError and ClearErrors.
    /// </summary>
    /// <param name="propertyName">propertyName.</param>
    private void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    /* END ERROR HANDLING */
}
