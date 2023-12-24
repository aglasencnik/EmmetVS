using Community.VisualStudio.Toolkit;
using EmmetVS.Enums;
using System.ComponentModel;

namespace EmmetVS.Options;

/// <summary>
/// Represents the options page for Emmet configuration.
/// </summary>
public class ConfigurationOptions : BaseOptionModel<ConfigurationOptions>
{
    /// <summary>
    /// Gets or sets the bem.elementSeparator preference.
    /// </summary>
    [Category("General")]
    [DisplayName("inlineElements")]
    [Description("List of elements that should be expanded inline (split by \",\").")]
    [DefaultValue("a, abbr, acronym, applet, b, basefont, bdo, big, br, button, cite, code, del, dfn, em, font, i, iframe, img, input, ins, kbd, label, map, object, q, s, samp, select, small, span, strike, strong, sub, sup, textarea, tt, u, var")]
    public string InlineElements { get; set; } = "a, abbr, acronym, applet, b, basefont, bdo, big, br, button, cite, code, del, dfn, em, font, i, iframe, img, input, ins, kbd, label, map, object, q, s, samp, select, small, span, strike, strong, sub, sup, textarea, tt, u, var";

    /// <summary>
    /// Gets or sets the output.indent preference.
    /// </summary>
    [Category("Output")]
    [DisplayName("output.indent")]
    [Description("Indentation string for output markup.")]
    [DefaultValue("\t")]
    public string OutputIndent { get; set; } = "\t";

    /// <summary>
    /// Gets or sets the output.baseIndent preference.
    /// </summary>
    [Category("Output")]
    [DisplayName("output.baseIndent")]
    [Description("Base indentation string for output markup.")]
    [DefaultValue("")]
    public string OutputBaseIndent { get; set; } = "";

    /// <summary>
    /// Gets or sets the output.newline preference.
    /// </summary>
    [Category("Output")]
    [DisplayName("output.newline")]
    [Description("New line string for output markup.")]
    [DefaultValue("\n")]
    public string OutputNewLine { get; set; } = "\n";

    /// <summary>
    /// Gets or sets the output.tagCase preference.
    /// </summary>
    [Category("Output")]
    [DisplayName("output.tagCase")]
    [Description("Tag case for output markup.")]
    [DefaultValue(StringCase.AsIs)]
    [TypeConverter(typeof(EnumConverter))]
    public StringCase OutputTagCase { get; set; } = StringCase.AsIs;

    /// <summary>
    /// Gets or sets the output.attributeCase preference.
    /// </summary>
    [Category("Output")]
    [DisplayName("output.attributeCase")]
    [Description("Attribute case for output markup.")]
    [DefaultValue(StringCase.AsIs)]
    [TypeConverter(typeof(EnumConverter))]
    public StringCase OutputAttributeCase { get; set; } = StringCase.AsIs;

    /// <summary>
    /// Gets or sets the output.attributeQuotes preference.
    /// </summary>
    [Category("Output")]
    [DisplayName("output.attributeQuotes")]
    [Description("Attribute quotes for output markup.")]
    [DefaultValue(AttributeQuotes.Double)]
    [TypeConverter(typeof(EnumConverter))]
    public AttributeQuotes OutputAttributeQuotes { get; set; } = AttributeQuotes.Double;

    /// <summary>
    /// Gets or sets the output.format preference.
    /// </summary>
    [Category("Output")]
    [DisplayName("output.format")]
    [Description("Select whether to output in formatted form.")]
    [DefaultValue(true)]
    public bool OutputFormat { get; set; } = true;

    /// <summary>
    /// Gets or sets the output.formatLeafNode preference.
    /// </summary>
    [Category("Output")]
    [DisplayName("output.formatLeafNode")]
    [Description("Select whether to format the leaf node.")]
    [DefaultValue(false)]
    public bool OutputFormatLeafNode { get; set; } = false;

    /// <summary>
    /// Gets or sets the output.formatSkip preference.
    /// </summary>
    [Category("Output")]
    [DisplayName("output.formatSkip")]
    [Description("List of tag names that should be skipped in output markup formatting (split by \",\").")]
    [DefaultValue("html")]
    public string OutputFormatSkip { get; set; } = "html";

    /// <summary>
    /// Gets or sets the output.formatForce preference.
    /// </summary>
    [Category("Output")]
    [DisplayName("output.formatForce")]
    [Description("List of tag names that should be forced to be formatted in output markup (split by \",\").")]
    [DefaultValue("body")]
    public string OutputFormatForce { get; set; } = "body";

    /// <summary>
    /// Gets or sets the output.inlineBreak preference.
    /// </summary>
    [Category("Output")]
    [DisplayName("output.inlineBreak")]
    [Description("How many inline sibling elements should force line breaks in output markup.")]
    [DefaultValue(3)]
    public int OutputInlineBreak { get; set; } = 3;

    /// <summary>
    /// Gets or sets the output.compactBoolean preference.
    /// </summary>
    [Category("Output")]
    [DisplayName("output.compactBoolean")]
    [Description("Select whether to produce compact notation of boolean attributes in output markup.")]
    [DefaultValue(false)]
    public bool OutputCompactBoolean { get; set; } = false;

    /// <summary>
    /// Gets or sets the output.booleanAttributes preference.
    /// </summary>
    [Category("Output")]
    [DisplayName("output.booleanAttributes")]
    [Description("List of boolean attributes that should be expanded in output markup (split by \",\").")]
    [DefaultValue("contenteditable, seamless, async, autofocus, autoplay, checked, controls, defer, disabled, formnovalidate, hidden, ismap, loop, multiple, muted, novalidate, readonly, required, reversed, selected, typemustmatch")]
    public string OutputBooleanAttributes { get; set; } = "contenteditable, seamless, async, autofocus, autoplay, checked, controls, defer, disabled, formnovalidate, hidden, ismap, loop, multiple, muted, novalidate, readonly, required, reversed, selected, typemustmatch";

    /// <summary>
    /// Gets or sets the output.reverseAttributes preference.
    /// </summary>
    [Category("Output")]
    [DisplayName("output.reverseAttributes")]
    [Description("Select whether to reverse attributes in output markup.")]
    [DefaultValue(false)]
    public bool OutputReverseAttributes { get; set; } = false;

    /// <summary>
    /// Gets or sets the output.selfClosingStyle preference.
    /// </summary>
    [Category("Output")]
    [DisplayName("output.selfClosingStyle")]
    [Description("Self closing type for output markup.")]
    [DefaultValue(SelfClosingStyle.Html)]
    public SelfClosingStyle OutputSelfClosingStyle { get; set; } = SelfClosingStyle.Html;

    /// <summary>
    /// Gets or sets the OUTPUT preference.
    /// </summary>
    [Category("Markup")]
    [DisplayName("markup.href")]
    [Description("Whether to automatically update value of element's href attribute if inserting URL or email.")]
    [DefaultValue(true)]
    public bool MarkupHref { get; set; } = true;

    /// <summary>
    /// Gets or sets the markup.unitAliases preference.
    /// </summary>
    [Category("Markup")]
    [DisplayName("markup.attributes")]
    [Description("A comma-separated list of markup attributes. Each should be defined as alias:keyword_name.")]
    [DefaultValue("")]
    public string MarkupAttrributes { get; set; } = "";

    /// <summary>
    /// Gets or sets the markup.unitAliases preference.
    /// </summary>
    [Category("Markup")]
    [DisplayName("markup.valuePrefix")]
    [Description("A comma-separated list of markup value prefixes. Each should be defined as alias:keyword_name.")]
    [DefaultValue("")]
    public string MarkupValuePrefix { get; set; } = "";

    /// <summary>
    /// Gets or sets the comment.enabled preference.
    /// </summary>
    [Category("Comment")]
    [DisplayName("comment.enabled")]
    [Description("Select whether to enable output of comments in output markup.")]
    [DefaultValue(false)]
    public bool CommentEnabled { get; set; } = false;

    /// <summary>
    /// Gets or sets the comment.trigger preference.
    /// </summary>
    [Category("Comment")]
    [DisplayName("comment.trigger")]
    [Description("List of attributes that trigger comment in output markup (split by \",\").")]
    [DefaultValue("id, class")]
    public string CommentTrigger { get; set; } = "id, class";

    /// <summary>
    /// Gets or sets the comment.before preference.
    /// </summary>
    [Category("Comment")]
    [DisplayName("comment.before")]
    [Description("Template string for comment in output markup.")]
    [DefaultValue("")]
    public string CommentBefore { get; set; } = "";

    /// <summary>
    /// Gets or sets the comment.after preference.
    /// </summary>
    [Category("Comment")]
    [DisplayName("comment.after")]
    [Description("Template string for comment in output markup.")]
    [DefaultValue("\n<!-- /[#ID][.CLASS] -->")]
    public string CommentAfter { get; set; } = "\n<!-- /[#ID][.CLASS] -->";

    /// <summary>
    /// Gets or sets the bem.enabled preference.
    /// </summary>
    [Category("BEM")]
    [DisplayName("bem.enabled")]
    [Description("Select whether to enable output of BEM-like attributes in output markup.")]
    [DefaultValue(false)]
    public bool BemEnabled { get; set; } = false;

    /// <summary>
    /// Gets or sets the bem.element preference.
    /// </summary>
    [Category("BEM")]
    [DisplayName("bem.element")]
    [Description("String for seperating BEM elements in output markup.")]
    [DefaultValue("__")]
    public string BemElement { get; set; } = "__";

    /// <summary>
    /// Gets or sets the bem.modifier preference.
    /// </summary>
    [Category("BEM")]
    [DisplayName("bem.modifier")]
    [Description("String for seperating BEM modifiers in output markup.")]
    [DefaultValue("_")]
    public string BemModifier { get; set; } = "_";

    /// <summary>
    /// Gets or sets the bem.enabled preference.
    /// </summary>
    [Category("JSX")]
    [DisplayName("jsx.enabled")]
    [Description("Select whether to enable JSX syntax in output markup.")]
    [DefaultValue(false)]
    public bool JsxEnabled { get; set; } = false;

    /// <summary>
    /// Gets or sets the stylesheet.keywords preference.
    /// </summary>
    [Category("Stylesheet")]
    [DisplayName("stylesheet.keywords")]
    [Description("A comma-separated list of valid keywords that can be used in stylesheet abbreviations.")]
    [DefaultValue("auto, inherit, unset, none")]
    public string StylesheetKeywords { get; set; } = "auto, inherit, unset, none";

    /// <summary>
    /// Gets or sets the stylesheet.unitless preference.
    /// </summary>
    [Category("Stylesheet")]
    [DisplayName("stylesheet.unitless")]
    [Description("The list of properties whose values must not contain units.")]
    [DefaultValue("z-index, line-height, opacity, font-weight, zoom, flex, flex-grow, flex-shrink")]
    public string StylesheetUnitless { get; set; } = "z-index, line-height, opacity, font-weight, zoom, flex, flex-grow, flex-shrink";

    /// <summary>
    /// Gets or sets the stylesheet.shortHex preference.
    /// </summary>
    [Category("Stylesheet")]
    [DisplayName("stylesheet.shortHex")]
    [Description("Should color values like #ffffff be shortened to #fff after abbreviation with color was expanded.")]
    [DefaultValue(true)]
    public bool StylesheetShortHex { get; set; } = true;

    /// <summary>
    /// Gets or sets the stylesheet.between preference.
    /// </summary>
    [Category("Stylesheet")]
    [DisplayName("stylesheet.between")]
    [Description("Defines a symbol that should be placed between CSS property and value when expanding CSS abbreviations.")]
    [DefaultValue(": ")]
    public string StylesheetBetween { get; set; } = ": ";

    /// <summary>
    /// Gets or sets the stylesheet.after preference.
    /// </summary>
    [Category("Stylesheet")]
    [DisplayName("stylesheet.after")]
    [Description("String after property value in output stylesheet.")]
    [DefaultValue(";")]
    public string StylesheetAfter { get; set; } = ";";

    /// <summary>
    /// Gets or sets the stylesheet.intUnit preference.
    /// </summary>
    [Category("Stylesheet")]
    [DisplayName("stylesheet.intUnit")]
    [Description("Default unit for integer values.")]
    [DefaultValue("px")]
    public string StylesheetIntUnit { get; set; } = "px";

    /// <summary>
    /// Gets or sets the stylesheet.floatUnit preference.
    /// </summary>
    [Category("Stylesheet")]
    [DisplayName("stylesheet.floatUnit")]
    [Description("Default unit for floating point values.")]
    [DefaultValue("em")]
    public string StylesheetFloatUnit { get; set; } = "em";

    /// <summary>
    /// Gets or sets the stylesheet.unitAliases preference.
    /// </summary>
    [Category("Stylesheet")]
    [DisplayName("stylesheet.unitAliases")]
    [Description("A comma-separated list of unit aliases, used in CSS abbreviation. Each alias should be defined as alias:keyword_name.")]
    [DefaultValue("e:em, p:%, x:ex, r:rem")]
    public string StylesheetUnitAliases { get; set; } = "e:em, p:%, x:ex, r:rem";

    /// <summary>
    /// Gets or sets the stylesheet.json preference.
    /// </summary>
    [Category("Stylesheet")]
    [DisplayName("stylesheet.json")]
    [Description("Select whether to output abbreviation as JSON object properties.")]
    [DefaultValue(false)]
    public bool StylesheetJson { get; set; } = false;

    /// <summary>
    /// Gets or sets the stylesheet.jsonDoubleQuotes preference.
    /// </summary>
    [Category("Stylesheet")]
    [DisplayName("stylesheet.jsonDoubleQuotes")]
    [Description("Select whether to use double quotes for JSON properties in output stylesheet.")]
    [DefaultValue(false)]
    public bool StylesheetJsonDoubleQuotes { get; set; } = false;

    /// <summary>
    /// Gets or sets the stylesheet.fuzzySearchMinScore preference.
    /// </summary>
    [Category("Stylesheet")]
    [DisplayName("stylesheet.fuzzySearchMinScore")]
    [Description("Float number between 0 and 1 to pick fuzzy search result in stylesheet.")]
    [DefaultValue(0)]
    public float StylesheetFuzzySearchMinScore { get; set; } = 0;
}
