using Community.VisualStudio.Toolkit;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EmmetVS.Options;

/// <summary>
/// Represents the XSL options
/// </summary>
public class XslOptions : BaseOptionModel<XslOptions>
{
    /// <summary>
    /// Gets or sets the property changed event handler.
    /// </summary>
    public event PropertyChangedEventHandler SnippetsChanged;

    /// <summary>
    /// Serialized representation of Snippets dictionary.
    /// </summary>
    [Browsable(false)]
    public string SnippetsSerialized { get; set; }

    /// <summary>
    /// Gets or sets the XSL snippets.
    /// </summary>
    [Browsable(false)]
    public Dictionary<string, string> Snippets
    {
        get { return JsonConvert.DeserializeObject<Dictionary<string, string>>(SnippetsSerialized ?? "{}"); }
        set 
        { 
            SnippetsSerialized = JsonConvert.SerializeObject(value); 
            OnSnippetsChanged();
        }
    }

    /// <summary>
    /// Serialized representation of SupportedFileTypes list.
    /// </summary>
    [Browsable(false)]
    public string SupportedFileTypesSerialized { get; set; }

    /// <summary>
    /// Gets or sets the supported file types.
    /// </summary>
    [Browsable(false)]
    public List<string> SupportedFileTypes
    {
        get { return JsonConvert.DeserializeObject<List<string>>(SupportedFileTypesSerialized ?? "[]")?.Select(e => e.ToLower())?.ToList(); }
        set { SupportedFileTypesSerialized = JsonConvert.SerializeObject(value?.Select(e => e.ToLower())); }
    }

    /// <summary>
    /// Handles on property changed event for snippets.
    /// </summary>
    protected virtual void OnSnippetsChanged()
    {
        SnippetsChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Snippets)));
    }
}
