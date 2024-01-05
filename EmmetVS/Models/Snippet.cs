namespace EmmetVS.Models;

/// <summary>
/// Represents a snippet model.
/// </summary>
internal class Snippet
{
    /// <summary>
    /// Gets or sets the name of the snippet.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the snippet author.
    /// </summary>
    public string Author { get; set; }

    /// <summary>
    /// Gets or sets the snippet language.
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// Gets or sets the snippet prefix.
    /// </summary>
    public string Prefix { get; set; }

    /// <summary>
    /// Gets or sets the snippet code.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the snippet code literals.
    /// </summary>
    public Literal[] Literals { get; set; }
}
