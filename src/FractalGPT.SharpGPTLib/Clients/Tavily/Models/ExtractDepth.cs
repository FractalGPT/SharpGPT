using System.ComponentModel;

namespace FractalGPT.SharpGPTLib.Clients.Tavily.Models;

public enum ExtractDepth
{
    [Description("basic")]
    Basic = 1,

    [Description("advanced")]
    Advanced = 2,
}
