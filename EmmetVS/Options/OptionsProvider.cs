using Community.VisualStudio.Toolkit;
using System.Runtime.InteropServices;

namespace EmmetVS.Options;

/// <summary>
/// Represents the options provider class.
/// </summary>
internal partial class OptionsProvider
{
    /// <summary>
    /// Represents the general options provider.
    /// </summary>
    [ComVisible(true)]
    public class GeneralOptionsProvider : BaseOptionPage<GeneralOptions> { }

    /// <summary>
    /// Represents the HTML options provider.
    /// </summary>
    [ComVisible(true)]
    public class HtmlOptionsProvider : BaseOptionPage<HtmlOptions> { }

    /// <summary>
    /// Represents the CSS options provider.
    /// </summary>
    [ComVisible(true)]
    public class CssOptionsProvider : BaseOptionPage<CssOptions> { }

    /// <summary>
    /// Represents the XSL options provider.
    /// </summary>
    [ComVisible(true)]
    public class XslOptionsProvider : BaseOptionPage<XslOptions> { }

    /// <summary>
    /// Represents the variable options provider.
    /// </summary>
    [ComVisible(true)]
    public class VariableOptionsProvider : BaseOptionPage<VariableOptions> { }

    /// <summary>
    /// Represents the configuration options provider.
    /// </summary>
    [ComVisible(true)]
    public class ConfigurationOptionsProvider : BaseOptionPage<ConfigurationOptions> { }

    /// <summary>
    /// Represents the runtime options provider.
    /// </summary>
    [ComVisible(true)]
    public class RuntimeOptionsProvider : BaseOptionPage<RuntimeOptions> { }
}
