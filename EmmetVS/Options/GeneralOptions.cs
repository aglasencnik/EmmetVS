using Community.VisualStudio.Toolkit;
using System.ComponentModel;

namespace EmmetVS.Options;

/// <summary>
/// Represents the general options page.
/// </summary>
public class GeneralOptions : BaseOptionModel<GeneralOptions>
{
    private bool _enable = true;
    private bool _enableAdvanced = false;
    private bool _enableTabKey = true;

    /// <summary>
    /// Gets or sets the property changed event handler.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Gets or sets whether extension is enabled.
    /// </summary>
    [Category("General Options")]
    [DisplayName("Enable Emmet")]
    [Description("Select whether to enable Emmet.")]
    [DefaultValue(true)]
    public bool Enable
    {
        get => _enable;
        set
        {
            if (_enable != value)
            {
                _enable = value;
                OnPropertyChanged(nameof(Enable));
            }
        }
    }

    /// <summary>
    /// Gets or sets whether to advanced functionality is enabled.
    /// </summary>
    [Category("General Options")]
    [DisplayName("Enable Advanced Emmet")]
    [Description("Select whether to enable advanced Emmet functionalities.")]
    [DefaultValue(false)]
    public bool EnableAdvanced
    {
        get => _enableAdvanced;
        set
        {
            if (_enableAdvanced != value)
            {
                _enableAdvanced = value;
                OnPropertyChanged(nameof(EnableAdvanced));
            }
        }
    }

    /// <summary>
    /// Gets or sets whether to enable expanding abbreviations with tab key.
    /// </summary>
    [Category("General Options")]
    [DisplayName("Enable Expanding Abbreviation With Tab Key")]
    [Description("Select whether to enable expanding abbreviations with tab key.")]
    [DefaultValue(true)]
    public bool EnableTabKey
    {
        get => _enableTabKey;
        set
        {
            if (_enableTabKey != value)
            {
                _enableTabKey = value;
                OnPropertyChanged(nameof(EnableTabKey));
            }
        }
    }

    /// <summary>
    /// Handles on property changed event
    /// </summary>
    /// <param name="propertyName">Property name</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
