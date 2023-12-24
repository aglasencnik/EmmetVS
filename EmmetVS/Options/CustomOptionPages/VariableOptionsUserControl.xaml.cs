using EmmetVS.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace EmmetVS.Options.CustomOptionPages;

/// <summary>
/// Interaction logic for VariableOptionsUserControl.xaml
/// </summary>
public partial class VariableOptionsUserControl : UserControl
{
    public ObservableCollection<KeyValuePair<string, string>> SettingsDict { get; set; }

    public VariableOptionsUserControl()
    {
        InitializeComponent();

        SettingsDict = new ObservableCollection<KeyValuePair<string, string>>(VariableOptions.Instance.Variables);
        dictListView.ItemsSource = SettingsDict;
    }

    #region ListView

    /// <summary>
    /// Handles on list view selection changes event
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">SelectionChangedEventArgs object</param>
    private void OnListViewSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        var selectedItem = dictListView.SelectedItem;
        btnEdit.IsEnabled = selectedItem != null;
        btnDelete.IsEnabled = selectedItem != null;
    }

    // Dictionary to store the maximum width encountered for each column
    private readonly Dictionary<int, double> maxColumnWidths = new();

    /// <summary>
    /// Handles the ListViewItem Loaded event
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">RoutedEventArgs object</param>
    private void ListViewItem_Loaded(object sender, RoutedEventArgs e)
    {
        var gridView = dictListView.View as GridView;

        if (sender is ListViewItem item && gridView != null)
        {
            var columnIndex = 0;
            foreach (var column in gridView.Columns)
            {
                var cellContent = OptionsUserControlHelper.FindChild<TextBlock>(item, columnIndex);
                if (cellContent != null)
                {
                    cellContent.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    var currentWidth = cellContent.DesiredSize.Width;

                    if (maxColumnWidths.ContainsKey(columnIndex))
                    {
                        if (currentWidth > maxColumnWidths[columnIndex])
                        {
                            maxColumnWidths[columnIndex] = currentWidth;
                        }
                    }
                    else
                    {
                        maxColumnWidths.Add(columnIndex, currentWidth);
                    }
                }
                columnIndex++;
            }
        }

        // Adjust column widths based on the maximum content width encountered
        var colIndex = 0;
        foreach (var column in gridView.Columns)
        {
            if (maxColumnWidths.ContainsKey(colIndex) && maxColumnWidths[colIndex] > column.ActualWidth)
            {
                column.Width = maxColumnWidths[colIndex];
            }
            colIndex++;
        }
    }

    #endregion

    #region Actions

    #region Add

    /// <summary>
    /// Adds an item
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">RoutedEventArgs object</param>
    private void OnAdd(object sender, RoutedEventArgs e)
    {
        var dialogControl = new SnippetEntryDialog(actionButtonText: "Add", lblKeyText: "Name");
        dialogControl.DialogResultEvent += AddEventHandler;

        var dialogWindow = new Window()
        {
            Title = "Add new variable",
            Content = dialogControl,
            SizeToContent = SizeToContent.WidthAndHeight,
            ResizeMode = ResizeMode.CanResizeWithGrip,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };

        // Subscribe to the CloseDialogRequested event
        ((SnippetEntryDialog)dialogWindow.Content).CloseDialogRequested += (s, args) =>
        {
            dialogWindow.Close();
        };

        dialogWindow.ShowDialog();
    }

    /// <summary>
    /// Handles add event
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="isCancelled">Is cancelled boolean</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Event handler requires async void.")]
    private async void AddEventHandler(object sender, bool isCancelled)
    {
        try
        {
            var dialogControl = sender as SnippetEntryDialog;

            if (!isCancelled)
            {
                if (!string.IsNullOrWhiteSpace(dialogControl.Key) && !string.IsNullOrWhiteSpace(dialogControl.Value))
                {
                    var currentSnippets = VariableOptions.Instance.Variables;
                    currentSnippets.Add(dialogControl.Key, dialogControl.Value);
                    VariableOptions.Instance.Variables = currentSnippets;

                    await VariableOptions.Instance.SaveAsync();

                    SettingsDict = new ObservableCollection<KeyValuePair<string, string>>(VariableOptions.Instance.Variables);
                    dictListView.ItemsSource = SettingsDict;
                }
            }

            // Cleanup
            dialogControl.DialogResultEvent -= AddEventHandler;
        }
        catch
        {
        }
    }

    #endregion

    #region Edit

    /// <summary>
    /// Edits item
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">RoutedEventArgs object</param>
    private void OnEdit(object sender, RoutedEventArgs e)
    {
        var selectedItem = dictListView.SelectedItem as KeyValuePair<string, string>?;
        if (!selectedItem.HasValue) return;

        var selectedKey = selectedItem.Value.Key;
        var selectedValue = selectedItem.Value.Value;

        var dialogControl = new SnippetEntryDialog(key: selectedKey, isKeyEnabled: false, value: selectedValue, actionButtonText: "Save", lblKeyText: "Name");
        dialogControl.DialogResultEvent += EditEventHandler;

        var dialogWindow = new Window()
        {
            Title = "Edit variable",
            Content = dialogControl,
            SizeToContent = SizeToContent.WidthAndHeight,
            ResizeMode = ResizeMode.CanResizeWithGrip,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };

        // Subscribe to the CloseDialogRequested event
        ((SnippetEntryDialog)dialogWindow.Content).CloseDialogRequested += (s, args) =>
        {
            dialogWindow.Close();
        };

        dialogWindow.ShowDialog();
    }

    /// <summary>
    /// Handles edit event
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="isCancelled">Is cancelled boolean</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Event handler requires async void.")]
    private async void EditEventHandler(object sender, bool isCancelled)
    {
        try
        {
            var dialogControl = sender as SnippetEntryDialog;

            if (!isCancelled)
            {
                if (!string.IsNullOrWhiteSpace(dialogControl.Key) && !string.IsNullOrWhiteSpace(dialogControl.Value))
                {
                    var currentSnippets = VariableOptions.Instance.Variables;

                    if (currentSnippets.ContainsKey(dialogControl.Key))
                        currentSnippets[dialogControl.Key] = dialogControl.Value;
                    else
                        currentSnippets.Add(dialogControl.Key, dialogControl.Value);

                    VariableOptions.Instance.Variables = currentSnippets;

                    await VariableOptions.Instance.SaveAsync();

                    SettingsDict = new ObservableCollection<KeyValuePair<string, string>>(VariableOptions.Instance.Variables);
                    dictListView.ItemsSource = SettingsDict;
                }
            }

            // Cleanup
            dialogControl.DialogResultEvent -= EditEventHandler;
        }
        catch
        {
        }
    }

    #endregion

    #region Delete

    /// <summary>
    /// Deletes item
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">RoutedEventArgs object</param>
    private void OnDelete(object sender, RoutedEventArgs e)
    {
        var selectedItem = dictListView.SelectedItem as KeyValuePair<string, string>?;
        if (!selectedItem.HasValue) return;

        var selectedKey = selectedItem.Value.Key;
        var selectedValue = selectedItem.Value.Value;

        var dialogControl = new SnippetEntryDialog(selectedKey, false, selectedValue, false, "Delete", lblKeyText: "Name");
        dialogControl.DialogResultEvent += DeleteEventHandler;

        var dialogWindow = new Window()
        {
            Title = "Delete variable",
            Content = dialogControl,
            SizeToContent = SizeToContent.WidthAndHeight,
            ResizeMode = ResizeMode.CanResizeWithGrip,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };

        // Subscribe to the CloseDialogRequested event
        ((SnippetEntryDialog)dialogWindow.Content).CloseDialogRequested += (s, args) =>
        {
            dialogWindow.Close();
        };

        dialogWindow.ShowDialog();
    }

    /// <summary>
    /// Handles delete event
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="isCancelled">Is cancelled boolean</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Event handler requires async void.")]
    private async void DeleteEventHandler(object sender, bool isCancelled)
    {
        try
        {
            var dialogControl = sender as SnippetEntryDialog;

            if (!isCancelled)
            {
                if (!string.IsNullOrWhiteSpace(dialogControl.Key))
                {
                    var currentSnippets = VariableOptions.Instance.Variables;

                    if (currentSnippets.ContainsKey(dialogControl.Key))
                        currentSnippets.Remove(dialogControl.Key);

                    VariableOptions.Instance.Variables = currentSnippets;

                    await VariableOptions.Instance.SaveAsync();

                    SettingsDict = new ObservableCollection<KeyValuePair<string, string>>(VariableOptions.Instance.Variables);
                    dictListView.ItemsSource = SettingsDict;
                }
            }

            // Cleanup
            dialogControl.DialogResultEvent -= DeleteEventHandler;
        }
        catch
        {
        }
    }

    #endregion

    #endregion
}
