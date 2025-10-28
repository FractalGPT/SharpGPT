using System.ComponentModel;

namespace FractalGPT.SharpGPTLib.Clients.Tavily.Models;

public enum FormatType
{
    [Description("markdown")]
    Markdown = 1,

    [Description("text")]
    Text = 2,
}
