using Community.VisualStudio.Toolkit;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace EmmetVS.Options;

/// <summary>
/// Represents the options page for Emmet variables.
/// </summary>
public class VariableOptions : BaseOptionModel<VariableOptions>
{
    /// <summary>
    /// Serialized representation of Variables dictionary.
    /// </summary>
    [Browsable(false)]
    public string VariablesSerialized { get; set; }

    /// <summary>
    /// Gets or sets the variables.
    /// </summary>
    [Browsable(false)]
    public Dictionary<string, string> Variables
    {
        get { return JsonConvert.DeserializeObject<Dictionary<string, string>>(VariablesSerialized ?? "{}"); }
        set { VariablesSerialized = JsonConvert.SerializeObject(value); }
    }
}
