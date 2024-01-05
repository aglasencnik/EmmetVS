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
    private bool _enableExpandWithTabKey = true;
    private bool _enableWrapWithTabKey = true;
    private bool _enableSnippets = true;

    /// <summary>
    /// Gets or sets the property changed event handler.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Gets or sets the property changed event handler.
    /// </summary>
    public event PropertyChangedEventHandler SnippetsChanged;

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
    public bool EnableExpandWithTabKey
    {
        get => _enableExpandWithTabKey;
        set
        {
            if (_enableExpandWithTabKey != value)
            {
                _enableExpandWithTabKey = value;
                OnPropertyChanged(nameof(EnableExpandWithTabKey));
            }
        }
    }

    /// <summary>
    /// Gets or sets whether to enable wrapping with abbreviation with tab key.
    /// </summary>
    [Category("General Options")]
    [DisplayName("Enable Wrapping With Abbreviation With Tab Key")]
    [Description("Select whether to enable wrapping with abbreviation with tab key.")]
    [DefaultValue(true)]
    public bool EnableWrapWithTabKey
    {
        get => _enableWrapWithTabKey;
        set
        {
            if (_enableWrapWithTabKey != value)
            {
                _enableWrapWithTabKey = value;
                OnPropertyChanged(nameof(EnableWrapWithTabKey));
            }
        }
    }

    /// <summary>
    /// Gets or sets whether to enable snippets.
    /// </summary>
    [Category("General Options")]
    [DisplayName("Enable Snippets")]
    [Description("Select whether to enable snippets.")]
    [DefaultValue(true)]
    public bool EnableSnippets
    {
        get => _enableSnippets;
        set
        {
            if (_enableSnippets != value)
            {
                _enableSnippets = value;
                OnSnippetsChanged();
            }
        }
    }

    /// <summary>
    /// Handles on property changed event.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Handles on property changed event for snippets.
    /// </summary>
    protected virtual void OnSnippetsChanged()
    {
        SnippetsChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnableSnippets)));
    }
}
