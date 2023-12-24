using Community.VisualStudio.Toolkit;
using System.ComponentModel;

namespace EmmetVS.Options;

/// <summary>
/// Represents the runtime options.
/// </summary>
public class RuntimeOptions : BaseOptionModel<RuntimeOptions>
{
    /// <summary>
    /// Gets or sets whether default values have been set.
    /// </summary>
    [Browsable(false)]
    public bool DefaultValuesSet { get; set; } = false;
}
