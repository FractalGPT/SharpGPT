using System.ComponentModel;

namespace FractalGPT.SharpGPTLib.Clients.Tavily.Models;

public enum TopicType
{
    [Description("general")]
    General = 1,

    [Description("news")]
    News = 2,

    [Description("finance")]
    Finance = 3,
}
