using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace EmmetVS.Options.CustomOptionPages;

/// <summary>
/// Represents the custom options page provider class
/// </summary>
internal partial class CustomOptionsPageProvider
{
    /// <summary>
    /// Gets the options pages.
    /// </summary>
    /// <typeparam name="T">User control</typeparam>
    [ComVisible(true)]
    internal abstract class BaseOptionPage<T> : UIElementDialogPage where T : UserControl, new()
    {
        protected override UIElement Child
        {
            get
            {
                return new T();
            }
        }
    }

    /// <summary>
    /// Represents the CSS snippets options page
    /// </summary>
    [ComVisible(true)]
    [Guid("AEE6F29F-54D7-4C98-A825-76FF59D71491")]
    public class CssSnippetsOptionsPage : BaseOptionPage<CssSnippetsOptionsUserControl> { }

    /// <summary>
    /// Represents the CSS supported file types options page
    /// </summary>
    [ComVisible(true)]
    [Guid("70341202-8BBB-4E6E-8CC6-C95F8278DD48")]
    public class CssSupportedFileTypesOptionsPage : BaseOptionPage<CssSupportedFileTypesOptionsUserControl> { }

    /// <summary>
    /// Represents the HTML snippets options page
    /// </summary>
    [ComVisible(true)]
    [Guid("E88EEA74-E886-42FD-963A-2F1284895B2F")]
    public class HtmlSnippetsOptionsPage : BaseOptionPage<HtmlSnippetsOptionsUserControl> { }

    /// <summary>
    /// Represents the HTML supported file types options page
    /// </summary>
    [ComVisible(true)]
    [Guid("BBA3AD02-1BEB-4910-BE19-274ED0ED057B")]
    public class HtmlSupportedFileTypesOptionsPage : BaseOptionPage<HtmlSupportedFileTypesOptionsUserControl> { }

    /// <summary>
    /// Represents the variable options page
    /// </summary>
    [ComVisible(true)]
    [Guid("8596D678-0403-42CD-80A0-7665B93694C4")]
    public class VariableOptionsPage : BaseOptionPage<VariableOptionsUserControl> { }

    /// <summary>
    /// Represents the XSL snippets options page
    /// </summary>
    [ComVisible(true)]
    [Guid("982E467B-19E2-49A9-8F04-CDDAA3337BF6")]
    public class XslSnippetsOptionsPage : BaseOptionPage<XslSnippetsOptionsUserControl> { }

    /// <summary>
    /// Represents the XSL supported file types options page
    /// </summary>
    [ComVisible(true)]
    [Guid("6AE7D8CB-330D-4B4B-8DBF-35B0AF400136")]
    public class XslSupportedFileTypesOptionsPage : BaseOptionPage<XslSupportedFileTypesOptionsUserControl> { }
}
