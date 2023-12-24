using EmmetVS.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace EmmetVS.Options.CustomOptionPages;

/// <summary>
/// Interaction logic for CssSupportedFileTypesOptionsUserControl.xaml
/// </summary>
public partial class CssSupportedFileTypesOptionsUserControl : UserControl
{
    public ObservableCollection<string> SettingsCollection { get; set; }

    public CssSupportedFileTypesOptionsUserControl()
    {
        InitializeComponent();

        SettingsCollection = new ObservableCollection<string>(CssOptions.Instance.SupportedFileTypes);
        listView.ItemsSource = SettingsCollection;
    }

    #region ListView

    /// <summary>
    /// Handles on list view selection changes event
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">SelectionChangedEventArgs object</param>
    private void OnListViewSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        var selectedItem = listView.SelectedItem;
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
        var gridView = listView.View as GridView;

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
        var dialogControl = new FileTypeEntryDialog(actionButtonText: "Add");
        dialogControl.DialogResultEvent += AddEventHandler;

        var dialogWindow = new Window()
        {
            Title = "Add new CSS supported file type",
            Content = dialogControl,
            SizeToContent = SizeToContent.WidthAndHeight,
            ResizeMode = ResizeMode.CanResizeWithGrip,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };

        // Subscribe to the CloseDialogRequested event
        ((FileTypeEntryDialog)dialogWindow.Content).CloseDialogRequested += (s, args) =>
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
            var dialogControl = sender as FileTypeEntryDialog;

            if (!isCancelled)
            {
                if (!string.IsNullOrWhiteSpace(dialogControl.Value))
                {
                    var currentFileTypes = CssOptions.Instance.SupportedFileTypes;
                    currentFileTypes.Add(dialogControl.Value);
                    CssOptions.Instance.SupportedFileTypes = currentFileTypes;

                    await CssOptions.Instance.SaveAsync();

                    SettingsCollection = new ObservableCollection<string>(CssOptions.Instance.SupportedFileTypes);
                    listView.ItemsSource = SettingsCollection;
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
        var selectedItem = listView.SelectedItem as string;
        if (string.IsNullOrWhiteSpace(selectedItem)) return;

        var dialogControl = new FileTypeEntryDialog(value: selectedItem, actionButtonText: "Save");
        dialogControl.DialogResultEvent += EditEventHandler;

        var dialogWindow = new Window()
        {
            Title = "Edit CSS supported file type",
            Content = dialogControl,
            SizeToContent = SizeToContent.WidthAndHeight,
            ResizeMode = ResizeMode.CanResizeWithGrip,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };

        // Subscribe to the CloseDialogRequested event
        ((FileTypeEntryDialog)dialogWindow.Content).CloseDialogRequested += (s, args) =>
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
            var dialogControl = sender as FileTypeEntryDialog;

            if (!isCancelled)
            {
                if (!string.IsNullOrWhiteSpace(dialogControl.Value) && !string.IsNullOrWhiteSpace(dialogControl.OldValue))
                {
                    var currentFileTypes = CssOptions.Instance.SupportedFileTypes;

                    int index = currentFileTypes.IndexOf(dialogControl.OldValue);
                    if (index != -1)
                        currentFileTypes[index] = dialogControl.Value;

                    CssOptions.Instance.SupportedFileTypes = currentFileTypes;

                    await CssOptions.Instance.SaveAsync();

                    SettingsCollection = new ObservableCollection<string>(CssOptions.Instance.SupportedFileTypes);
                    listView.ItemsSource = SettingsCollection;
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
        var selectedItem = listView.SelectedItem as string;
        if (string.IsNullOrWhiteSpace(selectedItem)) return;

        var dialogControl = new FileTypeEntryDialog(value: selectedItem, false, "Delete");
        dialogControl.DialogResultEvent += DeleteEventHandler;

        var dialogWindow = new Window()
        {
            Title = "Delete CSS supported file type",
            Content = dialogControl,
            SizeToContent = SizeToContent.WidthAndHeight,
            ResizeMode = ResizeMode.CanResizeWithGrip,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };

        // Subscribe to the CloseDialogRequested event
        ((FileTypeEntryDialog)dialogWindow.Content).CloseDialogRequested += (s, args) =>
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
            var dialogControl = sender as FileTypeEntryDialog;

            if (!isCancelled)
            {
                if (!string.IsNullOrWhiteSpace(dialogControl.Value))
                {
                    var currentFileTypes = CssOptions.Instance.SupportedFileTypes;

                    if (currentFileTypes.Contains(dialogControl.Value))
                        currentFileTypes.Remove(dialogControl.Value);

                    CssOptions.Instance.SupportedFileTypes = currentFileTypes;

                    await CssOptions.Instance.SaveAsync();

                    SettingsCollection = new ObservableCollection<string>(CssOptions.Instance.SupportedFileTypes);
                    listView.ItemsSource = SettingsCollection;
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
