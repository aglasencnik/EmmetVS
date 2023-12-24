using System.Windows;
using System.Windows.Controls;

namespace EmmetVS.Options.CustomOptionPages;

/// <summary>
/// Interaction logic for FileTypeEntryDialog.xaml
/// </summary>
public partial class FileTypeEntryDialog : UserControl
{
    #region Properties

    /// <summary>
    /// Gets or sets the Value
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// Gets or sets the old value
    /// </summary>
    public string OldValue { get; set; }

    /// <summary>
    /// Gets or sets the IsCancelled property
    /// </summary>
    public bool IsCancelled { get; private set; } = true;

    /// <summary>
    /// Gets or sets the event for dialog result event
    /// </summary>
    public event EventHandler<bool> DialogResultEvent;

    /// <summary>
    /// Gets or sets thge close dialog event handler delegate
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">EventArgs object</param>
    public delegate void CloseDialogEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Gets or sets the CloseDialogEventHandler event object
    /// </summary>
    public event CloseDialogEventHandler CloseDialogRequested;

    #endregion

    #region Ctor

    public FileTypeEntryDialog(string value = "", bool isValueEnabled = true, string actionButtonText = "Save", string lblValueText = "File Extension")
    {
        InitializeComponent();

        OldValue = value;
        lblValue.Content = lblValueText;
        txtValue.Text = value;
        txtValue.IsEnabled = isValueEnabled;
        btnAction.Content = actionButtonText;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handles button action click
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">RoutedEventArgs object</param>
    private void BtnAction_Click(object sender, RoutedEventArgs e)
    {
        Value = txtValue.Text;
        IsCancelled = false;

        DialogResultEvent?.Invoke(this, false);
        CloseDialogRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Handles button cancel click
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">RoutedEventArgs object</param>
    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        IsCancelled = true;
        DialogResultEvent?.Invoke(this, true);
        CloseDialogRequested?.Invoke(this, EventArgs.Empty);
    }

    #endregion
}
